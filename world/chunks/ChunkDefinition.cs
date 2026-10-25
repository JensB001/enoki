using Godot;

/// <summary>One hand-made chunk plus the rules for where/how often it may appear.</summary>
[GlobalClass]
public partial class ChunkDefinition : Resource
{
	/// <summary>Chunk scene (a TileMapLayer painted in local tile coords 0..31).</summary>
	[Export] public PackedScene Scene;

	/// <summary>Relative spawn frequency.</summary>
	[Export] public float Weight = 1f;

	/// <summary>Fillers ignore both distance rules and are the fallback when a special loses.</summary>
	[Export] public bool IsFiller;

	/// <summary>Never spawns within this many chunks (Chebyshev) of chunk (0,0).</summary>
	[Export] public int MinDistanceFromOrigin;

	/// <summary>No other chunk of this type within this many chunks (Chebyshev).</summary>
	[Export] public int MinDistanceSameType;
}
