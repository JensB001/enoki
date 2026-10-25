using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class MultiplierBlock : OperatorBlock {

	protected override Color BlockColor => new("9966ff");
	protected override string Symbol => "×";
	protected override BlockValue Combine(BlockValue l, BlockValue r) => BlockValue.Multiply(l, r);
}
