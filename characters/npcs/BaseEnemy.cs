using Godot;

public partial class BaseEnemy : CharacterBody2D, IDamageable
{
	protected virtual int Speed => 0;
	protected virtual int MaxHealth => 0;
	[Export] public int ScoreValue { get; set; } = 10;
	[Export(PropertyHint.Range, "0,1,0.01")] public float DropChance { get; set; } = 0.25f;

	public Player Target { get; set; }
	public AnimatedSprite2D animatedSprite2D;
	MovementComponent Movement { get; set; }
	[Export] HealthComponent Health { get; set; }
	RarityComponent Rarity { get; set; }

	private double _attackCooldown;

	public float PhysicalDamage { get; set; } = 5;
	public float FireDamage { get; set; }
	public float IceDamage { get; set; }

	[Export] public double AttackInterval { get; set; } = 0.5;

	public override void _Ready()
	{
		Movement ??= new MovementComponent();
		Health ??= new HealthComponent();
		Rarity ??= new RarityComponent();
		
		AddChild(Movement);
		Movement.Body = this;

		Health.MaxHealth = MaxHealth;
		Health.Died += OnDied;

		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void TakeDamage(DamageInfo damage)
	{
		Health.TakeDamage(damage);
	}
	
	private void OnDied() {
		GameState.Instance.AddScore(ScoreValue);

		Rarity.Drop();

		QueueFree();
	}

	private void CheckPlayerCollision()
	{
		if (_attackCooldown > 0) return;

		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			if (GetSlideCollision(i).GetCollider() is Player player && player is IDamageable target)
			{
				target.TakeDamage(new DamageInfo(PhysicalDamage, FireDamage, IceDamage));
				_attackCooldown = AttackInterval;
				break;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_attackCooldown > 0) _attackCooldown -= delta;

		if (!IsInstanceValid(Target)) return;
		Movement.Move(GlobalPosition.DirectionTo(Target.GlobalPosition), Speed);

		CheckPlayerCollision();

		if (Movement.Body.Velocity.X != 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.FlipV = false;
			// See the note below about the following boolean assignment.
			animatedSprite2D.FlipH = Movement.Body.Velocity.X > 0;
			animatedSprite2D.Play();
		}
		else if (Movement.Body.Velocity.Y != 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.Play();
		}
	}
}
