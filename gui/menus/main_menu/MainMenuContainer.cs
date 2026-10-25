using Godot;
using System;
using System.Diagnostics;

public partial class MainMenuContainer : VBoxContainer
{
	public override void _Ready()
	{
		var playButton = GetNode<Button>("PlayButton");
		var exitButton = GetNode<Button>("ExitButton");
		
		playButton.Pressed += OnPlayButtonPressed;
		exitButton.Pressed += OnExitButtonPressed;
	}
	
	public override void _Process(double delta)
	{
	}

	private void OnPlayButtonPressed()
	{
		GetTree().ChangeSceneToFile("world/game_screen.tscn");
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}
