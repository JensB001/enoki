using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node, IDamageable
{
	[Signal] public delegate void DiedEventHandler();

	[Signal] public delegate void HealthChangeEventHandler(float Health, float MaxHealth);

	private float _MaxHealth;

	[Export] public float MaxHealth { 
		get => _MaxHealth; 
		set
		{
			_MaxHealth = value;
			Health = value;
			if (IsInsideTree())
        	EmitSignal(SignalName.HealthChange, Health, _MaxHealth);
		}
	}
	
	public float Health {get; private set;}
	public bool isDead = false;

    public override void _Ready()
	{
	}


	public void TakeDamage(DamageInfo damage)
	{
		if (isDead) return;

		Health = Mathf.Max(0, Health - (damage.Physical + damage.Fire + damage.Ice));

		EmitSignal(SignalName.HealthChange, Health, MaxHealth);


		if (Health <= 0)
		{
			isDead = true;
		}

		if (isDead) EmitSignal(SignalName.Died);
	}
}
