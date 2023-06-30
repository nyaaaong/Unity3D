using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PlayerSpawnPoint : BaseScript
{
	private MeshRenderer m_Mesh;

	protected override void Awake()
	{
		base.Awake();

		m_Mesh = GetComponent<MeshRenderer>();
		m_Mesh.enabled = false;
	}

	protected override void Start()
	{
		base.Start();

		StageManager.SetPlayerSpawnPoint(transform);
	}
}
