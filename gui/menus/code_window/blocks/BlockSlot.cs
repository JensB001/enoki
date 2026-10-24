using System.Linq;
using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class BlockSlot : PanelContainer {

	public Block Block => GetChildren().OfType<Block>().FirstOrDefault();
	public bool IsEmpty => Block == null;

	public override void _Ready() {
		CustomMinimumSize = new Vector2(40, 28);
		MouseFilter = MouseFilterEnum.Ignore;

		var style = new StyleBoxFlat();
		style.BgColor = new Color(0, 0, 0, 0.3f);
		style.SetCornerRadiusAll(4);
		style.SetContentMarginAll(2);
		AddThemeStyleboxOverride("panel", style);
	}

	public void Insert(Block block) {
		block.Reparent(this, false);
	}
}
