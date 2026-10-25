using Godot;
using System;

[GlobalClass]
public partial class MovementComponent : Node
{
	[Export] public CharacterBody2D Body { get; set; }

	public void Move(Vector2 velocity, int speed)
	{
		Body.Velocity = velocity * speed;
		Body.MoveAndSlide();
	}
}
