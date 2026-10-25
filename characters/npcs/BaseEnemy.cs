using Godot;

public partial class BaseEnemy : CharacterBody2D, IDamageable
{
	protected virtual int Speed => 0;
	protected virtual int MaxHealth => 0;
	[Export] public int ScoreValue { get; set; } = 10;
	[Export(PropertyHint.Range, "0,1,0.01")] public float DropChance { get; set; } = 0.25f;
	
	[ExportGroup("Status Effects")]
	[Export] public double BurnDuration { get; set; } = 3.0;
	[Export] public double BurnTickInterval { get; set; } = 0.5;
	[Export] public double FreezeDuration { get; set; } = 2.0;
	[Export] public Color BurnTint { get; set; } = new Color(1.0f, 0.6f, 0.6f);
	[Export] public Color BurnPulseTint { get; set; } = new Color(1.0f, 0.25f, 0.25f);
	[Export] public Color FreezeTint { get; set; } = new Color(0.6f, 0.75f, 1.0f);
	[Export] public Color BothTint { get; set; } = new Color(0.8f, 0.6f, 1.0f);       // purple
	[Export] public Color BothPulseTint { get; set; } = new Color(0.95f, 0.3f, 1.0f); // stronger purple

	private float _burnPulse; // 1 right after a tick, fades to 0

	public Player Target { get; set; }
	public AnimatedSprite2D animatedSprite2D;
	MovementComponent Movement { get; set; }
	[Export] HealthComponent Health { get; set; }
	RarityComponent Rarity { get; set; }

	private double _attackCooldown;
	
	private double _burnTimeLeft;
	private double _burnTickTimer;
	private float _burnTickDamage;
	private double _freezeTimeLeft;

	private bool IsBurning => _burnTimeLeft > 0;
	private bool IsFrozen => _freezeTimeLeft > 0;

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

		// Don't apply effects if that hit killed us
		if (IsQueuedForDeletion()) return;

		if (damage.Fire > 0) ApplyBurn(damage.Fire);
		if (damage.Ice > 0) ApplyFreeze();
	}

	private void ApplyBurn(float tickDamage)
	{
		_burnTimeLeft = BurnDuration;              // refresh duration
		_burnTickTimer = BurnTickInterval;
		_burnTickDamage = Mathf.Max(_burnTickDamage, tickDamage); // keep strongest
		UpdateTint();
	}

	private void ApplyFreeze()
	{
		_freezeTimeLeft = FreezeDuration;
		UpdateTint();
	}
	
	private void UpdateStatusEffects(double delta)
	{
		bool changed = false;

		if (IsBurning)
		{
			_burnTimeLeft -= delta;
			_burnTickTimer -= delta;

			while (_burnTickTimer <= 0 && !IsQueuedForDeletion())
			{
				Health.TakeDamage(new DamageInfo(0, _burnTickDamage, 0));
				_burnTickTimer += BurnTickInterval;
				_burnPulse = 1f; // pulse on every tick
			}

			// Fade pulse out over one tick interval
			_burnPulse = Mathf.MoveToward(_burnPulse, 0f, (float)(delta / BurnTickInterval));

			if (!IsBurning)
			{
				_burnTickDamage = 0;
				_burnPulse = 0f;
				changed = true;
			}
		}

		if (IsFrozen)
		{
			_freezeTimeLeft -= delta;
			if (!IsFrozen) changed = true;
		}

		if (IsQueuedForDeletion()) return;

		// While burning the tint changes every frame because of the pulse
		if (changed || IsBurning) UpdateTint();
	}

	private void UpdateTint()
	{
		Color tint = Colors.White;

		if (IsBurning && IsFrozen)
			tint = BothTint.Lerp(BothPulseTint, _burnPulse);
		else if (IsBurning)
			tint = BurnTint.Lerp(BurnPulseTint, _burnPulse);
		else if (IsFrozen)
			tint = FreezeTint;

		animatedSprite2D.Modulate = tint;
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

		UpdateStatusEffects(delta);
		if (IsQueuedForDeletion()) return;   // burned to death this frame

		if (IsFrozen)
		{
			Velocity = Vector2.Zero;
			animatedSprite2D.Pause();
			return;                          // no movement, no attacking
		}

		if (!IsInstanceValid(Target)) return;
		Movement.Move(GlobalPosition.DirectionTo(Target.GlobalPosition), Speed);

		CheckPlayerCollision();

		if (Movement.Body.Velocity.X != 0)
		{
			animatedSprite2D.Animation = "default";
			animatedSprite2D.FlipV = false;
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
