
using UnityEngine;

public interface IDamageable
{
	bool IsDead();
	void TakeDamage(float dmg, Vector3 hitPoint, bool isMelee, bool isCheat = false);
}