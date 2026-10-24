using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public abstract partial class Block : PanelContainer {

	private bool _dragging;
	private Vector2 _grabOffset;

	protected abstract Color BlockColor { get; }

	public override void _Ready() {
		var style = new StyleBoxFlat();
		style.BgColor = BlockColor;
		style.SetCornerRadiusAll(6);
		style.SetContentMarginAll(6);
		AddThemeStyleboxOverride("panel", style);

		MinimumSizeChanged += OnMinimumSizeChanged;
	}

	public override void _GuiInput(InputEvent @event) {
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }) {
			AcceptEvent();

			// Pull the block out of its slot so it can move freely
			var canvas = FindBlockCanvas();
			if (GetParent() != canvas) {
				Reparent(canvas);
			}
			MoveToFront();

			_grabOffset = GetGlobalMousePosition() - GlobalPosition;
			_dragging = true;
		}
	}

	// Motion and release are handled here instead of _GuiInput, because reparenting drops the GUI mouse focus
	public override void _Input(InputEvent @event) {
		if (!_dragging) {
			return;
		}

		if (@event is InputEventMouseMotion) {
			GlobalPosition = GetGlobalMousePosition() - _grabOffset;
			FindBlockCanvas().FitToBlocks();
			GetViewport().SetInputAsHandled();
		} else if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }) {
			_dragging = false;
			var canvas = FindBlockCanvas();
			canvas.TryDropIntoSlot(this);
			canvas.FitToBlocks();
			GetViewport().SetInputAsHandled();
		}
	}

	private BlockCanvas FindBlockCanvas() {
		var node = GetParent();
		while (node is not BlockCanvas) {
			node = node.GetParent();
		}
		return (BlockCanvas)node;
	}

	private void OnMinimumSizeChanged() {
		// Free blocks don't shrink on their own when a child is removed
		if (GetParent() is BlockCanvas) {
			Size = Vector2.Zero;
		}
	}
}
