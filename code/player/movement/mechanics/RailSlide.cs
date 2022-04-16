﻿
using System.Linq;
using Sandbox;

namespace Platformer.Movement;
internal class RailSlide : BaseMoveMechanic
{

	public override bool TakesOverControl => true;

	private GenericPathEntity Path;
	private int Node;
	private float Alpha;
	private TimeSince TimeSinceJump;
	private bool IsRailSliding;
	private bool DoOnce;
	private Sound railSlideSound;

	public RailSlide( PlatformerController controller ) : base( controller )
	{
	}

	protected override bool TryActivate()
	{

		if ( TimeSinceJump < .3f ) return false;

		foreach ( var path in Entity.All.OfType<RailPathEntity>() )
		{

			if ( path.PathNodes.Count < 2 ) continue;

			var nearestPoint = path.NearestPoint( ctrl.Position, out int node, out float t );
			if ( nearestPoint.Distance( ctrl.Position ) > 30 ) continue;

			Path = path;
			Node = node;
			Alpha = t;

			return true;
		}
		
		return false;
	}

	public override void PostSimulate()
	{
		base.PostSimulate();

		ctrl.GroundEntity = Path;
	}

	public override void Simulate()
	{
		base.Simulate();

		Alpha += Time.Delta;

		if ( Alpha >= 1 )
		{
			Alpha = 0;
			Node++;

			if ( Node >= Path.PathNodes.Count - 1 )
			{
				IsActive = false;
				Path = null;
				SlideStopped();
				DoOnce = false;

				TimeSinceJump = 0;
				IsActive = false;

				// todo: add velocity up from rail normal,
				// and fix getting grounded immediately so we don't have to set position
				ctrl.Velocity = ctrl.Velocity.WithZ( 320f );
				ctrl.Position = ctrl.Position.WithZ( ctrl.Position.z + 10 );

				return;
			}
		}

		var node = Path.PathNodes[Node];
		var nextNode = Path.PathNodes[Node + 1];
		var currentPosition = ctrl.Position;
		var nextPosition = Path.GetPointBetweenNodes( node, nextNode, Alpha );

		ctrl.Velocity = (nextPosition - currentPosition).Normal * 300f;
		ctrl.Position = nextPosition;
		ctrl.Rotation = Rotation.LookAt( ctrl.Velocity.Normal );
		ctrl.GroundEntity = Path;
		ctrl.SetTag( "skidding" );
		Particles.Create( "particles/gameplay/player/sliding/railsliding.vpcf", ctrl.Pawn );

		SlideEffect();
		IsRailSliding = true;

		if ( Input.Pressed( InputButton.Jump ) )
		{
			TimeSinceJump = 0;
			IsActive = false;

			// todo: add velocity up from rail normal,
			// and fix getting grounded immediately so we don't have to set position
			ctrl.Velocity = ctrl.Velocity.WithZ( 320f );
			ctrl.Position = ctrl.Position.WithZ( ctrl.Position.z + 10 );
			DoOnce = false;
			SlideStopped();
		}
	}

	private void SlideEffect()
	{

		if ( !ctrl.Pawn.IsServer ) return;

		if ( IsRailSliding && !DoOnce )
		{
			DoOnce = true;

			using var _ = Prediction.Off();
			ctrl.Pawn.PlaySound( "rail.slide.start" );
			railSlideSound.Stop();
			railSlideSound = ctrl.Pawn.PlaySound( "rail.slide.loop" );
		//	Sound.FromEntity( "rail.slide.loop", ctrl.Pawn );


		}
	}
	private void SlideStopped()
	{
		railSlideSound.Stop();
		ctrl.Pawn.PlaySound( "rail.slide.end" );
	}
}
