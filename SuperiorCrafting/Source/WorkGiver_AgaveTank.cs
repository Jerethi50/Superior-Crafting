using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
    public class WorkGiver_AgaveTank : WorkGiver
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);
            }
        }
        public WorkGiver_AgaveTank(WorkGiverDef giverDef)
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
            if (pawn.health == pawn.healthTracker.MaxHealth)
            {
                return null;
            }
            if (!pawn.CanReserve(t, ReservationType.Use))
            {
                return null;
            }
            if (!t.def.defName.Equals("AgaveTank"))
            {
                return null;
            }
            return new Job(JobDefOf.ManThing, new TargetPack(t));
        }

    }
}
