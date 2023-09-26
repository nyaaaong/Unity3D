﻿using UnityEngine;

public class Map : BaseScript
{
	[SerializeField] private Transform m_Floor;
	[SerializeField] private MapSpawn m_Spawn;

	private Vertex m_Vertex = null;

	public Vertex MapVertex => m_Vertex;
	public Vector3 RandomSpawnPos => m_Spawn.GetRandomSpawnPos();

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (m_Floor == null)
			Debug.LogError("if (m_Floor == null)");
#endif
		// 좌상단의 로컬 위치를 월드 위치로 바꾼 후 각 꼭지점 좌표를 넣어준다.
		m_Vertex = new Vertex(m_Floor);
	}
}