
using UnityEngine;

public interface IDamageable
{
	bool IsDead();
	void TakeDamage(float dmg);
	void TakeHit(float dmg, RaycastHit hit);
}