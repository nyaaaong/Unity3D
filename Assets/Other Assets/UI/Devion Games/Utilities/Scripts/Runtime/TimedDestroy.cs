using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DevionGames
{
	public class TimedDestroy : MonoBehaviour
	{
		[SerializeField]
		private float m_Delay = 1f;
		[SerializeField]
		private UnityEvent m_OnDestroy = null;

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(m_Delay);
			m_OnDestroy?.Invoke();
			Destroy(gameObject);
		}
	}
}