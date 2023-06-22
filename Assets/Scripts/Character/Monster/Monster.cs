using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	[SerializeField] protected GameObject m_TargetObj;

	protected NavMeshAgent m_NavAgent;
	protected Transform m_Player;
	protected WaitForSeconds m_UpdateTime = new WaitForSeconds(.1f);
	protected bool m_VisibleTarget;

	public void SetVisibleTarget(bool visible)
	{
		m_VisibleTarget = visible;
	}

	public override void Destroy()
	{
		StopAllCoroutines();
	}

	protected virtual IEnumerator UpdatePath()
	{
		Vector3 targetPos = Vector3.zero;

		while (true)
		{
			if (!StageManager.IsPlayerDeath)
			{
				targetPos.x = m_Player.position.x;
				targetPos.z = m_Player.position.z;

				m_NavAgent.SetDestination(targetPos);

				targetPos = (targetPos - transform.position).normalized;
				targetPos.y = 0f;

				if (targetPos != Vector3.zero)
					transform.rotation = Quaternion.LookRotation(targetPos);
			}

			yield return null;
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

	protected override void OnEnable()
	{
		base.OnEnable();

		m_MoveSpeed = 1f;

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_MoveSpeed;
		m_NavAgent.updateRotation = false; // 회전 업데이트 속도가 너무 느리므로 비활성화 후 코루틴에서 회전을 업데이트 하게 한다.

		if (!m_TargetObj)
			Debug.LogError("if (!m_TargetObj)");

		m_Player = StageManager.GetPlayer.transform;

		StartCoroutine(UpdatePath());
		StartCoroutine(VisibleTarget());
	}
}
