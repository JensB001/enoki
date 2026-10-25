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
	SpriteFxComponent SpriteFx { get; set; }

	[Export] public double FireInterval { get; set; } = 0.2; // seconds between shots while holding shoot

	[ExportGroup("Death")]
	[Export] public double DeathDelay { get; set; } = 1.2;       // real seconds before going back to the menu
	[Export] public double DeathTimeScale { get; set; } = 0.3;   // slow-mo while dying

	private double _invincible;
	private double _fireCooldown;

	public override void _Ready()
	{
		// GetTree().Root.PrintTreePretty();
		ScreenSize = GetViewportRect().Size;

		MotionMode = MotionModeEnum.Floating; // top-down movement, no floors/walls

		Movement ??= new MovementComponent();
		Health ??= new HealthComponent();

		AddChild(Movement);
		Movement.Body = this;

		Health.MaxHealth = 150;

		Health.Died += OnDied;

		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		SpriteFx = new SpriteFxComponent { Sprite = animatedSprite2D };
		AddChild(SpriteFx);

		var hud = GetNodeOrNull<CanvasLayer2>("../CanvasLayer2");
		hud?.Bind(this);
	}

	public void TakeDamage(DamageInfo damage)
	{
		if (_invincible > 0) return;

		_invincible = 0.75;

		SpriteFx.Flash();
		Health.TakeDamage(damage);
	}

	public void ApplyKnockback(Vector2 impulse)
	{
		if (_invincible > 0) return; // only get pushed by hits that actually land

		Movement.ApplyKnockback(impulse);
	}

	private void OnDied() {
		// Freeze in place, flash and fade out in slow-mo, then go back to the menu.
		SetPhysicsProcess(false);
		animatedSprite2D.Stop();
		SpriteFx.Flash();
		Engine.TimeScale = DeathTimeScale;

		var fade = CreateTween().SetIgnoreTimeScale();
		fade.TweenProperty(animatedSprite2D, "modulate:a", 0f, DeathDelay);

		var tree = GetTree();
		tree.CreateTimer(DeathDelay, processAlways: true, ignoreTimeScale: true).Timeout += () =>
		{
			if (!IsInstanceValid(this)) return; // already left the game (e.g. Main Menu button)
			Engine.TimeScale = 1;
			tree.ChangeSceneToFile("res://gui/menus/main_menu/main_menu.tscn");
		};
	}

	public override void _ExitTree()
	{
		Engine.TimeScale = 1; // never leave the slow-mo on when the game scene closes
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
		if (_fireCooldown > 0) _fireCooldown -= delta;

		// Blink while invincible after a hit
		bool hidden = _invincible > 0 && (int)(_invincible * 15) % 2 == 0;
		animatedSprite2D.Modulate = hidden ? new Color(1, 1, 1, 0.3f) : Colors.White;

		// Reads WASD movement using the configured move_* actions
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Movement.Move(inputDir, Speed);

		// Handle Shooting input (hold to keep firing)
		if (Input.IsActionPressed("shoot") && _fireCooldown <= 0)
		{
			Shoot(GameState.Instance.ShootValue);
			_fireCooldown = FireInterval;
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
