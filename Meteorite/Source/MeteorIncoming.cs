using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using VerseBase;



namespace RimWorld{
    public static class ThingDefOfTwo
    {
        public static ThingDef Meteor;
        public static void RebindDefs()
        {
            DefOfHelper.BindDefsFor<ThingDef>(typeof(ThingDefOf));
        }
    }
    public class MeteorIncoming : Building
{
	//Config
	public MeteorInfo contents = null;
	
	//Working vars
	protected int ticksToImpact = MinTicksToImpact;
	private bool soundPlayed = false;
    private int animation = 30;
    private static Material frame1 = MaterialPool.MatFrom("Things/Buildings/Meteor1");
    private static Material frame2 = MaterialPool.MatFrom("Things/Buildings/Meteor2");
    private static Material frame3 = MaterialPool.MatFrom("Things/Buildings/Meteor3");

	//Constants
	protected const int MinTicksToImpact = 120;
	protected const int MaxTicksToImpact = 200;
	private const int SoundAnticipationTicks = 100;
    private static readonly SoundDef LandSound = SoundDef.Named("MortarRound_PreImpact");
	private static readonly Material ShadowMat = MaterialPool.MatFrom("Things/Special/DropPodShadow", MatBases.Transparent);
	
	public override void SpawnSetup()
	{
		base.SpawnSetup();
		ticksToImpact = Random.Range(MinTicksToImpact,MaxTicksToImpact); 
	}

	public override void ExposeData()
	{
		base.ExposeData();

		Scribe_Values.LookValue( ref ticksToImpact, "ticksToImpact");
		Scribe_Deep.LookDeep(ref contents, "contents");
	}
	
	public override void Tick()
	{

        if (this.animation > 0)
        {
            this.animation--;
            if (this.animation == 20)
            {
                Find.MapDrawer.MapChanged(base.Position, MapChangeType.Things);
            }
            else
            {
                if (this.animation == 10)
                {
                    Find.MapDrawer.MapChanged(base.Position, MapChangeType.Things);
                }
            }
        }
        else
        {
            this.animation = 30;
            Find.MapDrawer.MapChanged(base.Position, MapChangeType.Things);
        }
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

    public override Material DrawMat(IntRot rot)
    {
        if (this.animation > 20)
        {
            return MeteorIncoming.frame1;
        }
        if (this.animation < 10)
        {
            return MeteorIncoming.frame3;
        }
        return MeteorIncoming.frame2;
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
		Meteor newPod = (Meteor)ThingMaker.MakeThing( ThingDef.Named("Meteor"));
		newPod.info = contents;
		GenSpawn.Spawn(newPod, Position, rotation);

		//Remove me
		Destroy();
	}
}}
