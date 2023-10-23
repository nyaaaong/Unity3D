public class Boss1 : Boss
{
	// 각 패턴의 지속시간
	private float m_MultiAttackDur = 3f; // MultiAttack가 언제 끝날지
	private float m_MultiAttackLoopDelay = 0.7f; // MultiAttack의 간격

	private float m_DoubleAttackDur = 1f;
	private float m_DoubleAttackLoopDelay = 0.3f;

	private float m_AllAttackDur = 7f;
	private float m_AllAttackLoopDelay = 0.7f;

	protected override void PatternEndEvent()
	{
		m_NavUpdate = true;

		RemoveAllBulletAngle();

		NavMoveSpeed = MoveSpeed;
	}

	private void MultiAttackInit()
	{
		AddBulletAngle(0f);
		AddBulletAngle(30f);
		AddBulletAngle(-30f);
	}

	private void DoubleAttackInit()
	{
		AddBulletAngle(15f);
		AddBulletAngle(-15f);
	}

	private void AllAttackInit()
	{
		m_NavUpdate = false;

		for (int i = 0; i <= 180; i += 20)
		{
			AddBulletAngle(i);

			if (i == 0 || i == 180)
				continue;

			AddBulletAngle(-(float)i);
		}
	}

	private void AttackLoop()
	{
		m_Spawner.AttackEvent();
	}

	protected override void Awake()
	{
		base.Awake();

		m_UseNavSystem = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AddPattern(40f, m_MultiAttackDur, MultiAttackInit, AttackLoop, m_MultiAttackLoopDelay);
		AddPattern(20f, m_DoubleAttackDur, DoubleAttackInit, AttackLoop, m_DoubleAttackLoopDelay);
		AddPattern(10f, m_AllAttackDur, AllAttackInit, AttackLoop, m_AllAttackLoopDelay);
	}
}
