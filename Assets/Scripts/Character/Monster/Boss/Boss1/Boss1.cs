
using UnityEngine;

public class Boss1 : Boss
{
	// 각 패턴의 지속시간
	private float m_MultiAttackDur = 3f; // MultiAttack가 언제 끝날지
	private float m_MultiAttackLoopDelay = 0.7f; // MultiAttack의 간격

	private float m_AllAttackDur = 7f;
	private float m_AllAttackLoopDelay = 0.7f;

	private float m_DashSpeed; // 기본 이동 속도 * Dash배율
	private Vector3 m_DashDir; // 대쉬 직전 방향
	private float m_DashMultiplier = 10f;
	private float m_DashDurTime = 0.3f;
	private bool m_Dash;

	protected override void PatternEndEvent()
	{
		m_NavUpdate = true;
		m_Dash = false;

		RemoveAllBulletAngle();

		NavMoveSpeed = MoveSpeed;
	}

	private void MultiAttackInit()
	{
		AddBulletAngle(0f);
		AddBulletAngle(30f);
		AddBulletAngle(-30f);
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

	private void DashInit()
	{
		m_NavUpdate = false;
		m_Dash = true;

		m_DashDir = transform.forward;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (m_Dash)
			m_Rig.MovePosition(m_Rig.position + m_DashDir * m_DashSpeed * Time.fixedDeltaTime);
	}

	protected override void Awake()
	{
		base.Awake();

		m_UseNavSystem = true;

		m_DashSpeed = MoveSpeed * m_DashMultiplier;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AddPattern(40f, m_MultiAttackDur, MultiAttackInit, AttackLoop, m_MultiAttackLoopDelay);
		AddPattern(10f, m_AllAttackDur, AllAttackInit, AttackLoop, m_AllAttackLoopDelay);
		AddPattern(15f, m_DashDurTime, DashInit);
	}
}
