using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node, IDamageable
{
	[Export] public float MaxHealth { get; set; }
	private float _health;

    public override void _Ready()
	{
		_health = MaxHealth;
	}


	public void TakeDamage(DamageInfo damage)
	{
		_health -= damage.Physical + damage.Fire + damage.Ice;
		if (_health <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		QueueFree();
	}
}
