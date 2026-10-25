using Godot;
using System;

public partial class GlogTheGlutton : BaseEnemy
{
	protected override int Speed => 88;
	protected override int MaxHealth => 50;
    protected override float PhysicalDamage => 5;
	protected override float DropChance => 0.45f;
}
