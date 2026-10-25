using Godot;

public partial class Bullet : Area2D {

	[Export] public float Speed { get; set; } = 128;
	[Export] public float Lifetime { get; set; } = 3;

	public Vector2 Direction { get; set; } = Vector2.Right;
	public float PhysicalDamage { get; set; } = 1;
	public float FireDamage { get; set; }
	public float IceDamage { get; set; }

	public AnimatedSprite2D animatedSprite2D;

	public override void _Ready() {
		BodyEntered += OnBodyEntered;

		animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite2D");
	}

	public override void _PhysicsProcess(double delta) {
		GlobalPosition += Direction * Speed * (float)delta;

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


		if (body is IDamageable target) {
			target.TakeDamage(new DamageInfo(PhysicalDamage, FireDamage, IceDamage));
		}
		QueueFree();
	}
}
