using UnityEngine.Pool;

public class MeleeMonster : Monster
{
	private IObjectPool<MeleeMonster> m_Pool;

	public void SetPool(IObjectPool<MeleeMonster> pool)
	{
		m_Pool = pool;
	}

	protected override void Awake()
	{
		m_CharInfo = InfoManager.Clone(Character_Type.Melee);

		base.Awake();
	}

	protected override void Destroy()
	{
		base.Destroy();

		if (m_Pool != null)
			m_Pool.Release(this);
	}
}
