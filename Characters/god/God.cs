using Godot;
using System;

public partial class God : Node
{
    [Export] public PackedScene EnemyScene { get; set; }
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
		GD.Print("Spawn called, Target: ", Target, ", Scene: ", EnemyScene);
        if (!IsInstanceValid(Target)) return;

        var enemy = EnemyScene.Instantiate<BaseEnemy>();
        AddChild(enemy);
        enemy.Target = Target;
        enemy.GlobalPosition = Target.GlobalPosition
            + Vector2.Right.Rotated(GD.Randf() * Mathf.Tau) * SpawnRadius;
    }
}
