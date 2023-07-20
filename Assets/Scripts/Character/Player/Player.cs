using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	private Vector3 m_Dir;
	private Vector3 m_TargetDir;
	private Vector3 m_Velocity;
	private Monster m_Target;
	private bool m_CanAttack;
	private bool m_Move;
	private bool m_UseTargetRot;
	private float m_AttackTimer = 0f;
	private float m_CanAttackTimer = .1f;
	private float m_CanAttackCheckTimer = .1f;
	private float m_TargetDist;

	public bool IsMove { get { return m_Move; } }

	// ���� �ִϸ��̼ǿ�
	private void Attack()
	{
		m_Spawner.Attack();
	}

	public void SetSpawnerInfo(Character owner, Character_Type type)
	{
		m_Spawner.SetSpawnerInfo(owner, type);
	}

	private IEnumerator CheckNearMonster()
	{
		LinkedList<Monster> monsters;
		float result;
		bool IsEmpty;

		while (!m_Dead)
		{
			IsEmpty = StageManager.IsEnemyEmpty;

			m_CanAttack = !IsEmpty;

			m_CanAttackTimer += m_deltaTime;

			if (m_CanAttackTimer >= m_CanAttackCheckTimer)
			{
				m_CanAttackTimer = 0f;

				if (m_CanAttack)
				{
					Vector3 targetDir = Vector3.zero;

					monsters = StageManager.GetActiveMonsters();
					result = float.MaxValue;
					m_TargetDist = 0f;

					foreach (Monster target in monsters)
					{
						m_TargetDist = Vector3.Distance(SpawnerPos, target.Pos);

						targetDir = (target.Pos - SpawnerPos).normalized;
						targetDir.y = 0f;

						// ���Ϳ� �÷��̾� ������ ���� üũ�ϰ�, ���� ���̶�� �ٸ� ���͸� üũ�ϰ� �Ѵ�.
						bool IsWall = Physics.Raycast(new Ray(SpawnerPos, targetDir), m_TargetDist, m_WallMask, QueryTriggerInteraction.Collide);

						if (IsWall)
						{
							if (m_Target == target)
							{
								m_Target = null;

								StageManager.SetInvisibleTarget(target);
							}

							continue;
						}

						// ���� ����� ���͸� Ÿ������ �����Ѵ�.
						else if (result > m_TargetDist)
						{
							result = m_TargetDist;

							StageManager.SetVisibleTarget(target);

							m_Target = target;
							m_TargetDir = targetDir;
						}
					}
				}
			}

			yield return null;
		}

		StageManager.SetInvisibleTarget(m_Target);

		m_Target = null;
	}

	private void AttackAnim()
	{
		// ���� �� m_UseTargetRot�� �̿��Ͽ� ȸ���� �Ϸ��ϸ� �Ѿ��� ��� �Ѵ�.
		// Ÿ���� �׾������� �߻����� �ʴ´�.
		if (m_CanAttack && !m_Move && m_UseTargetRot)
		{
			if (m_Target)
			{
				if (!m_Target.IsDead())
				{
					m_AttackTimer += m_deltaTime;

					if (m_AttackTimer >= m_CharInfo.FireRateTime)
					{
						m_AttackTimer = 0f;

						SetAnimType(Animation_Type.Attack);
						return;
					}
				}
			}
		}

		else
			m_AttackTimer = m_CharInfo.FireRateTime;

		SetAnimType(Animation_Type.Idle);
	}

	private void Rotation(float rotSpeed, Vector3 dir)
	{
		if (m_Move)
		{
			m_UseTargetRot = false;

			if (dir != Vector3.zero)
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Mathf.Clamp(rotSpeed * m_fixedDeltaTime, 0f, 1f));
		}

		else
		{
			if (m_Target)
			{
				if (m_TargetDir != Vector3.zero)
					transform.rotation = Quaternion.LookRotation(m_TargetDir);

				m_UseTargetRot = true;
			}

			else
			{
				m_UseTargetRot = false;

				if (dir != Vector3.zero)
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Mathf.Clamp(rotSpeed * m_fixedDeltaTime, 0f, 1f));
			}
		}
	}

	private void Move(Vector3 velocity)
	{
		m_Velocity = velocity;

		m_Move = m_Velocity == Vector3.zero ? false : true;

		if (m_Move || !m_CanAttack)
			SetAnimType(Animation_Type.Move);
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

		m_CharInfo = InfoManager.Clone(Character_Type.Player);

		SetSpawnerInfo(this, Character_Type.Player);

		DebugManager.SetPlayerInfo(m_CharInfo);
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(CheckNearMonster());
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// ����
		Rotation(m_RotSpeed, m_Dir);

		m_Rig.MovePosition(m_Rig.position + m_Velocity * m_fixedDeltaTime);
	}

	protected override void BeforeUpdate()
	{
		// �̵�

		m_Dir = !m_Dead ? new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")) : Vector3.zero;
		m_Velocity = m_Dir * m_CharInfo.MoveSpeed;

		Move(m_Velocity);
		AttackAnim();
	}
}
