using Godot;
using Enoki.gui.menus.code_window.blocks;

public partial class BaseEnemy : CharacterBody2D, IDamageable
{
	[Export] public int Speed { get; set; } = 410; // How fast the player will move (pixels/sec).
	[Export] public int ScoreValue { get; set; } = 10;
	[Export(PropertyHint.Range, "0,1,0.01")] public float DropChance { get; set; } = 0.25f;
	[Export] public float MaxHealth { get; set; } = 3;
	private float _health;

	public Player Target { get; set; }
	public AnimatedSprite2D animatedSprite2D;
	MovementComponent Movement { get; set; }

	public override void _Ready()
	{
		_health = MaxHealth;
		Movement ??= new MovementComponent();
		AddChild(Movement);
		Movement.Body = this;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}
	
	public void TakeDamage(DamageInfo damage)
	{
		_health -= damage.Physical + damage.Fire + damage.Ice;
		if (_health <= 0) {
			Die();
		}
	}
	
	private void Die() {
		GameState.Instance.AddScore(ScoreValue);

		if (GD.Randf() < DropChance) {
			GameState.Instance.DropUpgrade(BlockFactory.CreateRandom());
		}

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
