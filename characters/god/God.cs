using Godot;

public partial class God : Node
{
	[Export] public PackedScene EnemyChimknScene { get; set; }
	[Export] public PackedScene EnemyGlogScene { get; set; }
	[Export] public Player Target { get; set; }
	[Export] public float Chimkn_Interval { get; set; } = 2f;
    [Export] public float Glog_Interval { get; set; } = 10f;
	[Export] public float SpawnRadius { get; set; } = 800f;

	public override void _Ready()
	{
		var chimkn_timer = new Timer { WaitTime = Chimkn_Interval, Autostart = true };
		chimkn_timer.Timeout += Spawn_Chimkn;
		AddChild(chimkn_timer);

        var glog_timer = new Timer { WaitTime = Glog_Interval, Autostart = true };
		glog_timer.Timeout += Spawn_Glog;
		AddChild(glog_timer);
	}

	private void Spawn_Chimkn()
	{
		if (!IsInstanceValid(Target)) return;

		var enemy_chimkn = EnemyChimknScene.Instantiate<Chimkn>();

		AddChild(enemy_chimkn);

		enemy_chimkn.Target = Target;
		enemy_chimkn.GlobalPosition = Target.GlobalPosition
			+ Vector2.Right.Rotated(GD.Randf() * Mathf.Tau) * SpawnRadius;
	}

    private void Spawn_Glog()
    {
		if (!IsInstanceValid(Target)) return;

        var enmey_glog = EnemyGlogScene.Instantiate<GlogTheGlutton>();

        AddChild(enmey_glog);
        
        enmey_glog.Target = Target;
		enmey_glog.GlobalPosition = Target.GlobalPosition
			+ Vector2.Right.Rotated(GD.Randf() * Mathf.Tau) * SpawnRadius;
	}
}
