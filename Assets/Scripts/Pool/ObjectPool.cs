
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	private GameObject m_Prefeb;
	private List<GameObject> m_Pool;
	private GameObject m_Group;

	public ObjectPool(GameObject prefeb)
	{
		m_Prefeb = prefeb;
		m_Pool = new List<GameObject>();
	}

	public void SetGroup(GameObject group)
	{
		m_Group = group;
	}

	private GameObject Create(Vector3 position, Quaternion rotation)
	{
		GameObject obj = Utility.Instantiate(m_Prefeb, position, rotation);
		obj.transform.parent = m_Group.transform;

		m_Pool.Add(obj);

		return obj;
	}

	public GameObject Get(Vector3 position = default, Quaternion rotation = default)
	{
		foreach (GameObject obj in m_Pool)
		{
			if (!obj.activeSelf)
			{
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				obj.SetActive(true);

				return obj;
			}
		}

		return Create(position, rotation);
	}

	public void ReleaseAll()
	{
		foreach (var obj in m_Pool)
		{
			obj.SetActive(false);
		}
	}

	public void Clear(GameObject gameObject)
	{
		foreach (GameObject obj in m_Pool)
		{
			if (obj == null)
				m_Pool.Remove(obj);

			else if (obj == gameObject)
			{
				Object.Destroy(obj);
				m_Pool.Remove(obj);
				return;
			}
		}

		Utility.LogError(gameObject.name + "을 Pool에서 찾지 못했습니다!");
	}
}