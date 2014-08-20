using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using VerseBase;



namespace RimWorld{
public class DropPodIncoming : Thing
{
	//Config
	public DropPodInfo contents = null;
	
	//Working vars
	protected int ticksToImpact = MinTicksToImpact;
	private bool soundPlayed = false;

	//Constants
	protected const int MinTicksToImpact = 120;
	protected const int MaxTicksToImpact = 200;
	private const int SoundAnticipationTicks = 100;
	private static readonly SoundDef LandSound = SoundDef.Named("DropPodFall");
	private static readonly Material ShadowMat = MaterialPool.MatFrom("Things/Special/DropPodShadow", MatBases.Transparent);

	
	public override void SpawnSetup()
	{
		base.SpawnSetup();
		
		ticksToImpact = Rand.RangeInclusive(MinTicksToImpact,MaxTicksToImpact); 

		if( Find.RoofGrid.Roofed(Position) )
			Log.Warning("Drop pod dropped on roof at " + Position);
	}

	public override void ExposeData()
	{
		base.ExposeData();

		Scribe_Values.LookValue( ref ticksToImpact, "ticksToImpact");
		Scribe_Deep.LookDeep(ref contents, "contents");
	}
	
	public override void Tick()
	{
		ticksToImpact--;
		
		if( ticksToImpact <= 0 )
		{
			PodImpact();	
		}

		
		if( !soundPlayed && ticksToImpact  <SoundAnticipationTicks )
		{
			soundPlayed = true;
			LandSound.PlayOneShot(Position);
		}
	}

	public override void Draw()
	{
		Vector3 podPos = Position.ToVector3ShiftedWithAltitude(AltitudeLayer.FlyingItem);


		float podDist = ticksToImpact*ticksToImpact * 0.01f;  //Squaring it makes the pod slow down gently

		podPos.x -= podDist * 0.4f;
		podPos.z += podDist * 0.6f;

		DrawAt( podPos );

		//Do target indicator
		{
			float scale = 2.0f + ticksToImpact / 100f;
			Vector3 scaler = new Vector3( scale, 1, scale);
			Matrix4x4 Matrix = new Matrix4x4();
			Vector3 drawLoc = DrawPos;
			drawLoc.y = Altitudes.AltitudeFor( AltitudeLayer.Shadows);

			Matrix.SetTRS( drawLoc, rotation.AsQuat, scaler);

			Graphics.DrawMesh(MeshPool.plane10Back, Matrix, ShadowMat, 0);
		}


	}

	private void PodImpact()
	{
		//Make dust puff
		for(int i=0; i<6; i++ )
		{
			Vector3 dustLoc = Position.ToVector3Shifted() + Gen.RandomHorizontalVector(1f);
			MoteMaker.ThrowDustPuff(dustLoc, 1.2f);
		}
		MoteMaker.ThrowLightningGlow(Position.ToVector3Shifted(), 2f);

		
		//Create the landed pod	
		DropPod newPod = (DropPod)ThingMaker.MakeThing( ThingDefOf.DropPod );
		newPod.info = contents;
		GenSpawn.Spawn(newPod, Position, rotation);
		
		
		//Remove me
		Destroy();
	}
}}
