using Godot;
using System;

public partial class CanvasLayer2 : CanvasLayer
{
	[Export] public ProgressBar HealthBar { get; set; }
	[Export] public Label ScoreLabel { get; set; }

	public void Bind(Player player)
	{
		GD.Print("HUD Bind Called");
		HealthBar.Target = player.Health;
	}
}
