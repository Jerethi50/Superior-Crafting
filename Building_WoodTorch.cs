using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ----------------------------------------------------------------------
// These are RimWorld-specific usings. Activate/Deactivate what you need:
// ----------------------------------------------------------------------

using UnityEngine;   // Always needed
//using VerseBase;   // Material/Graphics handling functions are found here
using RimWorld;      // RimWorld specific functions are found here (like 'Building_Battery')
using Verse;         // RimWorld universal objects are here (like 'Building')
//using Verse.AI;    // Needed when you do something with the AI

namespace SuperiorCrafting
{
    public class Building_WoodTorch : Building
    {
        private int burnDelay = 54000;
        private CompGlower glowerComp;
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            glowerComp = base.GetComp<CompGlower>();
            glowerComp.Lit = true;
        }
        public override void Tick()
        {
            base.Tick();
            if (this.burnDelay > 0)
            {
                this.burnDelay--;
            }
            else
            {
                this.TakeDamage(new DamageInfo(DamageTypeDefOf.Breakdown, 30, null));
            }
        }
    }
}