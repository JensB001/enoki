using Godot;

public partial class God : Node
{
	[Export] public PackedScene EnemyChimknScene { get; set; }
	[Export] public PackedScene EnemyGlogScene { get; set; }
	[Export] public Player Target { get; set; }
	[Export] public float Interval { get; set; } = 2f;
	[Export] public float SpawnRadius { get; set; } = 800f;

	public override void _Ready()
	{
		var timer = new Timer { WaitTime = Interval, Autostart = true };
		timer.Timeout += Spawn;
		AddChild(timer);
	}

	private void Spawn()
	{
		if (!IsInstanceValid(Target)) return;

		var enemy_chimkn = EnemyChimknScene.Instantiate<Chimkn>();
        var enmey_glog = EnemyGlogScene.Instantiate<GlogTheGlutton>();

		AddChild(enemy_chimkn);
        AddChild(enmey_glog);

		enemy_chimkn.Target = Target;
		enemy_chimkn.GlobalPosition = Target.GlobalPosition
			+ Vector2.Right.Rotated(GD.Randf() * Mathf.Tau) * SpawnRadius;
        
        enmey_glog.Target = Target;
		enmey_glog.GlobalPosition = Target.GlobalPosition
			+ Vector2.Right.Rotated(GD.Randf() * Mathf.Tau) * SpawnRadius;
	}
}
