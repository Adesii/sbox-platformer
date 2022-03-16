﻿
using Hammer;
using Sandbox;

namespace Platformer;

[Library( "plat_lifepickup", Description = "Addition Life" )]
[Model( Model = "models/gameplay/temp/temp_heart_01.vmdl" )]
[EntityTool( "Life Pickup", "Platformer", "Addition Life." )]
internal partial class LifePickup : BaseCollectible
{

	[Net, Property]
	public int NumberOfLife { get; set; }

	protected override bool OnCollected( PlatformerPawn pl )
	{
		pl.NumberLife++;
		pl.PickedUpItem( Color.Orange );

		Particles.Create( "particles/gameplay/player/lifepickup/lifepickup.vpcf", pl );

		return true;
	}

	protected override void OnCollectedEffect()
	{
		base.OnCollectedEffect();

		Sound.FromEntity( "life.pickup", this );
	}

}
