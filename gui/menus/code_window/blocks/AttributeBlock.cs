using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public enum AttributeType {
	Bullet,
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
	
	public override BlockValue Evaluate() => BlockValue.FromAttribute(Value);

	public override void _Ready() {
		base._Ready();
		AddChild(new Label { Text = Value.ToString(), HorizontalAlignment = HorizontalAlignment.Center });
	}
}
