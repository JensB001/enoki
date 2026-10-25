using Godot;

namespace Enoki.gui.menus.code_window;

public partial class CodeWindowContainer : PanelContainer {
	
	private bool _dragWindowButtonPressed;
	private Vector2 _previousMousePosition = Vector2.Zero;
	
	public override void _Ready() {
		var dragWindowButton = GetNode<Button>("DragWindowButton");
		dragWindowButton.FocusMode = Control.FocusModeEnum.None;
		dragWindowButton.ButtonDown += OnDragWindowButtonDown;
		dragWindowButton.ButtonUp += OnDragWindowButtonUp;

		// Always be as small as the children allow
		MinimumSizeChanged += () => CallDeferred(Control.MethodName.ResetSize);
		ResetSize();
	}
	
	public override void _Process(double delta) {
		if (_dragWindowButtonPressed) {
			var currentMousePosition = GetViewport().GetMousePosition();
			var mouseDelta = currentMousePosition - _previousMousePosition;
			
			var target = Position + mouseDelta;
			var viewportSize = GetViewportRect().Size;
			Position = new Vector2(
				Mathf.Clamp(target.X, 0, Mathf.Max(viewportSize.X - Size.X, 0)),
				Mathf.Clamp(target.Y, 0, Mathf.Max(viewportSize.Y - Size.Y, 0)));
			_previousMousePosition = currentMousePosition;
		}
	}

	private void OnDragWindowButtonDown() {
		_dragWindowButtonPressed = true;
		_previousMousePosition = GetViewport().GetMousePosition();
	}

	private void OnDragWindowButtonUp() {
		_dragWindowButtonPressed = false;
	}
}
