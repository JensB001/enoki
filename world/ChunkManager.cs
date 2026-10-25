using System.Collections.Generic;
using Godot;

/// <summary>
/// Streams 32x32-tile chunks around the visible screen. Which hand-made chunk appears at
/// each chunk coordinate is a pure function of (Seed, coordinate), so the world is stable
/// when you leave and come back, and identical for the same seed.
///
/// A chunk is instanced when it overlaps the screen or touches (within NeighbourRadius)
/// one that does, and freed when it leaves that set.
/// </summary>
public partial class ChunkManager : Node2D
{
	public const int ChunkSize = 32;

	[Export] public ChunkDefinition[] Definitions = System.Array.Empty<ChunkDefinition>();
	[Export] public int Seed;

	/// <summary>Max chunks instanced per frame (the very first frame ignores this).</summary>
	[Export] public int MaxSpawnsPerFrame = 2;

	/// <summary>Must match the TileSet tile_size (isometric).</summary>
	[Export] public Vector2I TileSize = new(32, 16);

	/// <summary>Extra chunks kept loaded around the ones on screen (1 = direct neighbours).</summary>
	[Export] public int NeighbourRadius = 1;

	/// <summary>World-space padding when testing if a chunk is on screen (tall tiles stick out).</summary>
	[Export] public Vector2 ScreenPadding = new(64, 64);

	/// <summary>Same offset the old TileMapBackground node had.</summary>
	[Export] public Vector2 ChunkOffset = new(0, 1);

	private const ulong RollSalt = 1, PrioritySalt = 2, FillerSalt = 3;

	private readonly List<ChunkDefinition> _fillers = new();
	private readonly List<ChunkDefinition> _specials = new();
	private readonly Dictionary<Vector2I, ChunkDefinition> _rolls = new();  // first roll, before spacing
	private readonly Dictionary<Vector2I, ChunkDefinition> _final = new();  // result after spacing
	private readonly Dictionary<Vector2I, Node2D> _loaded = new();
	private HashSet<Vector2I> _wanted = new();
	private bool _firstUpdate = true;

	public override void _Ready()
	{
		foreach (ChunkDefinition d in Definitions)
		{
			if (d?.Scene == null) continue;
			(d.IsFiller ? _fillers : _specials).Add(d);
		}

		if (_fillers.Count == 0)
		{
			GD.PushError("ChunkManager needs at least one filler ChunkDefinition.");
			SetProcess(false);
		}
	}

	public override void _Process(double delta)
	{
		_wanted = ComputeWantedChunks();
		UnloadUnwanted();
		SpawnWanted(_firstUpdate); // first frame: spawn everything so there's no pop-in
		_firstUpdate = false;
	}

	// ------------------------------------------------------- which chunks are visible

	private HashSet<Vector2I> ComputeWantedChunks()
	{
		// Visible rect in this node's local space (works with any Camera2D / zoom).
		Rect2 view = GetGlobalTransformWithCanvas().AffineInverse() * GetViewportRect();

		// Range of chunks to test, derived from the view's corners in tile space.
		Vector2I min = new(int.MaxValue, int.MaxValue);
		Vector2I max = new(int.MinValue, int.MinValue);
		foreach (Vector2 corner in new[]
				 {
					 view.Position,
					 view.Position + new Vector2(view.Size.X, 0),
					 view.Position + new Vector2(0, view.Size.Y),
					 view.End,
				 })
		{
			Vector2I chunk = TileToChunk(LocalToTile(corner));
			min = new Vector2I(Mathf.Min(min.X, chunk.X), Mathf.Min(min.Y, chunk.Y));
			max = new Vector2I(Mathf.Max(max.X, chunk.X), Mathf.Max(max.Y, chunk.Y));
		}

		Rect2 paddedView = view.GrowIndividual(
			ScreenPadding.X, ScreenPadding.Y, ScreenPadding.X, ScreenPadding.Y);

		var wanted = new HashSet<Vector2I>();
		for (int cx = min.X; cx <= max.X; cx++)
		{
			for (int cy = min.Y; cy <= max.Y; cy++)
			{
				var c = new Vector2I(cx, cy);
				if (!ChunkWorldBounds(c).Intersects(paddedView)) continue;

				// On screen -> keep it and its neighbours.
				for (int dx = -NeighbourRadius; dx <= NeighbourRadius; dx++)
					for (int dy = -NeighbourRadius; dy <= NeighbourRadius; dy++)
						wanted.Add(c + new Vector2I(dx, dy));
			}
		}

		return wanted;
	}

	// ------------------------------------------------------------ spawn / unload

	private void SpawnWanted(bool unlimited)
	{
		int budget = unlimited ? int.MaxValue : MaxSpawnsPerFrame;
		foreach (Vector2I c in _wanted)
		{
			if (_loaded.ContainsKey(c)) continue;
			if (budget-- <= 0) break;
			Instantiate(c, GetDefinition(c).Scene);
		}
	}

	private void UnloadUnwanted()
	{
		List<Vector2I> toRemove = null;
		foreach (Vector2I c in _loaded.Keys)
		{
			if (_wanted.Contains(c)) continue;
			(toRemove ??= new List<Vector2I>()).Add(c);
		}

		if (toRemove == null) return;
		foreach (Vector2I c in toRemove)
		{
			_loaded[c].QueueFree();
			_loaded.Remove(c);
		}
	}

	private void Instantiate(Vector2I c, PackedScene scene)
	{
		var node = scene.Instantiate<Node2D>();
		node.Name = $"Chunk_{c.X}_{c.Y}";
		node.Position = TileToLocal(c * ChunkSize) + ChunkOffset;
		AddChild(node);
		_loaded[c] = node;
	}

	// ------------------------------------------------- which chunk type goes where
	// Everything below depends only on (Seed, coordinate); the dictionaries are caches.

	private ChunkDefinition GetDefinition(Vector2I c)
	{
		if (_final.TryGetValue(c, out ChunkDefinition def)) return def;

		def = Roll(c);
		if (!def.IsFiller && !WinsSpacing(c, def))
			def = Pick(c, FillerSalt, _fillers); // lost the spacing check -> demote to a filler

		_final[c] = def;
		return def;
	}

	/// <summary>What this coordinate wants to be, ignoring other chunks.</summary>
	private ChunkDefinition Roll(Vector2I c)
	{
		if (_rolls.TryGetValue(c, out ChunkDefinition cached)) return cached;

		int dist = Mathf.Max(Mathf.Abs(c.X), Mathf.Abs(c.Y));
		var pool = new List<ChunkDefinition>(_fillers); // fillers: always allowed
		foreach (ChunkDefinition s in _specials)
			if (dist >= s.MinDistanceFromOrigin) pool.Add(s); // specials: origin rule

		ChunkDefinition result = Pick(c, RollSalt, pool);
		_rolls[c] = result;
		return result;
	}

	/// <summary>c keeps its type only if no same-type roll within range has a higher priority.</summary>
	private bool WinsSpacing(Vector2I c, ChunkDefinition def)
	{
		int r = def.MinDistanceSameType;
		if (r <= 0) return true;

		ulong mine = Hash(c, PrioritySalt);
		for (int dx = -r; dx <= r; dx++)
		{
			for (int dy = -r; dy <= r; dy++)
			{
				if (dx == 0 && dy == 0) continue;
				var n = c + new Vector2I(dx, dy);
				if (Roll(n) == def && Hash(n, PrioritySalt) > mine) return false;
			}
		}

		return true;
	}

	private ChunkDefinition Pick(Vector2I c, ulong salt, List<ChunkDefinition> pool)
	{
		double total = 0;
		foreach (ChunkDefinition d in pool) total += d.Weight;

		double t = (Hash(c, salt) >> 11) * (1.0 / (1UL << 53)) * total; // [0, total)
		foreach (ChunkDefinition d in pool)
		{
			t -= d.Weight;
			if (t < 0) return d;
		}

		return pool[^1];
	}

	private ulong Hash(Vector2I c, ulong salt) =>
		Mix(Mix(Mix(Mix((ulong)Seed) + (ulong)(uint)c.X) + (ulong)(uint)c.Y) + salt);

	private static ulong Mix(ulong z) // splitmix64
	{
		z += 0x9E3779B97F4A7C15UL;
		z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
		z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
		return z ^ (z >> 31);
	}

	// ------------------------------------------------------------------ geometry

	/// <summary>Isometric tile -> local position (centre of the tile).</summary>
	private Vector2 TileToLocal(Vector2I t) =>
		new((t.X - t.Y) * TileSize.X * 0.5f, (t.X + t.Y) * TileSize.Y * 0.5f);

	/// <summary>Isometric local position -> (fractional) tile coordinate.</summary>
	private Vector2 LocalToTile(Vector2 p)
	{
		float sum = p.Y / (TileSize.Y * 0.5f);  // x + y
		float diff = p.X / (TileSize.X * 0.5f); // x - y
		return new Vector2((sum + diff) * 0.5f, (sum - diff) * 0.5f);
	}

	private static Vector2I TileToChunk(Vector2 tile) =>
		new(Mathf.FloorToInt(tile.X / ChunkSize), Mathf.FloorToInt(tile.Y / ChunkSize));

	/// <summary>Axis-aligned world bounds of a chunk's diamond (plus half a tile on each side).</summary>
	private Rect2 ChunkWorldBounds(Vector2I c)
	{
		Vector2I o = c * ChunkSize;
		Vector2 a = TileToLocal(o);
		Vector2 b = TileToLocal(o + new Vector2I(ChunkSize, 0));
		Vector2 d = TileToLocal(o + new Vector2I(0, ChunkSize));
		Vector2 e = TileToLocal(o + new Vector2I(ChunkSize, ChunkSize));

		float minX = Mathf.Min(Mathf.Min(a.X, b.X), Mathf.Min(d.X, e.X));
		float maxX = Mathf.Max(Mathf.Max(a.X, b.X), Mathf.Max(d.X, e.X));
		float minY = Mathf.Min(Mathf.Min(a.Y, b.Y), Mathf.Min(d.Y, e.Y));
		float maxY = Mathf.Max(Mathf.Max(a.Y, b.Y), Mathf.Max(d.Y, e.Y));

		return new Rect2(minX - TileSize.X * 0.5f, minY - TileSize.Y * 0.5f,
			maxX - minX + TileSize.X, maxY - minY + TileSize.Y);
	}
}
