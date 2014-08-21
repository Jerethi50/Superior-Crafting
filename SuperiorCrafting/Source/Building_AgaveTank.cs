using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;

namespace SuperiorCrafting
{
    public class Building_AgaveTank : Building
    {
        private int healDelay = 180;
        public CompPowerTrader powerComp;
        public CompMannable WhoIsManningMe;

        public bool CanHealNow
        {
            get
            {
                return this.powerComp.PowerOn && this.HasAgaveGelInHopper;
            }
        }

        public bool Powered
        {
            get
            {
                return this.powerComp.PowerOn;
            }
        }

        public bool HasAgaveGelInHopper
        {
            get
            {
                return this.AgaveGelInHopper != null;
            }
        }

        public Building AnyAdjacentHopper
        {
            get
            {
                ThingDef thingDef = ThingDef.Named("Hopper");
                foreach (IntVec3 current in GenAdj.AdjacentSquaresCardinal(this))
                {
                    Building building = Find.BuildingGrid.BuildingAt(current);
                    if (building != null && building.def == thingDef)
                    {
                        return (Building_Storage)building;
                    }
                }
                return null;
            }
        }
        private Thing AgaveGelInHopper
        {
            get
            {
                ThingDef thingdef = ThingDef.Named("Medicine");
                ThingDef thingdef2 = ThingDef.Named("Hopper");
                foreach (IntVec3 current in GenAdj.AdjacentSquaresCardinal(this))
                {
                    Thing thing = null;
                    Thing thing2 = null;
                    foreach (Thing current2 in Find.ThingGrid.ThingsAt(current))
                    {
                        if (current2.def == thingdef)
                        {
                            thing = current2;
                        }
                        if (current2.def == thingdef2)
                        {
                            thing2 = current2;
                        }
                    }
                    if (thing != null && thing2 != null)
                    {
                        return thing;
                    }

                }
                return null;
            }
        }
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            powerComp = base.GetComp<CompPowerTrader>();
            WhoIsManningMe = base.GetComp<CompMannable>();
        }
        public override void Tick()
        {
            base.Tick();
            if (this.CanHealNow && this.WhoIsManningMe.MannedNow)
            {
                this.healDelay--;
                Pawn pawn = this.WhoIsManningMe.ManningPawn;
                if (this.healDelay <= 0)
                {
                    this.healDelay = 180;
                    if (pawn.healthTracker.Wounded == true)
                    {
                        pawn.TakeDamage(new DamageInfo(DamageTypeDefOf.Healing, 10, this));
                        MoteMaker.ThrowHealingCross(pawn.Position);
                        int num = 0;
                        List<ThingDef> list = new List<ThingDef>();
                        Thing AgaveGelInHopper = this.AgaveGelInHopper;
                        int num2 = Mathf.Min(AgaveGelInHopper.stackCount, 1);
                        num += num2;
                        list.Add(AgaveGelInHopper.def);
                        AgaveGelInHopper.SplitOff(num2);
                        AgaveGelInHopper = this.AgaveGelInHopper;

                    }
                }
            }
        }
    }
}