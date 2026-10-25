using Godot;
using System;

[GlobalClass]
public partial class HitBox : Area2D
{
	[Export] public int Damage {get; set;} = 10;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
