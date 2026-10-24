using Godot;
using System;

public partial class TileMapBackground : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var filledTiles = GetUsedCells();

		foreach (Vector2I cell in filledTiles)
		{
			var neighboringTiles = GetSurroundingCells(cell);

			foreach (Vector2I neighbor in neighboringTiles)
			{
				if (GetCellSourceId(neighbor) == -1)
				{
					SetCell(neighbor, 0, Vector2I.Zero);
				}
			}

		}
	}

}
