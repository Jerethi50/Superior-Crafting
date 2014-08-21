using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace SuperiorCrafting
{
    public class Building_ShootingRange : Building
    {
        private int fireDelay = 300;
        private string skillDefName = "Shooting";
        private int skillIncrease = 25;
        public CompMannable WhoIsManningMe;
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            WhoIsManningMe = base.GetComp<CompMannable>();
        }
        public override void Tick()
        {
            base.Tick();
            if (this.WhoIsManningMe.MannedNow)
            {
                this.fireDelay--;
                Pawn pawn = this.WhoIsManningMe.ManningPawn;
                if (this.fireDelay <= 0)
                {
                    this.fireDelay = 300;
                    MoteMaker.ThrowDustPuff(pawn.Position, 1);
                    foreach (SkillRecord current in pawn.skills.skills)
                    {
                        if (current.def.defName == this.skillDefName)
                        {
                            current.Learn(skillIncrease);
                            break;
                        }
                    }
                }
            }
        }
    }
}
