using System.Linq;
using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class BlockCanvas : Control {

	private const float Margin = 24;

	private int _spawnCount;
	private Control _window;

	public override void _Ready() {
		MouseFilter = MouseFilterEnum.Ignore;
		_window = GetParent().GetParent<Control>();
		Spawn(new ShootBlock());
	}

	public void Spawn(Block block) {
		AddChild(block);
		block.Position = Vector2.One * (Margin + _spawnCount++ % 10 * 16);
		FitToBlocks();
	}

	public bool TryDropIntoSlot(Block block) {
		if (block is ShootBlock) return false;
		
		var mouse = GetGlobalMousePosition();
		var slot = FindChildren("*", "", true, false)
			.OfType<BlockSlot>()
			.LastOrDefault(s => s.IsEmpty && !block.IsAncestorOf(s) && s.GetGlobalRect().HasPoint(mouse));

		if (slot == null) {
			return false;
		}
		slot.Insert(block);
		return true;
	}

	// Keeps the window that results from this block position fully inside the viewport
	public Vector2 ClampToViewport(Block block, Vector2 target) {
		var screen = GetViewportRect().Size;
		var windowRect = _window.GetGlobalRect();
		var canvasRect = GetGlobalRect();
		var padding = Vector2.One * Margin;

		// Space the window uses around the canvas (title bar, panel padding, ...)
		var min = canvasRect.Position - windowRect.Position + padding;
		var max = screen - (windowRect.End - canvasRect.End) - padding - block.GetCombinedMinimumSize();

		return new Vector2(
			Mathf.Clamp(target.X, min.X, Mathf.Max(min.X, max.X)),
			Mathf.Clamp(target.Y, min.Y, Mathf.Max(min.Y, max.Y)));
	}

	// Window = exactly the bounding box of the blocks + margin. No blocks = no canvas at all.
	public void FitToBlocks() {
		var blocks = GetChildren().OfType<Block>().ToList();

		if (blocks.Count == 0) {
			CustomMinimumSize = Vector2.Zero;
			_window.Size = Vector2.Zero;
			return;
		}

		// Keep the top-left block exactly one margin from the canvas edge (grows AND shrinks left/up).
		// Blocks move inward and the window moves the other way, so nothing jumps on screen.
		var topLeft = new Vector2(blocks.Min(b => b.Position.X), blocks.Min(b => b.Position.Y));
		var shift = Vector2.One * Margin - topLeft;
		if (!shift.IsZeroApprox()) {
			foreach (var block in blocks) {
				block.Position += shift;
			}
			_window.Position -= shift;
		}

		var extent = Vector2.Zero;
		foreach (var block in blocks) {
			extent = extent.Max(block.Position + block.GetCombinedMinimumSize());
		}
		CustomMinimumSize = extent + Vector2.One * Margin;
		_window.Size = Vector2.Zero;
	}
}