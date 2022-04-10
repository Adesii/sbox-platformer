﻿using Sandbox;
using Hammer;
using Sandbox.UI;

namespace Platformer;

/// <summary>
/// When the player is inside the trigger it will display the location on the hud. It will fall back to the map name.
/// </summary>
[Library( "plat_buttontrigger", Description = "When the player is inside the trigger it will display the location on the hud." )]
[Hammer.AutoApplyMaterial( "materials/editor/areatrigger/areatrigger.vmat" )]
[EntityTool( "Button Trigger", "Platformer", "Button Trigger." )]
internal partial class ButtonTrigger : BaseTrigger
{

	public override void Spawn()
	{
		base.Spawn();


		EnableTouchPersists = true;

	}
	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;



	}
	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !other.IsServer ) return;
		if(other is PropCarriable prop)
		{
			Log.Info( "Heyya" );
			_ = OnPressed.Fire( this );
		}

	}

	/// <summary>
	/// Fired when the door starts to close. This can be called multiple times during a single "door closing"
	/// </summary>
	protected Output OnPressed { get; set; }

	/// <summary>
	/// Fired when the door starts to close. This can be called multiple times during a single "door closing"
	/// </summary>
	protected Output UnPressed { get; set; }

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( !other.IsServer ) return;

		if ( other is PropCarriable prop )
		{
			Log.Info( "Heyya" );
			_ = UnPressed.Fire( this );
		}

	}

}