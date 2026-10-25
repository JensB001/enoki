using Godot;
using System;

/// <summary>
/// Gives an AnimatedSprite2D a white outline and a white hit flash (see sprite_fx.gdshader).
/// </summary>
[GlobalClass]
public partial class SpriteFxComponent : Node
{
	[Export] public AnimatedSprite2D Sprite { get; set; }
	[Export] public Color OutlineColor { get; set; } = Colors.White;
	[Export] public float OutlineWidth { get; set; } = 1;
	[Export] public float FlashDuration { get; set; } = 0.12f;

	private ShaderMaterial _material;
	private Tween _flashTween;

	public override void _Ready()
	{
		// Own material per sprite, so one enemy flashing doesn't flash them all.
		_material = new ShaderMaterial { Shader = GD.Load<Shader>("res://components/sprite_fx.gdshader") };
		_material.SetShaderParameter("outline_color", OutlineColor);
		_material.SetShaderParameter("outline_width", OutlineWidth);
		Sprite.Material = _material;
	}

	public override void _Process(double delta)
	{
		var frame = Sprite.SpriteFrames.GetFrameTexture(Sprite.Animation, Sprite.Frame);

		var region = new Vector4(0, 0, 1, 1);
		if (frame is AtlasTexture atlas)
		{
			var size = atlas.Atlas.GetSize();
			region = new Vector4(
				atlas.Region.Position.X / size.X, atlas.Region.Position.Y / size.Y,
				atlas.Region.End.X / size.X, atlas.Region.End.Y / size.Y);
		}

		_material.SetShaderParameter("region", region);
		_material.SetShaderParameter("flip", new Vector2(Sprite.FlipH ? -1 : 1, Sprite.FlipV ? -1 : 1));
	}

	public void Flash()
	{
		_flashTween?.Kill();
		_flashTween = CreateTween();
		_flashTween.TweenMethod(Callable.From<float>(v => _material.SetShaderParameter("flash", v)), 1f, 0f, FlashDuration);
	}
}
