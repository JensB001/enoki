using Godot;

public partial class Bullet : Area2D {

	[Export] public float Speed { get; set; } = 128;
	[Export] public float Lifetime { get; set; } = 3;
	[Export] public float Knockback { get; set; } = 100;

	public Vector2 Direction { get; set; } = Vector2.Right;
	public Vector2 InheritedVelocity { get; set; } = Vector2.Zero;
	public float PhysicalDamage { get; set; } = 1;
	public float FireDamage { get; set; }
	public float IceDamage { get; set; }

	public AnimatedSprite2D animatedSprite2D;

	public override void _Ready() {
		BodyEntered += OnBodyEntered;

		animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite2D");

		AddChild(new SpriteFxComponent { Sprite = animatedSprite2D });
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 velocity = Direction * Speed + InheritedVelocity;
		GlobalPosition += velocity * (float)delta;

		if (FireDamage <= 0 && IceDamage <= 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.Play();
		}
		else if (FireDamage > 0 && IceDamage > 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.Play();
		}
		else if (FireDamage > 0 && IceDamage <= 0)
		{
			animatedSprite2D.Animation = "fire";
			animatedSprite2D.Play();
		}
		else if (FireDamage <= 0 && IceDamage > 0)
		{
			animatedSprite2D.Animation = "ice";
			animatedSprite2D.Play();
		}

		Lifetime -= (float)delta;
		if (Lifetime <= 0) {
			QueueFree();
		}
	}

	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			return;
		}


		if (body is BaseEnemy enemy) {
			enemy.ApplyKnockback(Direction.Normalized() * Knockback);
		}

		if (body is IDamageable target) {
			target.TakeDamage(new DamageInfo(PhysicalDamage, FireDamage, IceDamage));
		}
		QueueFree();
	}
}
