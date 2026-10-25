using Enoki.gui.menus.code_window.blocks;
using Godot;
using System;

public partial class RarityComponent : Node
{
	[Export (PropertyHint.Range, "0,1,0.01")]public float DropChance { get; set; } = 0.25f;

	public void Drop()
	{
		if (GD.Randf() < DropChance)
		{
			GameState.Instance.DropUpgrade(BlockFactory.CreateRandom());
		}
	}
}
