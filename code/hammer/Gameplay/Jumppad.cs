﻿
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_jumppad" )]
[Display( Name = "Jump Pad", GroupName = "Platformer", Description = "A pad that launches players toward a target entity" ), Category( "Gameplay" ), Icon( "sports_gymnastics" )]
[Hammer.AutoApplyMaterial( "materials/editor/jumppad/jumppad.vmat" )]
[Hammer.Line( "targetname", "targetentity" )]
public partial class Jumppad : BaseTrigger
{
	[Net, Property, FGDType( "target_destination" )] public string TargetEntity { get; set; } = "";
	[Net, Property] public float VerticalBoost { get; set; } = 200f;
	[Net, Property] public float Force { get; set; } = 1000f;

	public override void Spawn()
	{
		if ( Force == 0f )
		{
			Force = 1000f;
		}

		base.Spawn();
	}

	public override void Touch( Entity other )
	{
		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;
		var target = FindByName( TargetEntity );

		if ( target.IsValid() )
		{
			var direction = (target.Position - other.Position).Normal;
			pl.ApplyForce( new Vector3( 0f, 0f, VerticalBoost ) );
			pl.ApplyForce( direction * Force );
		}

		base.Touch( other );
	}
}
