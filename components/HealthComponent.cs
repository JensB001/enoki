using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node, IDamageable
{
	[Signal] public delegate void DiedEventHandler();

	private float _MaxHealth;

	[Export] public float MaxHealth { 
		get => _MaxHealth; 
		set
		{
			_MaxHealth = value;
			Health = value;
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

		Health -= damage.Physical + damage.Fire + damage.Ice;

		GD.Print($"Health after hit: {Health}/{MaxHealth}");

		if (Health <= 0)
		{
			isDead = true;
		}

		if (isDead) EmitSignal(SignalName.Died);
	}
}
