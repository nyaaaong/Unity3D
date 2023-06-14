using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : Character
{
	protected IObjectPool<Monster> m_Pool;
	protected NavMeshAgent m_NavAgent;
	protected Transform m_Target;
	private float m_RefreshRate = .25f;
	private WaitForSeconds waitForSeconds;

	public void SetPool(IObjectPool<Monster> pool)
	{
		m_Pool = pool;
	}

	public void Destroy()
	{
		StopAllCoroutines();

		m_Pool.Release(this);
	}

	private IEnumerator UpdatePath()
	{
		Vector3 targetPos = Vector3.zero;

		while (m_Target)
		{
			targetPos.x = m_Target.position.x;
			targetPos.z = m_Target.position.z;

			m_NavAgent.SetDestination(targetPos);

			yield return waitForSeconds;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_MoveSpeed = 1f;

		m_NavAgent = GetComponent<NavMeshAgent>();
		m_NavAgent.speed = m_MoveSpeed;

		m_Target = GameObject.FindGameObjectWithTag("Player").transform;

		waitForSeconds = new WaitForSeconds(m_RefreshRate);

		StartCoroutine(UpdatePath());
	}
}
