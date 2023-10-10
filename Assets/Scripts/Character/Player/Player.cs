using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	private Vector3 m_Dir;
	private Vector3 m_TargetDir;
	private Vector3 m_Velocity;
	private Vector3 m_Input;
	private Monster m_Target;
	private bool m_CanAttack;
	private bool m_Move;
	private bool m_UseTargetRot;
	private bool m_InputLock;
	private float m_AttackTimer = 0f;
	private float m_TargetDist;
	private WaitForSeconds m_CheckNearMonsterTime = new WaitForSeconds(.3f);
	private FloatingJoystick m_Joystick;

	public bool IsMove => m_Move;

	public void AddDamage(float value)
	{
		m_CharData.AddDamage(value);
	}

	public void AddFireRate(float value)
	{
		m_CharData.AddFireRate(value);
	}

	public void Speed(float value)
	{
		m_CharData.Speed(value);
	}

	public void MultiShot()
	{
		m_CharData.MultiShot();
	}

	public void Cheat(Cheat_Type type, bool isCheck)
	{
		switch (type)
		{
			case Cheat_Type.PowerUp:
				if (m_CharData.PowerUp != isCheck)
					m_CharData.PowerUp = isCheck;
				break;
			case Cheat_Type.NoHit:
				if (m_CharData.NoHit != isCheck)
					m_CharData.NoHit = isCheck;
				break;
			case Cheat_Type.Death:
				if (isCheck)
					Kill();
				break;
		}
	}

	// 공격 애니메이션용
	private void Attack()
	{
		m_Spawner.Attack();
	}

	// 플레이어의 사정거리에 닿는 몬스터들 중, 가장 가까이에 있는 몬스터를 타겟으로 삼는다. 단 죽어있는 경우는 제외해야 한다.
	private IEnumerator CheckNearMonster()
	{
		LinkedList<Monster> monsters;
		float result;
		Vector3 targetDir = Vector3.zero;

		while (!m_Dead)
		{
			if (m_Target)
			{
				if (m_Target.IsDead() || Vector3.Distance(SpawnerPos, m_Target.Pos) > m_CharData.Range)
				{
					StageManager.SetInvisibleTarget(m_Target);
					m_Target = null;
				}
			}

			m_CanAttack = !StageManager.IsMonsterEmpty;

			if (m_CanAttack)
			{
				monsters = StageManager.GetActiveMonsters();
				result = float.MaxValue;
				m_TargetDist = 0f;

				foreach (Monster target in monsters)
				{
					if (!target.IsEnabled)
						continue;

					m_TargetDist = Vector3.Distance(SpawnerPos, target.Pos);

					if (m_TargetDist > m_CharData.Range)
						continue;

					// 가장 가까운 몬스터를 타겟으로 지정한다.
					if (result > m_TargetDist)
					{
						result = m_TargetDist;

						StageManager.SetVisibleTarget(target);

						targetDir = (target.Pos - SpawnerPos).normalized;
						targetDir.y = 0f;

						m_Target = target;
						m_TargetDir = targetDir;
					}
				}
			}

			yield return m_CheckNearMonsterTime;
		}

		StageManager.SetInvisibleTarget(m_Target);

		m_Target = null;
	}

	private IEnumerator AttackAnim()
	{
		// 멈춘 후 m_UseTargetRot를 이용하여 회전을 완료하면 총알을 쏘게 한다.
		// 타겟이 죽어있으면 발사하지 않는다.
		while (!m_Dead)
		{
			if (m_CanAttack && !m_Move && m_UseTargetRot && m_Target && !m_Target.IsDead())
			{
				m_AttackTimer += Time.deltaTime;

				if (m_AttackTimer >= m_CharData.FireRateTime)
				{
					m_AttackTimer = 0f;

					SetAnimType(Anim_Type.Attack);
				}
			}

			else
				m_AttackTimer = m_CharData.FireRateTime;

			yield return null;
		}
	}

	private void Rotation(float rotSpeed, Vector3 dir)
	{
		// 이동할 때에는 부드럽게 회전해야 한다
		if (m_Move)
		{
			m_UseTargetRot = false;

			if (dir != Vector3.zero)
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Mathf.Clamp(rotSpeed * Time.fixedDeltaTime, 0f, 1f));
		}

		// 이동하지 않으면 즉시 회전하여 몬스터를 바라본다
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
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Mathf.Clamp(rotSpeed * Time.fixedDeltaTime, 0f, 1f));
			}
		}
	}

	private void Move()
	{
		m_Velocity = m_Dir * m_CharData.MoveSpeed;

		m_Move = m_Velocity == Vector3.zero ? false : true;

		if (m_Move)
			SetAnimType(Anim_Type.Move);

		else if (!m_CanAttack || !m_Target)
			SetAnimType(Anim_Type.Idle);
	}

	public override void Destroy()
	{
		base.Destroy();

		Destroy(gameObject);
	}

	private void InputLock()
	{
		m_InputLock = true;
	}

	private void InputUnlock()
	{
		m_InputLock = false;
	}

	protected override void Awake()
	{
		base.Awake();

		DebugManager.SetPlayerData(m_CharData);

		m_CharClip = AudioManager.PlayerClip;

		UIManager.AddShowMenuEvent(InputLock);
		UIManager.AddHideMenuEvent(InputUnlock);

		m_Joystick = UIManager.Joystick;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (!m_Quit)
		{
			UIManager.RemoveShowMenuEvent(InputLock);
			UIManager.RemoveHideMenuEvent(InputUnlock);
		}
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(CheckNearMonster());
		StartCoroutine(AttackAnim());

		UIManager.UpdateExp();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// 방향
		Rotation(m_RotSpeed, m_Dir);
		m_Rig.MovePosition(m_Rig.position + m_Velocity * Time.fixedDeltaTime);
	}

	private void InputUpdate()
	{
		if (m_InputLock)
			m_Dir = Vector3.zero;

		else
		{
			m_Input.x = m_Joystick.Horizontal;
			m_Input.z = m_Joystick.Vertical;

			if (m_Input == Vector3.zero)
				m_Input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

			if (UIManager.IsShowAbility)
				m_Input = Vector3.zero;

			m_Dir = !m_Dead ? m_Input : Vector3.zero;

			if (Input.GetKeyDown(KeyCode.Escape))
				UIManager.ShowMenu(Menu_Type.Option);
		}
	}

	protected override void Update()
	{
		base.Update();

		InputUpdate();
		Move();
	}
}
