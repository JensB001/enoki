using Godot;

public partial class God : Node
{
	[Export] public PackedScene EnemyChimknScene { get; set; }
	[Export] public PackedScene EnemyGlogScene { get; set; }
	[Export] public Player Target { get; set; }
	[Export] public float Chimkn_Interval { get; set; } = 2f;
	[Export] public float Glog_Interval { get; set; } = 10f;

	/// <summary>How far outside the visible screen enemies appear.</summary>
	[Export] public float SpawnMargin { get; set; } = 48f;

	[ExportGroup("Difficulty")]
	/// <summary>Spawn intervals halve every this many seconds.</summary>
	[Export] public float HalvingTime { get; set; } = 90f;
	/// <summary>Spawn intervals never drop below this fraction of their starting value.</summary>
	[Export(PropertyHint.Range, "0.05,1,0.05")] public float MinIntervalScale { get; set; } = 0.15f;

	private double _elapsed;
	private Timer _chimknTimer;
	private Timer _glogTimer;

	public override void _Ready()
	{
		GameState.Instance.ResetRun();

		_chimknTimer = new Timer { WaitTime = Chimkn_Interval, Autostart = true };
		_chimknTimer.Timeout += Spawn_Chimkn;
		AddChild(_chimknTimer);

		_glogTimer = new Timer { WaitTime = Glog_Interval, Autostart = true };
		_glogTimer.Timeout += Spawn_Glog;
		AddChild(_glogTimer);
	}

	public override void _Process(double delta)
	{
		_elapsed += delta;
	}

	/// <summary>1 at the start of a run, shrinking over time so spawns speed up.</summary>
	private float IntervalScale =>
		Mathf.Max(MinIntervalScale, Mathf.Pow(0.5f, (float)_elapsed / HalvingTime));

	private void Spawn_Chimkn()
	{
		_chimknTimer.WaitTime = Chimkn_Interval * IntervalScale;
		if (!IsInstanceValid(Target)) return;

		var enemy_chimkn = EnemyChimknScene.Instantiate<Chimkn>();

		AddChild(enemy_chimkn);

		enemy_chimkn.Target = Target;
		enemy_chimkn.GlobalPosition = SpawnPosition();
	}

	private void Spawn_Glog()
	{
		_glogTimer.WaitTime = Glog_Interval * IntervalScale;
		if (!IsInstanceValid(Target)) return;

		var enemy_glog = EnemyGlogScene.Instantiate<GlogTheGlutton>();

		AddChild(enemy_glog);

		enemy_glog.Target = Target;
		enemy_glog.GlobalPosition = SpawnPosition();
	}

	/// <summary>A random point on a circle just outside the visible screen around the player.</summary>
	private Vector2 SpawnPosition()
	{
		var zoom = GetViewport().GetCamera2D()?.Zoom ?? Vector2.One;
		float radius = (GetViewport().GetVisibleRect().Size / zoom).Length() * 0.5f + SpawnMargin;

		return Target.GlobalPosition + Vector2.Right.Rotated(GD.Randf() * Mathf.Tau) * radius;
	}
}
