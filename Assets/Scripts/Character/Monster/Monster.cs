using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	public enum Monster_Type
	{
		Melee,
		Range
	}

	protected NavMeshAgent m_NavAgent;
	protected Transform m_Player;
	protected GameObject m_TargetObj;
	protected Monster_Type m_Type = Monster_Type.Melee;
	protected bool m_VisibleTarget = false;
	protected float m_RefreshRate = .05f;
	protected WaitForSeconds m_UpdateTime;

	public void SetVisibleTarget(bool visible)
	{
		m_VisibleTarget = visible;
	}

	public override void Destroy()
	{
		StopAllCoroutines();
	}

	protected virtual IEnumerator CheckDist()
	{
		yield return null;
	}

	protected virtual IEnumerator UpdatePath()
	{
		Vector3 targetPos = Vector3.zero;

		while (true)
		{
			targetPos.x = m_Player.position.x;
			targetPos.z = m_Player.position.z;

			m_NavAgent.SetDestination(targetPos);

			transform.rotation = Quaternion.LookRotation(targetPos);

			yield return m_UpdateTime;
		}
	}

	private IEnumerator VisibleTarget()
	{
		while (true)
		{
			m_TargetObj.SetActive(m_VisibleTarget);

			yield return m_UpdateTime;
		}
	}

	protected virtual IEnumerator Attack()
	{
		yield return null;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_MoveSpeed = 1f;

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_MoveSpeed;
		m_NavAgent.updateRotation = false; // 회전 업데이트 속도가 너무 느리므로 비활성화 후 코루틴에서 회전을 업데이트 하게 한다.

		m_Player = StageManager.GetPlayer.transform;

		m_UpdateTime = new WaitForSeconds(m_RefreshRate);

		foreach (Transform target in transform)
		{
			if (target.CompareTag("Target"))
			{
				m_TargetObj = target.gameObject;
				break;
			}
		}

		StartCoroutine(CheckDist());
		StartCoroutine(UpdatePath());
		StartCoroutine(VisibleTarget());
	}
}
