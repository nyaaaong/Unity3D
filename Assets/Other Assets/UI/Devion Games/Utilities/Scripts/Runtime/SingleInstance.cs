using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames
{
	public class SingleInstance : MonoBehaviour
	{
		private static readonly Dictionary<string, GameObject> m_Instances = new Dictionary<string, GameObject>();

		private void Awake()
		{
			SingleInstance.m_Instances.TryGetValue(name, out GameObject instance);
			if (instance == null)
			{
				DontDestroyOnLoad(gameObject);
				SingleInstance.m_Instances[name] = gameObject;
			}
			else
			{
				//Debug.Log("Multiple "+gameObject.name+" in scene. Destroying instance!");
				DestroyImmediate(gameObject);
			}
		}

		public static List<GameObject> GetInstanceObjects()
		{
			return m_Instances.Values.Where(x => x != null).ToList();
		}
	}
}