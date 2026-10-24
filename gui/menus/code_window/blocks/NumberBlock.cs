using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class NumberBlock : Block {

	public float Value { get; set; }

	protected override Color BlockColor => new("59c059");

	public override void _Ready() {
		base._Ready();
		AddChild(new Label { Text = Value.ToString(), HorizontalAlignment = HorizontalAlignment.Center });
	}
}
