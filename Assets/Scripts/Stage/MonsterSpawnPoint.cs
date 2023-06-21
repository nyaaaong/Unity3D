using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MonsterSpawnPoint : BaseScript
{
	[SerializeField] Transform m_LeftUp;
	[SerializeField] Transform m_RightDown;

	private MeshRenderer m_Mesh;

	protected override void Awake()
	{
		base.Awake();

		m_Mesh = GetComponent<MeshRenderer>();
		m_Mesh.enabled = false;

		if (!m_LeftUp || !m_RightDown)
			Debug.LogError("if (!m_LeftUp || !m_RightDown)");

		MeshRenderer childMesh = m_LeftUp.GetComponent<MeshRenderer>();
		childMesh.enabled = false;

		childMesh = m_RightDown.GetComponent<MeshRenderer>();
		childMesh.enabled = false;

		StageManager.SetSpawnPoint(Spawn_Type.Monster, m_LeftUp, m_RightDown);
	}
}
