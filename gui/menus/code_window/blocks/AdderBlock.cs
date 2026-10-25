using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class AdderBlock : OperatorBlock {

	protected override Color BlockColor => new("4c97ff");
	protected override string Symbol => "+";
	protected override BlockValue Combine(BlockValue l, BlockValue r) => BlockValue.Add(l, r);
}
