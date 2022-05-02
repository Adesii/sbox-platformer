﻿
using Hammer;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Platformer;

[Library( "plat_key", Description = "Key Pickup" )]
[Hammer.EditorModel( "models/editor/collectables/collectables.vmdl", FixedBounds = true )]
[Display( Name = "Key Pickup", GroupName = "Platformer", Description = "Key Pickup" ), Category( "Gameplay" ), Icon( "vpn_key" )]
internal partial class KeyPickup : AnimEntity
{
	public enum ModelType
	{
		FoamFinger,
		Ball
	}

	/// <summary>
	/// This will set the model and icon for the HUD.
	/// </summary>
	[Property( "model_type", Title = "Model Type" ), Net]
	public ModelType ModelTypeList { get; set; } = ModelType.FoamFinger;

	[Net]
	public string KeyIcon { get; set; }

	[Property]
	[Net]
	public int KeyNumber { get; set; } = 1;

	public override void Spawn()
	{
		base.Spawn();
		
		if ( ModelTypeList == ModelType.FoamFinger )
		{
			SetModel( "models/citizen_props/foamhand.vmdl" );
			KeyIcon = ("ui/hud/collectables/Collect_FoamHand.png");
		}
		if ( ModelTypeList == ModelType.Ball )
		{
			SetModel( "models/citizen_props/beachball.vmdl" );
			KeyIcon = ("ui/hud/collectables/Collect_BeachBall.png");
		}

		Transmit = TransmitType.Always;

		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;
		EnableAllCollisions = true;


	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not PlatformerPawn pl ) return;

		if ( Platformer.CurrentGameMode == Platformer.GameModes.Competitive )
		{
			if ( pl.KeysPlayerHas.Contains( KeyNumber ) ) return;
			pl.PickedUpItem( Color.Yellow );

			pl.KeysPlayerHas.Add( KeyNumber );
			pl.NumberOfKeys++;

			CollectedHealthPickup( To.Single( other.Client ) );
		}
		
		if ( Platformer.CurrentGameMode == Platformer.GameModes.Coop )
		{
			if ( Platformer.KeysAllPlayerHas.Contains( KeyNumber ) ) return;
			Platformer.KeysAllPlayerHas.Add( KeyNumber );
			Platformer.NumberOfKeys++;

			Log.Info( Platformer.NumberOfKeys );

			CollectedHealthPickup( To.Single( other.Client ) );
		}
	}

	[ClientRpc]
	private void CollectedHealthPickup()
	{
		Sound.FromEntity( "life.pickup", this );
		Particles.Create( "particles/gameplay/player/collectpickup/collectpickup.vpcf", this );

	}

	[Event.Tick.Server]
	public void Tick()
	{
		Rotation = Rotation.FromYaw( Rotation.Yaw() + 500 * Time.Delta );
	}

	[Event.Tick.Client]
	private void ClientTick()
	{
		if ( Platformer.CurrentGameMode == Platformer.GameModes.Competitive )
		{
			var a = ShouldRender() ? 1 : 0;
			RenderColor = RenderColor.WithAlpha( a );
		}
		if ( Platformer.CurrentGameMode == Platformer.GameModes.Coop )
		{
			var b = ShouldRenderAll() ? 1 : 0;
			RenderColor = RenderColor.WithAlpha( b );
		}
	}

	private bool ShouldRender()
	{
		if ( !Local.Pawn.IsValid() ) return true;
		if ( Local.Pawn is not PlatformerPawn pl ) return true;

		return !pl.KeysPlayerHas.Contains( KeyNumber );
	}

	private bool ShouldRenderAll()
	{
		return !Platformer.KeysAllPlayerHas.Contains( KeyNumber );
	}
}
