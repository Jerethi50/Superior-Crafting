using System;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace RimWorld
{
    // Incident Worker "BoulderMassHit" - spawns a Thing in the game world that is responsible for
    // doing the actual work
    public class IncidentWorker_BoulderMassHit : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            // Spawn me in the game world, please
            IntVec3 intVec = GenCellFinder.RandomCellWith((IntVec3 sq) => sq.Standable() && !Find.RoofGrid.Roofed(sq) && !sq.Fogged());
            GenSpawn.Spawn(ThingDef.Named("Thing_MeteorSpawner"), intVec);

            // Message the player that Quaggan is going to foo up their day
            Find.History.AddGameEvent("Meteor shower imminent.", GameEventType.BadUrgent, true, string.Empty);

            // We successfully initialized our event, return true
            return true;
        }
    }

    // I so wish we weren't doing this via invisible Thing. Anyways, this Thing registers for some ticks
    // and spawns some meteors. When it's done, it deletes itself.
    public class Thing_MeteorSpawner : ThingWithComponents
    {
        private const float FogClearRadius = 4.5f;
        private const int numMaxEvents = 30; // Maximum number of events to tick before suicide
        private const int numMinEvents = 20; // Minimum number of events to tick
        private const int numMaxMeteors = 7; // Maximum number of meteors per event
        private const int numMinMeteors = 3; // Minimum number of meteors per event
        private int numMeteorEventCount; // Counter of how many meteor events are left to run before suicide
        private System.Random random = new System.Random();

        // The Spawn event sets up random variables about the impending event
        public override void SpawnSetup()
        {
            numMeteorEventCount = random.Next(numMinEvents, numMaxEvents); // number of meteor events
            base.SpawnSetup(); // Call the SpawnSetup method in "ThingWithComponents" to do any further SpawnSetup required
        }

        public override void TickRare()
        {
            // 33% chance a tick actually causes a meteor event
            if (random.Next(2) == 1) { DoMeteorEvent(); }
        }

        // This event generates a meteor impacting event. If the meteor incident runs out of
        // impactors, this event is also responsible for suiciding the invisible Thing
        // and cleaning up
        private void DoMeteorEvent()
        {
            // If we're done, suicide and clean up
            numMeteorEventCount--;
            if (numMeteorEventCount < 0)
            {
                Find.History.AddGameEvent("No further meteors detected.", GameEventType.Good, true, string.Empty);
                Destroy();
                return;
            }

            // Here comes the meteors!

            // Number of meteors this event
            int numMeteorsThisEvent = random.Next(numMinMeteors, numMaxMeteors);

            // Pick a random location with certain properties:
            IntVec3 intVec = GenCellFinder.RandomCellWith((IntVec3 sq) =>
                    sq.Standable()           // Can stand on this square
                        //&& !Find.RoofGrid.Roofed(sq) // Doesn't have a roof
                && !sq.Fogged()            // Not in the fog
                );

            // This is a list of Things that can be spawned. The chance of a particular boulder spawning
            // is encoded by how many times it's in the list
            string[] meteorArray = new string[]
            {
                "StoneBoulder",
                "StoneBoulder",
                "StoneBoulder",
                "StoneBoulder",
                "MineralBoulder",
                "MineralBoulder",
                "MineralBoulder",
                "SilverBoulder",
                "SilverBoulder",
                "UraniumBoulder"
            };

            // Choose a thing from the list and get its definition based on its name
            ThingDef thingDef = ThingDef.Named(meteorArray[random.Next(meteorArray.Length)]);

            // List for spawning multiple things during a single event
            List<Thing> list = new List<Thing>();

            // Make a number of copies of the selected thing, put it in the list
            while (list.Count < numMeteorsThisEvent)
            {
                Thing thing = ThingMaker.MakeThing(thingDef);
                list.Add(thing); // Add the thing!
            }

            // Spawn the list of things near the position chosen earlier
            MeteorUtility.DropThingsNear(intVec, list, 1, true, false);
        }
    }
}