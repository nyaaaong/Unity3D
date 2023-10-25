
using UnityEngine;

public class SpawnLocation : BaseScript
{
	[SerializeField] private GameObject m_LT;
	[SerializeField] private GameObject m_RB;

	public Vector3 WorldPosLT => m_LT.transform.position;
	public Vector3 WorldPosRB => m_RB.transform.position;

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_LT, "m_LT");
		Utility.CheckEmpty(m_RB, "m_RB");
	}
}