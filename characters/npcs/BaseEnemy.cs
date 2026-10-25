using Godot;

public partial class BaseEnemy : CharacterBody2D, IDamageable
{
	[Export] public int Speed { get; set; } = 410; // How fast the player will move (pixels/sec).
	[Export] public int ScoreValue { get; set; } = 10;
	[Export(PropertyHint.Range, "0,1,0.01")] public float DropChance { get; set; } = 0.25f;

	public Player Target { get; set; }
	public AnimatedSprite2D animatedSprite2D;
	MovementComponent Movement { get; set; }
	[Export] HealthComponent Health { get; set; }
	RarityComponent Rarity { get; set; }

	public override void _Ready()
	{
		Movement ??= new MovementComponent();
		Health ??= new HealthComponent();
		Rarity ??= new RarityComponent();
		
		AddChild(Movement);
		Movement.Body = this;

		Health.MaxHealth = 3;
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

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(Target)) return;
		Movement.Move(GlobalPosition.DirectionTo(Target.GlobalPosition), Speed);

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
