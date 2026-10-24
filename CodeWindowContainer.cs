using Godot;
using Jint;
using Engine = Jint.Engine;

namespace Enoki;

public partial class CodeWindowContainer : PanelContainer {
	
	private Engine _engine = new Engine();

	private bool _dragWindowButtonPressed = false;
	private Vector2 _previousMousePosition = Vector2.Zero;
	
	private string _code = "";
	
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
		_code = GetNode<CodeEdit>("VBoxContainer/CodeEdit").Text;
		
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
		var result = _engine.Evaluate(_code).AsNumber();
		GD.Print(result);
	}
}
