using System;
using Enoki.gui.menus.code_window.blocks;
using Godot;

namespace Enoki.gui.menus.code_window;

public partial class CanvasScript : CanvasLayer
{
	private Label _scoreLabel;
	
	public override void _Ready()
	{
		var addBlockButton = GetNode<Button>("AddBlock");
		addBlockButton.FocusMode = Control.FocusModeEnum.None;
		addBlockButton.Pressed += OnAddBlockButtonPressed;

		var mainMenuButton = GetNode<Button>("MainMenuButton");
		mainMenuButton.FocusMode = Control.FocusModeEnum.None;
		mainMenuButton.Pressed += OnMainMenuButtonPressed;

		_scoreLabel = GetNode<Label>("ScoreLabel");
		OnScoreChanged(GameState.Instance.Score);
		GameState.Instance.ScoreChanged += OnScoreChanged;
	}

	public override void _ExitTree()
	{
		GameState.Instance.ScoreChanged -= OnScoreChanged;
	}

	private void OnScoreChanged(ulong score)
	{
		_scoreLabel.Text = "Score: " + score;
	}

	private void OnAddBlockButtonPressed() {
		GetNode<BlockCanvas>("CodeWindowContainer/VBoxContainer/BlockCanvas").Spawn(BlockFactory.CreateRandom());
	}

	private void OnMainMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("gui/menus/main_menu/main_menu.tscn");
	}
}
