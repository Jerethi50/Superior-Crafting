using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using VerseBase;
namespace RimWorld
{
    public class PlantNight : Plant
    {
        private const int MinPlantYield = 2;
        private const float MaxAirPressureForDOT = 0.6f;
        private const float SuffocationMaxDOTPerTick = 0.01f;
        private const float GridPosRandomnessFactor = 0.3f;
        private const int TicksWithoutLightBeforeRot = 100000;
        private int ticksSinceLit;
        private List<int> posIndexList = new List<int>();
        private Color32[] workingColors = new Color32[4];
        private static readonly Material MatSowing = MaterialPool.MatFrom("Things/Plant/Plant_Sowing");
        public new bool HarvestableNow
        {
            get
            {
                return this.def.plant.Harvestable && this.growthPercent > this.def.plant.harvestMinGrowth;
            }
        }
        public override bool EdibleNow
        {
            get
            {
                return this.growthPercent > this.def.plant.harvestMinGrowth;
            }
        }
        private string GrowthPercentString
        {
            get
            {
                float num = this.growthPercent * 100f;
                if (num > 100f)
                {
                    num = 100.1f;
                }
                return num.ToString("##0");
            }
        }
        public override string LabelMouseover
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(this.def.label);
                stringBuilder.Append(" (" + "PercentGrowth".Translate(new object[]
				{
					this.GrowthPercentString
				}));
                if (this.Rotting)
                {
                    stringBuilder.Append(", " + "DyingLower".Translate());
                }
                stringBuilder.Append(")");
                return stringBuilder.ToString();
            }
        }
        private bool HasEnoughLightToGrow
        {
            get
            {
                return Find.GlowGrid.PsychGlowAt(base.Position) <= this.def.plant.growMinGlow;
            }
        }
        private float LocalFertility
        {
            get
            {
                return Find.FertilityGrid.FertilityAt(base.Position);
            }
        }
        public new bool GrowingNow
        {
            get
            {
                return this.LifeStage == PlantLifeStage.Growing && this.HasEnoughLightToGrow;
            }
        }
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            for (int i = 0; i < this.def.plant.maxMeshCount; i++)
            {
                this.posIndexList.Add(i);
            }
            this.posIndexList.Shuffle<int>();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<float>(ref this.growthPercent, "growthPercent", 0f, false);
            Scribe_Values.LookValue<int>(ref this.age, "age", 0, false);
            Scribe_Values.LookValue<int>(ref this.ticksSinceLit, "ticksSinceLit", 0, false);
        }
        public override float Eaten(float nutritionWanted, Pawn eater)
        {
            this.PlantCollected();
            return this.def.food.nutrition;
        }
        public override void TickRare()
        {
            if (!this.HasEnoughLightToGrow)
            {
                this.ticksSinceLit += 250;
            }
            else
            {
                this.ticksSinceLit = 0;
            }
            if (this.GrowingNow)
            {
                float num = this.LocalFertility * this.def.plant.fertilityFactorGrowthRate + (1f - this.def.plant.fertilityFactorGrowthRate);
                this.growthPercent += num * 250f * (this.def.plant.growthPer20kTicks / 20000f);
                if (this.LifeStage == PlantLifeStage.Mature && Find.Map.Biome.CommonalityOfPlant(this.def) == 0f)
                {
                    PlantNight.SoundHarvestReady.PlayOneShot(base.Position);
                }
            }
            if (this.def.plant.LimitedLifespan)
            {
                this.age += 250;
                if (this.Rotting)
                {
                    int amount = Mathf.CeilToInt(1.25f);
                    base.TakeDamage(new DamageInfo(DamageTypeDefOf.Rotting, amount, null, null, null));
                }
            }
            if (!base.Destroyed)
            {
                GenPlantReproduction.TickReproduceFrom(this);
            }
        }
        public override Material DrawMat(IntRot rot)
        {
            if (this.LifeStage == PlantLifeStage.Sowing)
            {
                return PlantNight.MatSowing;
            }
            return base.DrawMat(rot);
        }
        public override void PrintOnto(SectionLayer layer)
        {
            Vector3 a = this.TrueCenter();
            Rand.Seed = base.Position.GetHashCode();
            float num;
            if (this.def.plant.maxMeshCount == 1)
            {
                num = 0.05f;
            }
            else
            {
                num = 0.5f;
            }
            int num2 = Mathf.CeilToInt(this.growthPercent * (float)this.def.plant.maxMeshCount);
            if (num2 < 1)
            {
                num2 = 1;
            }
            int num3 = 1;
            int maxMeshCount = this.def.plant.maxMeshCount;
            switch (maxMeshCount)
            {
                case 1:
                    num3 = 1;
                    goto IL_F3;
                case 2:
                case 3:
                    num3 = 5;
                    goto IL_F3;
                case 4:
                    num3 = 2;
                    goto IL_F3;
            }
            goto IL_96;
        IL_96:
            if (maxMeshCount == 9)
            {
                num3 = 3;
                goto IL_F3;
            }
            if (maxMeshCount == 16)
            {
                num3 = 4;
                goto IL_F3;
            }
            if (maxMeshCount != 25)
            {
                Log.Error(this.def + " must have plant.MaxMeshCount that is a perfect square.");
                goto IL_F3;
            }
        IL_F3:
            float num4 = 1f / (float)num3;
            Vector3 vector = Vector3.zero;
            Vector2 zero = Vector2.zero;
            int num5 = 0;
            int count = this.posIndexList.Count;
            for (int i = 0; i < count; i++)
            {
                int num6 = this.posIndexList[i];
                float num7 = this.def.plant.visualSizeRange.LerpThroughRange(this.growthPercent);
                if (this.def.plant.maxMeshCount == 1)
                {
                    vector = a + new Vector3(Rand.Range(-num, num), 0f, Rand.Range(-num, num));
                    float num8 = Mathf.Floor(a.z);
                    if (vector.z - num7 / 2f < num8)
                    {
                        vector.z = num8 + num7 / 2f;
                    }
                }
                else
                {
                    vector = base.Position.ToVector3();
                    vector.y = this.def.altitude;
                    vector.x += 0.5f * num4;
                    vector.z += 0.5f * num4;
                    int num9 = num6 / num3;
                    int num10 = num6 % num3;
                    vector.x += (float)num9 * num4;
                    vector.z += (float)num10 * num4;
                    float num11 = num4 * 0.3f;
                    vector += new Vector3(Rand.Range(-num11, num11), 0f, Rand.Range(-num11, num11));
                }
                bool flag = Rand.Value < 0.5f;
                Material mat = this.def.folderDrawMats.RandomListElement<Material>();
                this.workingColors[1].a = (this.workingColors[2].a = (byte)(255f * this.def.plant.topWindExposure));
                this.workingColors[0].a = (this.workingColors[3].a = 0);
                if (this.def.overdraw)
                {
                    num7 += 2f;
                }
                zero = new Vector2(num7, num7);
                bool flipUv = flag;
                Printer_Plane.PrintPlane(layer, vector, zero, mat, 0f, flipUv, null, this.workingColors);
                num5++;
                if (num5 >= num2)
                {
                    break;
                }
            }
            if (this.def.sunShadowInfo != null)
            {
                float num12;
                if (zero.y < 1f)
                {
                    num12 = 0.6f;
                }
                else
                {
                    num12 = 0.81f;
                }
                Vector3 center = vector;
                center.z -= zero.y / 2f * num12;
                center.y -= 0.04f;
                Printer_Shadow.PrintShadow(layer, center, this.def.sunShadowInfo);
            }
        }
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("PercentGrowth".Translate(new object[]
			{
				this.GrowthPercentString
			}));
            if (this.LifeStage != PlantLifeStage.Sowing)
            {
                if (this.LifeStage == PlantLifeStage.Growing)
                {
                    if (!this.HasEnoughLightToGrow)
                    {
                        stringBuilder.AppendLine("NotGrowingNow".Translate(new object[]
						{
							this.def.plant.growMinGlow.HumanName().ToLower()
						}));
                    }
                    else
                    {
                        if (!this.GrowingNow)
                        {
                            stringBuilder.AppendLine("NotGrowingNowNight".Translate());
                        }
                        else
                        {
                            stringBuilder.AppendLine("Growing".Translate());
                        }
                    }
                    int numTicks = (int)((1f - this.growthPercent) * (20000f / this.def.plant.growthPer20kTicks));
                    stringBuilder.AppendLine("FullyGrownIn".Translate(new object[]
					{
						numTicks.TickstoDaysString()
					}));
                }
                else
                {
                    if (this.LifeStage == PlantLifeStage.Mature)
                    {
                        if (this.def.plant.Harvestable)
                        {
                            stringBuilder.AppendLine("ReadyToHarvest".Translate());
                        }
                        else
                        {
                            stringBuilder.AppendLine("Mature".Translate());
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            Find.DesignationManager.RemoveAllDesignationsOn(this);
        }
    }
}
