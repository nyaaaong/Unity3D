using System;
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
	private float m_TargetDist;
	private FloatingJoystick m_Joystick;
	private float m_BulletAngle = 15f;
	private int m_BulletUpgrade;
	private int m_BulletUpgradeMax = 6;
	private event Action OnLevelUpEvent;
	private bool m_Update;
	private float m_RotTime;
	private float m_RotTimeMax = 0.5f;
	private Vector3 m_Dist;
	private List<float> m_BulletAngles;

	public bool IsMove => m_Move;
	public bool IsUpdate => m_Update;

	public override void DieAnim()
	{
		base.DieAnim();

		m_Update = false;
	}

	public void LevelUpEvent()
	{
		if (OnLevelUpEvent != null)
			OnLevelUpEvent();
	}

	private void RefreshExpMax()
	{
		DataManager.RefreshPlayerExpMax();
	}

	private void AddLevel()
	{
		DataManager.AddPlayerLevel();
	}

	private void ResetLevel()
	{
		DataManager.ResetPlayerLevel();
	}

	public void AddDamage(float value)
	{
		m_CharData.AddDamage(value);
	}

	public void AddFireRate(float value)
	{
		m_CharData.AddFireRate(value);

		UpdateFireRate();
	}

	public void AddMoveSpeed(float value)
	{
		m_CharData.AddMoveSpeed(value);
	}

	public void MultiShot(int value)
	{
		m_CharData.MultiShot(value);

		AddAttackCount();

		if (m_BulletUpgrade < m_BulletUpgradeMax)
		{
			if (m_CharData.BulletCount % 2 == 1)
			{
				++m_BulletUpgrade;
				RemoveAttackCount(2);

				AddBulletAngle(0);
				AddBulletAngle(m_BulletAngle * m_BulletUpgrade);
				AddBulletAngle(m_BulletAngle * -m_BulletUpgrade);
			}
		}
	}

	private void SaveData()
	{
		GetBulletAngles(m_BulletAngles);

		DataManager.SavePlayerData(m_BulletUpgrade, GetAttackCount(), m_BulletAngles);
	}

	private void LoadData()
	{
		DataManager.PlayerSaveData data = DataManager.LoadPlayerData();

		// 게임을 처음 시작하는 경우에는 제외
		if (data == null)
			return;

		m_BulletUpgrade = data.BulletUpgrade;
		SetAttackCount(data.AttackCount);
		SetBulletAngles(data.BulletAngles);

		UpdateFireRate();
	}

	private void UpdateFireRate()
	{
		m_Anim.SetFloat("FireRate", m_CharData.FireRateTime);
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
		m_Spawner.AttackEvent();
	}

	// 플레이어의 사정거리에 닿는 몬스터들 중, 가장 가까이에 있는 몬스터를 타겟으로 삼는다. 단 죽어있는 경우는 제외해야 한다.
	private IEnumerator CheckNearMonster()
	{
		LinkedList<Monster> monsters;
		float result;

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
					if (!target.IsUpdate || target.IsDead())
						continue;

					m_TargetDist = Vector3.Distance(SpawnerPos, target.Pos);

					if (m_TargetDist > m_CharData.Range)
						continue;

					// 가장 가까운 몬스터를 타겟으로 지정한다.
					if (result > m_TargetDist)
					{
						result = m_TargetDist;

						StageManager.SetVisibleTarget(target);

						m_Target = target;
					}
				}
			}

			yield return null;
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
				SetAnimType(Anim_Type.Attack);

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
				m_Dist = m_Target.Pos - SpawnerPos;
				// 만약 근접한 경우 도리도리를 방지하기 위해 시간이 될때마다 회전하게 한다.
				if (Vector3.SqrMagnitude(m_Dist) <= 1f)
				{
					m_RotTime += Time.deltaTime;

					if (m_RotTime >= m_RotTimeMax)
					{
						m_UseTargetRot = true;

						m_RotTime = 0f;

						m_TargetDir = m_Dist.normalized;
						m_TargetDir.y = 0f;

						transform.rotation = Quaternion.LookRotation(m_TargetDir);
					}
				}

				else
				{
					m_UseTargetRot = true;

					m_RotTime = 0f;

					m_TargetDir = (m_Target.Pos - SpawnerPos).normalized;
					m_TargetDir.y = 0f;

					transform.rotation = Quaternion.LookRotation(m_TargetDir);
				}
			}

			else
			{
				m_UseTargetRot = false;
				m_RotTime = 0f;

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

		m_CharClip = AudioManager.PlayerClip;
		m_Joystick = UIManager.Joystick;
		m_BulletAngles = new List<float>();

		UIManager.AddShowMenuEvent(InputLock);
		UIManager.AddHideMenuEvent(InputUnlock);

		m_HitClip = AudioManager.EffectClip.PlayerHit;
		m_MeleeHitClip = AudioManager.EffectClip.MeleeHit;
		m_Rig.drag = Mathf.Infinity;

		OnLevelUpEvent += AddLevel;
		OnLevelUpEvent += () => Heal(0.2f);
		OnLevelUpEvent += RefreshExpMax;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		UIManager.RemoveShowMenuEvent(InputLock);
		UIManager.RemoveHideMenuEvent(InputUnlock);
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(CheckNearMonster());
		StartCoroutine(AttackAnim());

		RefreshExpMax();
		LoadData();

		m_Update = true;
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
				m_Input.Set(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

			if (UIManager.IsShowAbility)
				m_Input = Vector3.zero;

			m_Dir = !m_Dead ? m_Input : Vector3.zero;
		}
	}

	protected override void Update()
	{
		base.Update();

		InputUpdate();
		Move();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		SaveData();
		UIManager.ResetExp();
		ResetLevel();
	}
}
