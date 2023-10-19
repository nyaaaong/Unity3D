
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Particle_Type))] private GameObject[] m_ParticlePrefeb = new GameObject[(int)Particle_Type.Max];

	public static GameObject GetParticlePrefeb(Particle_Type type)
	{
		return Inst.m_ParticlePrefeb[(int)type];
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_ParticlePrefeb, "m_ParticlePrefeb");

		int count = m_ParticlePrefeb.Length;

		for (int i = 0; i < count; ++i)
		{
			PoolManager.Create(m_ParticlePrefeb[i], "Particle");
		}
	}
}