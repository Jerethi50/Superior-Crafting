using System;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace RimWorld
{
    public class IncidentWorker_BoulderHit : IncidentWorker
	{
        private const float FogClearRadius = 4.5f;
		public override bool TryExecute(IncidentParms parms)
		{
			string[] array = new string[]
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
			System.Random random = new System.Random();
			int num = random.Next(array.Length);
			ThingDef thingDef = ThingDef.Named(array[num]);
            Thing thing = ThingMaker.MakeThing(thingDef);
            IntVec3 intVec = GenCellFinder.RandomCellWith((IntVec3 sq) => sq.Standable() && !Find.RoofGrid.Roofed(sq) && !sq.Fogged());
            MeteorUtility.MakeMeteorAt(intVec, new MeteorInfo
            {
                SingleContainedThing = thing,
                openDelay = 1,
                leaveSlag = false
            });
            Find.History.AddGameEvent("A meteoroid has entered the planets gravity well and come crashing down in a fiery explosion! weeee", GameEventType.BadNonUrgent, true, intVec, string.Empty);
			return true;
		}
	}
}
