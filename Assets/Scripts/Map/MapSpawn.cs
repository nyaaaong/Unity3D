
using UnityEngine;

public class MapSpawn : BaseScript
{
	[SerializeField] private SpawnLocation m_Spawn;

	private Vertex m_SpawnVertex;
	private Vector3 m_RandPos;
	private float m_Angle;
	private float m_Radius;
	private float m_MinDist;
	private float m_MaxDist;

	private Vector3 GetRange(Vector3 center)
	{
		// 플레이어를 중심으로 하는 원 모양을 만들어 그 위치에 몬스터를 소환할 것이다.
		m_Angle = Random.Range(0f, 2f * Mathf.PI); // 원의 방정식을 사용하여 랜덤한 각도를 구한다.
		m_Radius = Random.Range(m_MinDist, m_MaxDist); // 원의 반지름을 구한다

		// 극좌표계를 직교좌표계로 변환하는 식을 이용한다
		m_RandPos.x = center.x + m_Radius * Mathf.Cos(m_Angle);
		m_RandPos.z = center.z + m_Radius * Mathf.Sin(m_Angle);

		// m_SpawnVertex의 최소, 최대를 비교하여 안쪽으로 배치시킨다.
		m_RandPos.x = m_SpawnVertex.Right < m_RandPos.x ? m_SpawnVertex.Right : m_RandPos.x;
		m_RandPos.x = m_RandPos.x < m_SpawnVertex.Left ? m_SpawnVertex.Left : m_RandPos.x;

		m_RandPos.z = m_SpawnVertex.Top < m_RandPos.z ? m_SpawnVertex.Top : m_RandPos.z;
		m_RandPos.z = m_RandPos.z < m_SpawnVertex.Bottom ? m_SpawnVertex.Bottom : m_RandPos.z;

		return m_RandPos;
	}

	public Vector3 GetRandomSpawnPos(Player player)
	{
		return GetRange(player.Pos);
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_Spawn, "m_Spawn");

		m_Spawn.gameObject.SetActive(false);
		m_SpawnVertex = new Vertex(m_Spawn.WorldPosLT, m_Spawn.WorldPosRB);

		m_MinDist = StageManager.MinDist;
		m_MaxDist = StageManager.MaxDist;
	}
}