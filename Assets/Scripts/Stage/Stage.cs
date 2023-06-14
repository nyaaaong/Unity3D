using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class Stage : BaseScript
{
	[SerializeField] private Monster m_Monster;

	private IObjectPool<Monster> m_Pool;
	private LinkedList<Monster> m_ActiveList;

	public void GetActiveMonsters(List<Monster> list)
	{
		foreach (var item in m_ActiveList)
			list.Add(item);
	}

	private Monster CreateMonster()
	{
		Monster monster = Instantiate(m_Monster).GetComponent<Monster>();
		monster.SetPool(m_Pool);

		return monster;
	}

	private void OnGetMonster(Monster monster)
	{
		m_ActiveList.AddLast(monster);

		monster.gameObject.SetActive(true);
		// 스테이지 내 랜덤한 위치에 생성
	}

	private void OnReleaseMonster(Monster monster)
	{
		DeleteList(monster);

		monster.gameObject.SetActive(false);
	}

	private void OnDestroyMonster(Monster monster)
	{
		DeleteList(monster);

		Destroy(monster.gameObject);
	}

	private void DeleteList(Monster monster)
	{
		var node = m_ActiveList.Find(monster);

		if (node != null)
			m_ActiveList.Remove(node);
	}

	protected override void Awake()
	{
		base.Awake();

		m_Pool = new ObjectPool<Monster>(CreateMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, maxSize: 3);
		m_Pool.Get();

		m_ActiveList = new LinkedList<Monster>();
	}

	protected override void Start()
	{
		base.Start();
	}
}
