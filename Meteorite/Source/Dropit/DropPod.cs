using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using VerseBase;

namespace RimWorld{
public class DropPod : Thing
{
	//Working vars
	public int age = 0;
	public DropPodInfo info = null;

	//Constants
	private static readonly SoundDef OpenSound = SoundDef.Named("DropPodOpen");


	//Properties
	public override Mesh DrawMesh
	{
		get
		{
			//A special case to solve the common z-fighting with drop pods
			//Kind of hacky to special-case this here
			return MeshPool.plane30Twist;
		}
	}


	public override void ExposeData()
	{
		base.ExposeData();

		Scribe_Values.LookValue(ref age, "age");
		Scribe_Deep.LookDeep(ref info, "info");	
	}

	public override void Tick()
	{
		age++;
		if( age > info.openDelay )
		{
			PodOpen();	
		}
	}

	public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
	{
		foreach( Thing t in info.containedThings )
		{
			t.Destroy( DestroyMode.Vanish );
		}

		base.Destroy(mode);

		if( mode == DestroyMode.Kill )
		{
			//Always leave slag on killed
			for(int i=0; i<1; i++ )
			{
				Thing chunk = ThingMaker.MakeThing( ThingDef.Named("ChunkSlag") );
				GenPlace.TryPlaceThing( chunk, Position, ThingPlaceMode.Near );
			}
		}
	}
	
	private void PodOpen()
	{
		foreach( Thing t in info.containedThings )
		{
			GenPlace.TryPlaceThing( t, Position, ThingPlaceMode.Near );
		}
		info.containedThings.Clear();

		if( info.leaveSlag )
		{
			for(int i=0; i<1; i++ )
			{
				Thing chunk = ThingMaker.MakeThing( ThingDef.Named("ChunkSlag") );
				GenPlace.TryPlaceThing( chunk, Position, ThingPlaceMode.Near );
			}
		}

		OpenSound.PlayOneShot(Position);

		Destroy();

	}
}}


