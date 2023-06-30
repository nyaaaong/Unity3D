﻿using UnityEngine.Pool;

public class MeleeMonster : Monster
{
	protected IObjectPool<MeleeMonster> m_Pool;

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public void SetPool(IObjectPool<MeleeMonster> pool)
	{
		m_Pool = pool;
	}

	protected override void Destroy()
	{
		base.Destroy();

		if (m_Pool != null)
			m_Pool.Release(this);
	}
}