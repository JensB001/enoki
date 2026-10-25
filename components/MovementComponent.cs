using Godot;
using System;

[GlobalClass]
public partial class MovementComponent : Node
{
	[Export] public CharacterBody2D Body { get; set; }

	/// <summary>How quickly knockback dies out (higher = shorter, snappier). Distance travelled ≈ impulse / friction.</summary>
	[Export] public float KnockbackFriction { get; set; } = 12;

	/// <summary>Cap so many bullets at once can't launch something across the map.</summary>
	[Export] public float MaxKnockback { get; set; } = 320;

	public Vector2 Knockback { get; private set; }

	public void Move(Vector2 velocity, int speed)
	{
		Body.Velocity = velocity * speed + Knockback;
		Body.MoveAndSlide();

		// Exponential falloff: a hard shove that eases out instead of stopping dead.
		float dt = (float)GetPhysicsProcessDeltaTime();
		Knockback *= Mathf.Exp(-KnockbackFriction * dt);
		if (Knockback.LengthSquared() < 1) Knockback = Vector2.Zero;
	}

	public void ApplyKnockback(Vector2 impulse)
	{
		Knockback = (Knockback + impulse).LimitLength(MaxKnockback);
	}
}
