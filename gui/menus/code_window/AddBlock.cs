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
		addBlockButton.FocusMode = Control.FocusModeEnum.None;
		addBlockButton.Pressed += OnAddBlockButtonPressed;
	}

	private void OnAddBlockButtonPressed() {
		GetNode<BlockCanvas>("CodeWindowContainer/VBoxContainer/BlockCanvas").Spawn(BlockFactory.CreateRandom());
	}
}
