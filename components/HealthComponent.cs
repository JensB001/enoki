using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node, IDamageable
{
	[Signal] public delegate void DiedEventHandler();

	[Export] public float MaxHealth { get; set; }
	private float _health;
	public bool isDead = false;

    public override void _Ready()
	{
		_health = MaxHealth;
	}


	public void TakeDamage(DamageInfo damage)
	{
		if (isDead) return;

		_health -= damage.Physical + damage.Fire + damage.Ice;
		if (_health <= 0)
		{
			isDead = true;
		}

		if (isDead) EmitSignal(SignalName.Died);
	}
}
