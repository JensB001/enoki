using System;
using Enoki.gui.menus.code_window.blocks;
using Godot;

namespace Enoki.gui.menus.code_window;

public partial class CanvasScript : CanvasLayer
{
	public override void _Ready()
	{
		var addBlockButton = GetNode<Button>("AddBlock");
		addBlockButton.FocusMode = Control.FocusModeEnum.None;
		addBlockButton.Pressed += OnAddBlockButtonPressed;
		
		var mainMenuButton = GetNode<Button>("MainMenuButton");
		mainMenuButton.FocusMode = Control.FocusModeEnum.None;
		mainMenuButton.Pressed += OnMainMenuButtonPressed;
	}

	private void OnAddBlockButtonPressed() {
		GetNode<BlockCanvas>("CodeWindowContainer/VBoxContainer/BlockCanvas").Spawn(BlockFactory.CreateRandom());
	}

	private void OnMainMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("gui/menus/main_menu/main_menu.tscn");
	}
}
