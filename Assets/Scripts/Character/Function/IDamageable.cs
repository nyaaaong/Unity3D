
using UnityEngine;

public interface IDamageable
{
	void TakeHit(float dmg);
	void TakeHit(float dmg, RaycastHit hit);
}