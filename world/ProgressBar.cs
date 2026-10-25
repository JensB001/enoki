using Godot;

public partial class ProgressBar : Godot.ProgressBar
{
	private HealthComponent _target;

	[Export]
	public HealthComponent Target
	{
		get => _target;
		set
		{
			if (_target != null) _target.HealthChange -= OnHealthChanged;
			_target = value;
			if (_target == null) return;
			_target.HealthChange += OnHealthChanged;
			OnHealthChanged(_target.Health, _target.MaxHealth);
		}
	}

	public override void _Ready()
	{
		ShowPercentage = false;
	}

	private void OnHealthChanged(float health, float maxHealth)
	{
		MaxValue = maxHealth;
		Value = health;
	}

	public override void _ExitTree()
	{
		if (_target != null && IsInstanceValid(_target))
			_target.HealthChange -= OnHealthChanged;
	}
}
