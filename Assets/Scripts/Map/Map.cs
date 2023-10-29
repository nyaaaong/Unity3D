using UnityEditor;
using UnityEngine;

public class Map : BaseScript
{
	[SerializeField] private Transform m_Floor;
	[SerializeField] private MapSpawn m_Spawn;
	[SerializeField] private float m_MinDist = 1f;
	[SerializeField] private float m_MaxDist = 3f;

	private Vertex m_Vertex = null;

	public Vertex MapVertex => m_Vertex;
	public float MinDist => m_MinDist;
	public float MaxDist => m_MaxDist;

	public Vector3 RandomSpawnPos(Player player)
	{
		return m_Spawn.GetRandomSpawnPos(player);
	}

	private void OnDrawGizmosSelected()
	{
		Handles.color = Color.green;
		Handles.DrawWireDisc(transform.position, Vector3.up, m_MinDist);

		Handles.color = Color.red;
		Handles.DrawWireDisc(transform.position, Vector3.up, m_MaxDist);
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_Floor, "m_Floor");

		// 좌상단의 로컬 위치를 월드 위치로 바꾼 후 각 꼭지점 좌표를 넣어준다.
		m_Vertex = new Vertex(m_Floor);
	}
}