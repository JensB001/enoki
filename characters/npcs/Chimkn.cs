using Godot;
using System;

public partial class Chimkn : BaseEnemy
{
	protected override int Speed => 128;
	protected override int MaxHealth => 3;
	protected override float PhysicalDamage => 15;
}
