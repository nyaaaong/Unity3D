using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
	public class CoroutineQueue
	{
		private readonly MonoBehaviour m_Owner = null;
		private Coroutine m_InternalCoroutine = null;
		private readonly Queue<IEnumerator> actions = new Queue<IEnumerator>();
		public CoroutineQueue(MonoBehaviour coroutineOwner)
		{
			m_Owner = coroutineOwner;
		}
		public void Start()
		{
			m_InternalCoroutine = m_Owner.StartCoroutine(Process());
		}
		public void Stop()
		{
			m_Owner.StopCoroutine(m_InternalCoroutine);
			m_InternalCoroutine = null;
		}
		public void EnqueueAction(IEnumerator aAction)
		{
			actions.Enqueue(aAction);
		}

		private IEnumerator Process()
		{
			while (true)
			{
				if (actions.Count > 0)
					yield return m_Owner.StartCoroutine(actions.Dequeue());
				else
					yield return null;
			}
		}
	}
}