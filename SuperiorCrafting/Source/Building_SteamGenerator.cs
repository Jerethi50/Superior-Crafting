using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace SuperiorCrafting
{
    public class Building_SteamGenerator : Building
    {
        private int burnDelay;
        public CompPowerTrader powerComp;
        public CompGlower glowerComp;

        public bool IsFlareActive
        {
            get
            {
                return !Find.MapConditionManager.ConditionIsActive(MapConditionDefOf.SolarFlare);
            }
        }

        public bool CanBurnNow
        {
            get
            {
                return this.powerComp.PowerOn && this.HasWoodLogInHopper && IsFlareActive;
            }
        }

        public bool Powered
        {
            get
            {
                return this.powerComp.PowerOn;
            }
        }

        public bool HasWoodLogInHopper
        {
            get
            {
                return this.WoodLogInHopper != null;
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
        private Thing WoodLogInHopper
        {
            get
            {
                ThingDef thingdef = ThingDef.Named("WoodLog");
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
            glowerComp = base.GetComp<CompGlower>();
            glowerComp.Lit = false;
        }
        public override void Tick()
        {
            base.Tick();
            this.burnDelay--;
            if (!this.CanBurnNow)
            {
                glowerComp.Lit = false;
                this.powerComp.powerOutput = 0f;
                burnDelay = 0;
            }
            else
            if (this.burnDelay <= 0)
            {
                glowerComp.Lit = true;
                int num = 0;
                List<ThingDef> list = new List<ThingDef>();
                Thing WoodLogInHopper = this.WoodLogInHopper;
                int num2 = Mathf.Min(WoodLogInHopper.stackCount, 1);
                num += num2;
                list.Add(WoodLogInHopper.def);
                WoodLogInHopper.SplitOff(num2);
                WoodLogInHopper = this.WoodLogInHopper;
                this.powerComp.powerOutput = 1000f;
                this.burnDelay = 3600;
                MoteMaker.ThrowDustPuff(this.Position, 2);
                }
            }
        }
    }
