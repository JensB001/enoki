using Godot;

namespace Enoki;

public partial class CodeWindowContainer : PanelContainer {
	
	private bool _dragWindowButtonPressed = false;
	private Vector2 _previousMousePosition = Vector2.Zero;
	
	public override void _Ready() {
		var exitButton = GetNode<Button>("VBoxContainer/ExitButton");
		exitButton.Pressed += OnExitButtonPressed;

		var dragWindowButton = GetNode<Button>("DragWindowButton");
		dragWindowButton.ButtonDown += OnDragWindowButtonDown;
		dragWindowButton.ButtonUp += OnDragWindowButtonUp;

		var testButton = GetNode<Button>("VBoxContainer/TestButton");
		testButton.Pressed += OnTestButtonPressed;
	}
	
	public override void _Process(double delta) {
		if (_dragWindowButtonPressed) {
			var currentMousePosition = GetViewport().GetMousePosition();
			var mouseDelta = currentMousePosition - _previousMousePosition;
			
			Position += mouseDelta;
			_previousMousePosition = currentMousePosition;
		}
	}

	private void OnExitButtonPressed() {
		Hide();
	}

	private void OnDragWindowButtonDown() {
		_dragWindowButtonPressed = true;
		_previousMousePosition = GetViewport().GetMousePosition();
	}

	private void OnDragWindowButtonUp() {
		_dragWindowButtonPressed = false;
	}
	
	private void OnTestButtonPressed() {
		// var result = _engine.Evaluate(_code);
		// GD.Print(result);
	}
}
