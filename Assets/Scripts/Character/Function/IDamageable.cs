
using UnityEngine;

public interface IDamageable
{
	void TakeDamage(float dmg);
	void TakeHit(float dmg, RaycastHit hit);
}