using System.Collections.Generic;
using Enoki.gui.menus.code_window.blocks;
using Godot;

public partial class Player : CharacterBody2D, IDamageable
{
	[Export] MovementComponent Movement { get; set; }
	[Export] public PackedScene BulletScene { get; set; }

	public int Speed { get; set; } = 96; // How fast the player will move (pixels/sec).

	public Vector2 ScreenSize; // Size of the game window.
	public AnimatedSprite2D animatedSprite2D;

	[Export] public HealthComponent Health {get; set;}

	private double _invincible;

	public override void _Ready()
	{
		// GetTree().Root.PrintTreePretty();
		ScreenSize = GetViewportRect().Size;

		Movement ??= new MovementComponent();
		Health ??= new HealthComponent();

		AddChild(Movement);
		Movement.Body = this;

		Health.MaxHealth = 150;

		Health.Died += OnDied;

		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");


		var hud = GetNodeOrNull<CanvasLayer2>("../CanvasLayer2");
		GD.Print($"LOOKUP V2 at {GetPath()}: {(hud == null ? "null" : "found")}");
		hud?.Bind(this);
	}

	public void TakeDamage(DamageInfo damage)
	{
		if (_invincible > 0) return;

		_invincible = 0.75;

		Health.TakeDamage(damage);
	}

	private void OnDied() {
		GetTree().ChangeSceneToFile("res://gui/menus/main_menu/main_menu.tscn");
	}
	
	private void Shoot(BlockValue value) {
		var count = Mathf.Max(0, (int)value.Attributes.GetValueOrDefault(AttributeType.Bullet));
		var fire = value.Attributes.GetValueOrDefault(AttributeType.Fire);
		var ice = value.Attributes.GetValueOrDefault(AttributeType.Ice);

		var aim = GlobalPosition.DirectionTo(GetGlobalMousePosition());

		for (var i = 0; i < count; i++) {
			var bullet = BulletScene.Instantiate<Bullet>();
			GetParent().AddChild(bullet);
			bullet.GlobalPosition = GlobalPosition;
			bullet.Direction = aim.Rotated((i - (count - 1) / 2f) * 0.15f);
			bullet.InheritedVelocity = Velocity;   // new
			bullet.FireDamage = fire;
			bullet.IceDamage = ice;
		}
	}

	public override void _PhysicsProcess(double delta)
	{

		if (_invincible > 0) _invincible -= delta;

		// Reads WASD movement using the configured move_* actions
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Movement.Move(inputDir, Speed);

		// Handle Shooting input
		if (Input.IsActionJustPressed("shoot"))
		{
			Shoot(GameState.Instance.ShootValue);
		}

		// Animation Logic
		if (Movement.Body.Velocity.X != 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.FlipV = false;
			animatedSprite2D.FlipH = Movement.Body.Velocity.X < 0;
			animatedSprite2D.Play();
		}
		else if (Movement.Body.Velocity.Y != 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.Play();
 		}
		else
		{
			animatedSprite2D.Stop();
		}
	}
}
