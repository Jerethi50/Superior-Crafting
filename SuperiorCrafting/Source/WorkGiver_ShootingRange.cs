using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
    public class WorkGiver_ShootingRange : WorkGiver
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);
            }
        }
        public WorkGiver_ShootingRange(WorkGiverDef giverDef)
            : base(giverDef)
        {
        }
        public override Job StartingJobForOn(Pawn pawn, Thing t)
        {
            if (t.Faction != pawn.Faction)
            {
                return null;
            }
            if (pawn.food.Food.Starving)
            {
                return null;
            }            
            if (!pawn.CanReserve(t, ReservationType.Use))
            {
                return null;
            }            
            if (!t.def.defName.Equals("ShootingRange"))
            {
                return null;
            }
            return new Job(JobDefOf.ManThing, new TargetPack(t));
        }

    }
}