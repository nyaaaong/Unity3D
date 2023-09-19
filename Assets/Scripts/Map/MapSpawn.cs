
using UnityEngine;

public class MapSpawn : BaseScript
{
	[SerializeField] private Transform[] m_Spawn = new Transform[4];

	private Vertex[] m_SpawnVertex = new Vertex[4];

	public Vector3 GetRandomSpawnPos()
	{
		Vertex vertex = m_SpawnVertex[Random.Range(0, 4)];
		Vector3 randPos = new Vector3(Random.Range(vertex.Left, vertex.Right), 0f, Random.Range(vertex.Top, vertex.Bottom));

		return randPos;
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		Utility.CheckEmpty(m_Spawn, "m_Spawn");
#endif

		// �� ���� �������� �»�� �������� �־��� �� Vertex ������ �ʱ�ȭ�Ѵ�.
		for (int i = 0; i < 4; ++i)
		{
			m_Spawn[i].gameObject.SetActive(false);
			m_SpawnVertex[i] = new Vertex(m_Spawn[i]);
		}
	}
}