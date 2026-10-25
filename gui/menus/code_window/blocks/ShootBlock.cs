using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class ShootBlock : Block {

	private BlockSlot _slot;

	public Block Content => _slot.Block;

	protected override Color BlockColor => new("ff6680");

	public override void _Ready() {
		base._Ready();

		var row = new HBoxContainer();
		row.MouseFilter = MouseFilterEnum.Ignore;
		row.AddThemeConstantOverride("separation", 6);

		_slot = new BlockSlot();

		row.AddChild(new Label { Text = "Shoot", VerticalAlignment = VerticalAlignment.Center });
		row.AddChild(_slot);
		AddChild(row);

		// Default content
		_slot.AddChild(new AttributeBlock { Value = AttributeType.Bullet });
	}

	public override BlockValue Evaluate() => Content?.Evaluate();
}
