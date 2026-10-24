using System;
using Godot;

namespace Enoki.gui.menus.code_window;

public partial class AddBlock : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var addBlockButton = GetNode<Button>("AddBlock");
		addBlockButton.Pressed += OnAddBlockButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnAddBlockButtonPressed()
	{
		var block = new PanelContainer();
		var random = new Random();
		
		var style = new StyleBoxFlat();
		style.BgColor = new Color(random.NextSingle(), random.NextSingle(), random.NextSingle());
		style.SetCornerRadiusAll(6);
		style.SetContentMarginAll(8);
		block.AddThemeStyleboxOverride("panel", style);

		var label = new Label();
		label.Text = "Fire";
		block.AddChild(label);
		
		GetNode<VBoxContainer>("CodeWindowContainer/VBoxContainer").AddChild(block);
	}
}
