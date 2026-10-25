using System;
using Enoki.gui.menus.code_window.blocks;
using Godot;

public partial class GameState : Node {

	public static GameState Instance { get; private set; }

	// Run data (this is also what you'd save later)
	public int Score { get; private set; }
	public int Round { get; private set; } = 1;

	// Derived from the code window, not saved directly
	public BlockValue ShootValue { get; private set; } = BlockValue.FromAttribute(AttributeType.Bullet);

	public event Action<BlockValue> ShootValueChanged;
	public event Action<Block> UpgradeDropped;
	public event Action<int> ScoreChanged;
	public event Action<int> RoundChanged;

	public override void _Ready() => Instance = this;

	/// <summary>Called when a new game starts; GameState is an autoload, so it outlives each run.</summary>
	public void ResetRun() {
		Score = 0;
		Round = 1;
		ScoreChanged?.Invoke(Score);
		RoundChanged?.Invoke(Round);
	}

	public void SetShootValue(BlockValue value) {
		ShootValue = value;
		ShootValueChanged?.Invoke(value);
	}

	public void DropUpgrade(Block block) => UpgradeDropped?.Invoke(block);

	public void AddScore(int amount) {
		Score += amount;
		ScoreChanged?.Invoke(Score);
	}

	public void NextRound() {
		Round++;
		RoundChanged?.Invoke(Round);
	}
}
