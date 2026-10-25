using Godot;

namespace Enoki.gui.menus.code_window.blocks;

public partial class TrashBlock : Block {

    private const string IconPath = "res://asset/trashcan.png"; // adjust to your path

    protected override Color BlockColor => new("8c8c8c");

    // A trashcan produces no value
    public override BlockValue Evaluate() => null;

    public override void _Ready() {
        base._Ready();

        var icon = new TextureRect {
            Texture = GD.Load<Texture2D>(IconPath),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            CustomMinimumSize = new Vector2(32, 32),
            MouseFilter = MouseFilterEnum.Ignore,
        };
        AddChild(icon);
    }
}