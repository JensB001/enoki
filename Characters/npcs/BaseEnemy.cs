using Godot;
using System;

public partial class BaseEnemy : CharacterBody2D
{
	[Export] MovementComponent Movement { get; set; }
	[Export] public Player Target { get; set; }

	public int Speed { get; set; } = 500; // How fast the player will move (pixels/sec).

	public AnimatedSprite2D animatedSprite2D;

	public override void _Ready()
	{
		Movement ??= new MovementComponent();
		AddChild(Movement);
		Movement.Body = this;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Movement.Move(GlobalPosition.DirectionTo(Target.GlobalPosition), Speed);

		if (Movement.Body.Velocity.X != 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.FlipV = false;
			// See the note below about the following boolean assignment.
			animatedSprite2D.FlipH = Movement.Body.Velocity.X > 0;
		}
		else if (Movement.Body.Velocity.Y != 0)
		{
			animatedSprite2D.Animation = "default";
		}
	}
}
