using Godot;

public partial class Bullet : Area2D {

	[Export] public float Speed { get; set; } = 128;
	[Export] public float Lifetime { get; set; } = 3;

	public Vector2 Direction { get; set; } = Vector2.Right;
	public float PhysicalDamage { get; set; } = 1;
	public float FireDamage { get; set; }
	public float IceDamage { get; set; }

	public override void _Ready() {
		BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta) {
		GlobalPosition += Direction * Speed * (float)delta;

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
