using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public enum AttributeType {
	Fire,
	Ice,
	MoveUp,
	MoveDown,
	MoveLeft,
	MoveRight,
}

public partial class AttributeBlock : Block {

	public AttributeType Value { get; set; }

	protected override Color BlockColor => new("ff8c1a");

	public override void _Ready() {
		base._Ready();
		AddChild(new Label { Text = Value.ToString() });
	}
}
