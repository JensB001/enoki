using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public abstract partial class OperatorBlock : Block {

	private BlockSlot _leftSlot;
	private BlockSlot _rightSlot;

	public Block Left => _leftSlot.Block;
	public Block Right => _rightSlot.Block;
	
	protected abstract BlockValue Combine(BlockValue left, BlockValue right);

	public override BlockValue Evaluate() => Combine(Left?.Evaluate(), Right?.Evaluate());

	protected abstract string Symbol { get; }

	public override void _Ready() {
		base._Ready();

		var row = new HBoxContainer();
		row.MouseFilter = MouseFilterEnum.Ignore;
		row.AddThemeConstantOverride("separation", 6);

		_leftSlot = new BlockSlot();
		_rightSlot = new BlockSlot();

		row.AddChild(_leftSlot);
		row.AddChild(new Label { Text = Symbol, VerticalAlignment = VerticalAlignment.Center });
		row.AddChild(_rightSlot);
		AddChild(row);
	}
}
