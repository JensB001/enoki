using Godot;
using System;

public partial class Entity : CharacterBody2D
{
	public int health;
	public int movement_speed;
	private Vector2 velocity = Vector2.Zero;

	public bool can_move = true;
	public bool can_attack = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
