
using UnityEngine;

public class Singleton<T> : BaseScript where T : BaseScript
{
	private static T m_Inst = null;
	private static bool m_Quit;

	private void OnApplicationQuit()
	{
		m_Quit = true;
	}

	public static T Inst
	{
		get
		{
			if (Application.isPlaying && m_Quit)
				return null;

			else if (!m_Inst)
			{
				m_Inst = FindAnyObjectByType(typeof(T)) as T;

				if (!m_Inst)
				{
					GameObject obj = new GameObject(typeof(T).Name, typeof(T));
					m_Inst = obj.GetComponent<T>();
				}
			}

			return m_Inst;
		}
	}
}