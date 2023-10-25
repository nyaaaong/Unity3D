
using UnityEngine;

public class MapSpawn : BaseScript
{
	[SerializeField] private SpawnLocation m_Spawn;

	private Vertex m_SpawnVertex;
	private Vector3 m_RandPos;

	public Vector3 GetRandomSpawnPos()
	{
		m_RandPos.Set(Random.Range(m_SpawnVertex.Left, m_SpawnVertex.Right), 1f, Random.Range(m_SpawnVertex.Bottom, m_SpawnVertex.Top));

		return m_RandPos;
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_Spawn, "m_Spawn");

		m_Spawn.gameObject.SetActive(false);
		m_SpawnVertex = new Vertex(m_Spawn.WorldPosLT, m_Spawn.WorldPosRB);
	}
}