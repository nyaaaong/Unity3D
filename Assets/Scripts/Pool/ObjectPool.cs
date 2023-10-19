
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
		obj.SetActive(true);
		obj.transform.parent = m_Group.transform;

		m_Pool.Add(obj);

		return obj;
	}

	public GameObject Get(Vector3 position = default, Quaternion rotation = default)
	{
		int count = m_Pool.Count;
		GameObject obj;

		for (int i = 0; i < count; ++i)
		{
			obj = m_Pool[i];

			if (obj == null)
			{
				m_Pool.Remove(obj);
				count = m_Pool.Count;
				continue;
			}

			else if (!obj.activeSelf)
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
			if (obj != null)
				obj.SetActive(false);
		}
	}

	public void Clear(GameObject gameObject)
	{
		int count = m_Pool.Count;
		GameObject obj;

		for (int i = 0; i < count; ++i)
		{
			obj = m_Pool[i];

			if (obj == null)
			{
				m_Pool.Remove(obj);
				count = m_Pool.Count;
				continue;
			}

			else if (obj == gameObject)
			{
				Object.Destroy(obj);
				m_Pool.Remove(obj);
				return;
			}
		}

		Utility.LogError($"{gameObject.name}을 Pool에서 찾지 못했습니다!");
	}
}