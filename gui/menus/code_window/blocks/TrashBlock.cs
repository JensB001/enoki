using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class TrashBlock : Block {

    protected override Color BlockColor => new("8c8c8c");

    // A trashcan produces no value
    public override BlockValue Evaluate() => null;

    public override void _Ready() {
        base._Ready();
        AddChild(new Label { Text = "Trash", HorizontalAlignment = HorizontalAlignment.Center });
    }
}