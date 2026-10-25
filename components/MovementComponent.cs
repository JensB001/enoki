using Godot;
using System;

[GlobalClass]
public partial class MovementComponent : Node
{
	[Export] public CharacterBody2D Body { get; set; }

	/// <summary>How fast knockback wears off (pixels/sec²).</summary>
	[Export] public float KnockbackDecay { get; set; } = 900;

	public Vector2 Knockback { get; private set; }

	public void Move(Vector2 velocity, int speed)
	{
		Body.Velocity = velocity * speed + Knockback;
		Body.MoveAndSlide();

		Knockback = Knockback.MoveToward(Vector2.Zero, KnockbackDecay * (float)GetPhysicsProcessDeltaTime());
	}

	public void ApplyKnockback(Vector2 impulse)
	{
		Knockback += impulse;
	}
}
