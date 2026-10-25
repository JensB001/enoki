using System.Collections.Generic;
using System.Linq;

namespace Enoki.gui.menus.code_window.blocks;

// Plain data produced by evaluating a block tree. No Godot dependencies, so player/bullet can use it.
public class BlockValue {

	public float? Scalar { get; private init; }
	public Dictionary<AttributeType, float> Attributes { get; } = new();
	public bool IsScalar => Scalar.HasValue;

	public static BlockValue FromScalar(float value) => new() { Scalar = value };

	public static BlockValue FromAttribute(AttributeType type, float amount = 1) {
		var result = new BlockValue();
		result.Attributes[type] = amount;
		return result;
	}

	public static BlockValue Add(BlockValue a, BlockValue b) {
		if (a == null) return b;
		if (b == null) return a;

		if (a.IsScalar && b.IsScalar) return FromScalar(a.Scalar!.Value + b.Scalar!.Value);
		if (a.IsScalar) return b.WithScalarAdded(a.Scalar!.Value);
		if (b.IsScalar) return a.WithScalarAdded(b.Scalar!.Value);

		var result = a.Clone();
		foreach (var (type, amount) in b.Attributes) {
			result.Attributes[type] = result.Attributes.GetValueOrDefault(type) + amount;
		}
		return result;
	}

	public static BlockValue Multiply(BlockValue a, BlockValue b) {
		if (a == null) return b;
		if (b == null) return a;

		if (a.IsScalar && b.IsScalar) return FromScalar(a.Scalar!.Value * b.Scalar!.Value);
		if (a.IsScalar) return b.WithScalarMultiplied(a.Scalar!.Value);
		if (b.IsScalar) return a.WithScalarMultiplied(b.Scalar!.Value);

		return a.Clone(); // attribute × attribute isn't defined (yet)
	}

	// A bare number attaches to Bullet if there is one, otherwise to the first attribute
	private BlockValue WithScalarAdded(float scalar) {
		var result = Clone();
		if (result.Attributes.Count == 0) return FromScalar(scalar);

		var primary = result.Attributes.ContainsKey(AttributeType.Bullet)
			? AttributeType.Bullet
			: result.Attributes.Keys.First();
		result.Attributes[primary] += scalar;
		return result;
	}

	private BlockValue WithScalarMultiplied(float scalar) {
		var result = new BlockValue();
		foreach (var (type, amount) in Attributes) {
			result.Attributes[type] = amount * scalar;
		}
		return result;
	}

	private BlockValue Clone() {
		var result = new BlockValue { Scalar = Scalar };
		foreach (var (type, amount) in Attributes) {
			result.Attributes[type] = amount;
		}
		return result;
	}
}
