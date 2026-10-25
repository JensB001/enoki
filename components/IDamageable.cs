public record DamageInfo(float Physical, float Fire, float Ice);

public interface IDamageable {
	void TakeDamage(DamageInfo damage);
}
