using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GunController))]
public class PlayerController : BaseScript
{
	private Rigidbody m_Rig;
	private Vector3 m_Velocity;
	private GunController m_GunController;
	private Vector3 m_TargetDir;
	private bool m_CanAttack = false;
	private bool m_Move = false;
	private bool m_UseTargetRot = false;
	private float m_Timer = .1f;
	private float m_CheckTimer = .1f;

	public bool IsMove { get { return m_Move; } }

	private IEnumerator CheckNearMonster()
	{
		LinkedList<Monster> monsters;
		float result = 0f, dist = 0f;
		bool IsEmpty = false;

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
						dist = Vector3.Distance(transform.position, target.transform.position);

						if (result > dist)
						{
							result = dist;
							m_TargetDir = (target.transform.position - transform.position).normalized;
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
				m_GunController.Shoot(true);
		}

		else
			m_GunController.Shoot(false);
	}

	public void Rotation(float rotSpeed, Vector3 dir)
	{
		Quaternion rot;

		if (m_Move)
		{
			m_UseTargetRot = false;

			if (dir != Vector3.zero)
			{
				rot = Quaternion.LookRotation(dir);

				transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotSpeed * m_fixedDeltaTime);
			}
		}

		else
		{
			if (m_TargetDir != Vector3.zero)
			{
				rot = Quaternion.LookRotation(m_TargetDir);

				transform.rotation = rot;

				m_UseTargetRot = true;
			}
		}
	}

	public void Move(Vector3 velocity)
	{
		m_Velocity = velocity;

		m_Move = m_Velocity == Vector3.zero ? false : true;
	}

	protected override void Awake()
	{
		m_Rig = GetComponent<Rigidbody>();

		m_GunController = GetComponent<GunController>();
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(CheckNearMonster());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		m_Rig.MovePosition(m_Rig.position + m_Velocity * m_fixedDeltaTime);
	}
}
