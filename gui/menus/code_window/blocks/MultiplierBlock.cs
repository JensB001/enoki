using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class MultiplierBlock : OperatorBlock {

	protected override Color BlockColor => new("9966ff");
	protected override string Symbol => "×";
}
