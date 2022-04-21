﻿using Platformer;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace Platformer.UI
{
	public class LifeCurrent : Panel
	{

		public Label Number;

		public Image Image;

		private float _life;


		public LifeCurrent()
		{
			Image = Add.Image( "ui/hud/citizen/citizen.png", "playerimage" );
			Number = Add.Label( "", "number" );
		}

		public override void Tick()
		{

			var player = Local.Pawn;
			if ( player == null ) return;


			if ( Local.Pawn is not PlatformerPawn pl ) return;
			var life = pl.NumberLife;

			_life = _life.LerpTo( life, Time.Delta * 0 );

			Number.SetClass( "lifelow", life <= 1 );

			Number.Text = $"{life}";

			if ( life <= 1 )
			{
				LowHealth();
			}
			if ( life == 3 )
			{
				HighHealth();
			}

			if ( Platformer.CurrentGameMode != Platformer.GameModes.Tag )
			{
				Number.SetClass( "active", true );
			}
		}

		public void LowHealth()
		{
			Image.SetTexture( "ui/hud/citizen/citizen_low.png" );
		}
		public void HighHealth()
		{
			Image.SetTexture( "ui/hud/citizen/citizen.png" );
		}

	}
}
