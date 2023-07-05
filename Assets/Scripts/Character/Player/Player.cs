using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	private Vector3 m_Dir;
	private Vector3 m_TargetDir;
	private Vector3 m_Velocity;
	private bool m_CanAttack;
	private bool m_Move;
	private bool m_UseTargetRot;
	private float m_Timer = .1f;
	private float m_CheckTimer = .1f;

	public bool IsMove { get { return m_Move; } }

	public void SetSpawnInfo(Bullet_Type type, float fireRateTime, float dmg)
	{
		m_Spawner.SetSpawnInfo(type, fireRateTime, dmg);
	}

	private IEnumerator CheckNearMonster()
	{
		LinkedList<Monster> monsters;
		float result, dist;
		bool IsEmpty;

		while (true)
		{
			IsEmpty = StageManager.IsEnemyEmpty;

			m_CanAttack = !IsEmpty;

			m_Timer += m_deltaTime;

			if (m_Timer >= m_CheckTimer)
			{
				m_Timer = 0f;

				if (m_CanAttack)
				{
					monsters = StageManager.GetActiveMonsters();
					result = float.MaxValue;
					dist = 0f;

					foreach (Monster target in monsters)
					{
						//Distance는 무거우므로 SqrMagnitude로 교체
						dist = (m_Rig.position - target.Pos).sqrMagnitude;

						if (result > dist)
						{
							result = dist;
							m_TargetDir = (target.Pos - m_Rig.position).normalized;
							m_TargetDir.y = 0f;

							StageManager.SetVisibleTarget(target);
						}
					}
				}
			}

			yield return null;
		}
	}

	public void Shoot()
	{
		// 플레이어 공격 애니메이션 프레임에 맞춰서 동작해야 한다

		// 멈춘 후 m_UseTargetRot를 이용하여 회전을 완료하면 총알을 쏘게 한다.
		if (m_CanAttack && !m_Move)
		{
			if (m_UseTargetRot)
				m_Spawner.Attack(true);
		}

		else
			m_Spawner.Attack(false);
	}

	public void Rotation(float rotSpeed, Vector3 dir)
	{
		if (m_Move)
		{
			m_UseTargetRot = false;

			if (dir != Vector3.zero)
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * m_fixedDeltaTime);
		}

		else
		{
			if (m_TargetDir != Vector3.zero)
			{
				transform.rotation = Quaternion.LookRotation(m_TargetDir);

				m_UseTargetRot = true;
			}
		}
	}

	public void Move(Vector3 velocity)
	{
		m_Velocity = velocity;

		m_Move = m_Velocity == Vector3.zero ? false : true;
	}

	protected override void Destroy()
	{
		base.Destroy();

		if (gameObject)
			Destroy(transform.root.gameObject);
	}

	protected override void Awake()
	{
		base.Awake();

		SetSpawnInfo(Bullet_Type.Player, m_FireRateTime, m_Damage);
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(CheckNearMonster());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// 방향
		Rotation(m_RotSpeed, m_Dir);

		m_Rig.MovePosition(m_Rig.position + m_Velocity * m_fixedDeltaTime);
	}

	protected override void BeforeUpdate()
	{
		// 이동
		m_Dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		m_Velocity = m_Dir * m_MoveSpeed;

		Move(m_Velocity);
		Shoot();
	}
}
