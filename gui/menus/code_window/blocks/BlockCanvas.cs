using System.Linq;
using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class BlockCanvas : Control {

	private const float Margin = 24;
	
	private static bool Touches(Control a, Control b) => a.GetGlobalRect().Intersects(b.GetGlobalRect());
	
	private static bool IsProtected(Block block) =>
		block is ShootBlock or TrashBlock or AttributeBlock { Value: AttributeType.Bullet };

// Protected blocks, and any expression that contains one, can't be deleted
	private static bool IsDeletable(Block block) =>
		!IsProtected(block) &&
		!block.FindChildren("*", "", true, false).OfType<Block>().Any(IsProtected);

	private int _spawnCount;
	private Control _window;

	public override void _Ready() {
		MouseFilter = MouseFilterEnum.Ignore;
		_window = GetParent().GetParent<Control>();

		Spawn(new ShootBlock());
		Spawn(new TrashBlock());
		PublishShoot();

		GameState.Instance.UpgradeDropped += Spawn;
	}

	public override void _ExitTree() {
		// Unsubscribe, or the event keeps pointing at a freed node
		if (GameState.Instance != null) {
			GameState.Instance.UpgradeDropped -= Spawn;
		}
	}

	public void PublishShoot() {
		var shoot = GetChildren().OfType<ShootBlock>().FirstOrDefault();
		if (shoot != null) {
			GameState.Instance.SetShootValue(shoot.Evaluate());
		}
	}

	public void Spawn(Block block) {
		AddChild(block);
		block.Position = Vector2.One * (Margin + _spawnCount++ % 10 * 16);
		FitToBlocks();
	}

	public bool TryDropIntoSlot(Block block) {
		if (block is ShootBlock or TrashBlock) return false;

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
	
	public bool TryTrash(Block dragged) {
		if (dragged is TrashBlock) {
			// Trashcan dragged onto things: delete every deletable block it overlaps
			var touched = FindChildren("*", "", true, false)
				.OfType<Block>()
				.Where(b => b != dragged && IsDeletable(b) && Touches(b, dragged))
				.ToList();

			// If a nested block is touched, delete only that one, not the parent expression around it
			var toDelete = touched
				.Where(b => !touched.Any(other => other != b && b.IsAncestorOf(other)))
				.ToList();

			foreach (var block in toDelete) {
				Delete(block);
			}
			return toDelete.Count > 0;
		}

		if (!IsDeletable(dragged)) return false;

		// Block dragged onto a trashcan: delete the block
		var hitTrash = GetChildren()
			.OfType<TrashBlock>()
			.Any(t => Touches(t, dragged));

		if (hitTrash) {
			Delete(dragged);
		}
		return hitTrash;
	}

	private static void Delete(Block block) {
		// Remove immediately: QueueFree alone leaves the node in the tree until end of frame,
		// so FitToBlocks / PublishShoot would still see it.
		block.GetParent().RemoveChild(block);
		block.QueueFree();
	}
}
