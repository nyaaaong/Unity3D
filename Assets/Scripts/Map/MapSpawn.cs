
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
		// �÷��̾ �߽����� �ϴ� �� ����� ����� �� ��ġ�� ���͸� ��ȯ�� ���̴�.
		m_Angle = Random.Range(0f, 2f * Mathf.PI); // ���� �������� ����Ͽ� ������ ������ ���Ѵ�.
		m_Radius = Random.Range(m_MinDist, m_MaxDist); // ���� �������� ���Ѵ�

		// ����ǥ�踦 ������ǥ��� ��ȯ�ϴ� ���� �̿��Ѵ�
		m_RandPos.x = center.x + m_Radius * Mathf.Cos(m_Angle);
		m_RandPos.z = center.z + m_Radius * Mathf.Sin(m_Angle);

		// m_SpawnVertex�� �ּ�, �ִ븦 ���Ͽ� �������� ��ġ��Ų��.
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