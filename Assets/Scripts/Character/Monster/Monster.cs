using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	protected IObjectPool<Monster> m_Pool;
	protected NavMeshAgent m_NavAgent;
	protected Transform m_Player;
	protected GameObject m_TargetObj;
	protected bool m_VisibleTarget = false;
	private float m_RefreshRate = .1f;
	private WaitForSeconds waitForSeconds;

	public void SetVisibleTarget(bool visible)
	{
		m_VisibleTarget = visible;
	}

	public void SetPool(IObjectPool<Monster> pool)
	{
		m_Pool = pool;
	}

	public override void Destroy()
	{
		StopAllCoroutines();

		if (m_Pool != null)
			m_Pool.Release(this);
	}

	private IEnumerator UpdatePath()
	{
		Vector3 targetPos = Vector3.zero;

		while (m_Player)
		{
			targetPos.x = m_Player.position.x;
			targetPos.z = m_Player.position.z;

			m_NavAgent.SetDestination(targetPos);

			yield return waitForSeconds;
		}
	}

	private IEnumerator VisibleTarget()
	{
		while (true)
		{
			m_TargetObj.SetActive(m_VisibleTarget);

			yield return waitForSeconds;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_MoveSpeed = 1f;

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_MoveSpeed;

		m_Player = GameObject.FindGameObjectWithTag("Player").transform;

		waitForSeconds = new WaitForSeconds(m_RefreshRate);

		foreach (Transform target in transform)
		{
			if (target.CompareTag("Target"))
			{
				m_TargetObj = target.gameObject;
				break;
			}
		}

		StartCoroutine(UpdatePath());
		StartCoroutine(VisibleTarget());
	}
}
