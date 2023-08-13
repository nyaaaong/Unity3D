public interface IDamageable
{
	bool IsDead();
	void TakeDamage(float dmg, bool isCheat = false);
}