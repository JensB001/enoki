using System;

namespace Enoki.gui.menus.code_window.blocks;

public static class BlockFactory {

	public static Block CreateRandom() {
		var attributes = Enum.GetValues<AttributeType>();

		return Random.Shared.Next(4) switch {
			0 => new NumberBlock { Value = Random.Shared.Next(1, 6) },
			1 => new AttributeBlock { Value = attributes[Random.Shared.Next(attributes.Length)] },
			2 => new AdderBlock(),
			_ => new MultiplierBlock(),
		};
	}
}
