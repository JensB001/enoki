using System.Linq;
using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class BlockCanvas : Control {

	private const float Margin = 24;
	private static readonly Vector2 BaseSize = new(400, 460);

	private int _spawnCount;
	private Control _window;

	public override void _Ready() {
		CustomMinimumSize = BaseSize;
		MouseFilter = MouseFilterEnum.Ignore;
		_window = GetParent().GetParent<Control>();
	}

	public void Spawn(Block block) {
		AddChild(block);
		block.Position = Vector2.One * (Margin + _spawnCount++ % 10 * 16);
		FitToBlocks();
	}

	public bool TryDropIntoSlot(Block block) {
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

	// Grows the window when a block enters the margin, and shrinks it back on the right and bottom
	public void FitToBlocks() {
		var blocks = GetChildren().OfType<Block>().ToList();

		// Growing left or up: push the blocks inward and move the window the other way, so nothing jumps on screen
		var shift = Vector2.Zero;
		foreach (var block in blocks) {
			shift.X = Mathf.Max(shift.X, Margin - block.Position.X);
			shift.Y = Mathf.Max(shift.Y, Margin - block.Position.Y);
		}
		if (shift != Vector2.Zero) {
			foreach (var block in blocks) {
				block.Position += shift;
			}
			_window.Position -= shift;
		}

		var extent = BaseSize;
		foreach (var block in blocks) {
			var size = block.Size.Max(block.GetCombinedMinimumSize());
			extent = extent.Max(block.Position + size + Vector2.One * Margin);
		}
		CustomMinimumSize = extent;
		_window.Size = Vector2.Zero;
	}
}
