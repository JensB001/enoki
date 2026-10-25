using System;
using Enoki.gui.menus.code_window.blocks;
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
		var attributes = Enum.GetValues<AttributeType>();

		Block block = Random.Shared.Next(4) switch
		{
			0 => new NumberBlock { Value = Random.Shared.Next(1, 6) },
			1 => new AttributeBlock { Value = attributes[Random.Shared.Next(attributes.Length)] },
			2 => new AdderBlock(),
			_ => new MultiplierBlock(),
		};

		GetNode<BlockCanvas>("CodeWindowContainer/VBoxContainer/BlockCanvas").Spawn(block);
	}
}
