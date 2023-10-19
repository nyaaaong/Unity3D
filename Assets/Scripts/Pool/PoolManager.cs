
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
	private GameObject m_RootGroup;
	private Dictionary<string, GameObject> m_PoolGroups;
	private Dictionary<int, ObjectPool> m_Pools;

	public static void Create(GameObject prefeb, string groupName)
	{
		if (prefeb == null)
		{
			Utility.LogError("등록하려는 프리팹이 비어있습니다!");
			return;
		}

		int key = prefeb.GetHashCode();

		if (Inst.m_Pools.ContainsKey(key))
		{
			Utility.LogError("이미 등록되어 있는 프리팹을 등록하려고 합니다!");
			return;
		}

		ObjectPool objectPool = new ObjectPool(prefeb);
		Inst.m_Pools.Add(key, objectPool);

		GameObject group;

		if (!Inst.m_PoolGroups.ContainsKey(groupName))
		{
			group = new GameObject(groupName);
			group.transform.parent = Inst.m_RootGroup.transform;

			Inst.m_PoolGroups.Add(groupName, group);
		}

		else
			group = Inst.m_PoolGroups[groupName];

		objectPool.SetGroup(group);
	}

	public static GameObject Get(int prefebHashCode, Vector3 position = default, Quaternion rotation = default)
	{
		if (!Inst.m_Pools.ContainsKey(prefebHashCode))
		{
			Utility.LogError("프리팹 등록부터 하세요!");
			return null;
		}

		return Inst.m_Pools[prefebHashCode].Get(position, rotation);
	}

	public static GameObject Get(GameObject prefeb, Vector3 position = default, Quaternion rotation = default)
	{
		return Get(prefeb.GetHashCode(), position, rotation);
	}

	public static void Clear(GameObject gameObject)
	{
		foreach (var objPool in Inst.m_Pools.Values)
		{
			objPool.Clear(gameObject);
		}
	}

	public static void Release(GameObject gameObject)
	{
		gameObject.SetActive(false);
	}

	public static void ReleaseAll()
	{
		if (!Inst || Inst.m_Pools == null)
			return;

		foreach (var objPool in Inst.m_Pools.Values)
		{
			objPool.ReleaseAll();
		}
	}

	protected override void Awake()
	{
		base.Awake();

		m_RootGroup = new GameObject("Pool");
		m_PoolGroups = new Dictionary<string, GameObject>();

		m_Pools = new Dictionary<int, ObjectPool>();
	}
}