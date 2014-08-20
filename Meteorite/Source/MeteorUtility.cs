using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;


namespace RimWorld{
public class MeteorInfo : Saveable
{
	public List<Thing>	containedThings = new List<Thing>();
	public int			openDelay = DefaultOpenDelay;
	public bool			leaveSlag = false;

	//Constants
	public const int DefaultOpenDelay = 110;

	//Properties
	public Thing SingleContainedThing
	{
		get
		{
			if( containedThings.Count == 0 )
				return null;
			if( containedThings.Count > 1 )
				Log.Error("ContainedThing used on a DropPodInfo holding > 1 thing.");
			return containedThings[0];
		}
		set
		{
			containedThings.Clear();
			containedThings.Add(value);
		}
	}


	public MeteorInfo()
	{
		//Parameterless ctor for save/load
	}
	
	public void ExposeData()
	{
		Scribe_Collections.LookList(ref containedThings, "containedThings", LookMode.Deep);
		Scribe_Values.LookValue(ref openDelay, "openDelay", DefaultOpenDelay);
		Scribe_Values.LookValue(ref leaveSlag, "leaveSlag", false);
	}
}




public static class MeteorUtility
{
	public static void MakeMeteorAt( IntVec3 loc, MeteorInfo info )
	{
		MeteorIncoming inc = (MeteorIncoming)ThingMaker.MakeThing( ThingDef.Named("MeteorIncoming") );
		inc.contents = info;
		GenSpawn.Spawn(inc, loc );
	}

	private static List<List<Thing>> tempList = new List<List<Thing>>();
	public static void DropThingsNear( IntVec3 dropCenter, IEnumerable<Thing> things, int openDelay = MeteorInfo.DefaultOpenDelay, bool canInstaDropDuringInit = true, bool leaveSlag = false )
	{
		foreach( var t in things )
		{
			List<Thing> l = new List<Thing>();
			l.Add(t);
			tempList.Add( l );
		}

		DropThingGroupsNear( dropCenter, tempList, openDelay, canInstaDropDuringInit, leaveSlag );
		tempList.Clear();
	}


	public static void DropThingGroupsNear( IntVec3 dropCenter, List<List<Thing>> thingsGroups, int openDelay = MeteorInfo.DefaultOpenDelay, bool canInstaDropDuringInit = true, bool leaveSlag = false )
	{
		IntVec3 dropSpot;
		foreach( var group in thingsGroups )
		{
			if( !RCellFinder.TryFindDropPodSpotNear( dropCenter, out dropSpot ) )
			{
				Log.Warning("DropThingsNear failed to find a place to drop " + group.FirstOrDefault() + " near " + dropCenter + ". Dropping on random square instead." );
				dropSpot = GenCellFinder.RandomCellWith( sq=>sq.Walkable() );
			}

			//Forbid all the things if possible
			foreach( Thing t in group )
			{
				ThingWithComponents tComp = t as ThingWithComponents;
				if( tComp != null && tComp.GetComp<CompForbiddable>() != null )
					tComp.GetComp<CompForbiddable>().forbidden = true;
			}

			if( canInstaDropDuringInit && Find.TickManager.tickCount < 2 )
			{
				//Dropping before game start: just insta-spawn
				foreach( Thing t in group )
				{
					GenPlace.TryPlaceThing(t, dropSpot, ThingPlaceMode.Near);
				}
			}
			else
			{
				MeteorInfo podInfo = new MeteorInfo();	

				foreach( Thing t in group )
				{
					podInfo.containedThings.Add(t);
				}

				podInfo.openDelay = openDelay; 
				podInfo.leaveSlag = leaveSlag;
                dropSpot = GenCellFinder.RandomCellWith(sq => sq.Walkable());
				MeteorUtility.MakeMeteorAt( dropSpot, podInfo );	
			}
		}
	}



}

}