﻿using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Platformer;

[Library( "plat_tagspawn", Description = "Defines a checkpoint where the player will respawn after falling" )]
[Model( Model = "models/dev/playerstart_tint.vmdl" )]
[Display( Name = "Player Checkpoint", GroupName = "Platformer", Description = "Defines a checkpoint where the player will respawn after falling" )]
internal partial class TaggerSpawn : Entity
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

	}
}