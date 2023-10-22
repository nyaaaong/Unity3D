
using UnityEngine;

public class MapSpawn : BaseScript
{
	[SerializeField] private Transform[] m_Spawn = new Transform[4];

	private Vertex[] m_SpawnVertex = new Vertex[4];
	private Vector3 m_RandPos;

	public Vector3 GetRandomSpawnPos()
	{
		Vertex vertex = m_SpawnVertex[Random.Range(0, 4)];
		m_RandPos.Set(Random.Range(vertex.Left, vertex.Right), 1f, Random.Range(vertex.Top, vertex.Bottom));

		return m_RandPos;
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_Spawn, "m_Spawn");

		// 각 스폰 구역들의 좌상단 꼭지점을 넣어준 후 Vertex 값들을 초기화한다.
		for (int i = 0; i < 4; ++i)
		{
			m_Spawn[i].gameObject.SetActive(false);
			m_SpawnVertex[i] = new Vertex(m_Spawn[i]);
		}
	}
}