using System;
using System.Collections.Generic;

namespace Enoki.gui.menus.code_window.blocks;

public static class BlockFactory 
{
	public static Rarity RollRarity()
	{
		float roll = Random.Shared.NextSingle();
		if (roll < 0.65f) return Rarity.Common;
		if (roll < 0.90f) return Rarity.Uncommon;
		return Rarity.Rare;
	}

	private static NumberBlock Number(int min, int max) => 
		new() {Value = Random.Shared.Next(min, max + 1)};

	private static AttributeBlock Attribute()
	{
		var all = Enum.GetValues<AttributeType>();
		return new() { Value = all[Random.Shared.Next(all.Length)] };
	}

	private static readonly Dictionary<Rarity, Func<Block>[]> Pools = new()
    {
        [Rarity.Common] =
        [
            () => Number(1, 3),
            () => Attribute(),
        ],
        [Rarity.Uncommon] =
        [
            () => Number(4, 5),
            () => new AdderBlock()
        ],
        [Rarity.Rare] =
        [
            () => Number(6, 9),
            () => new MultiplierBlock(),
        ],
    };

	public static Block CreateRandom(Rarity rarity) {
		var pool = Pools[rarity];
		return pool[Random.Shared.Next(pool.Length)]();
	}

	public static Block CreateRandom() => CreateRandom(RollRarity());
}
