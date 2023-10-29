
using System;
using System.Collections;
using UnityEngine;

public class RendererManager : Singleton<RendererManager>
{
	[ReadOnly(true)][SerializeField] private Color m_HitEffectColor;
	[ReadOnly(true)][SerializeField] private GameObject m_Dummy;
	[ReadOnly(true)][SerializeField] private bool m_Preview;
	private const string m_EmissionKeyword = "_EMISSION";
	private const string m_EmissionColor = "_EmissionColor";

	private Renderer m_DummyRenderer;
	private Material m_DummyMaterial;

	public static Color HitEffectColor => Inst.m_HitEffectColor;

	public static void SetColor(Material material)
	{
		material.SetColor(m_EmissionColor, Inst.m_HitEffectColor);
	}

	public static void SetColorDummy()
	{
		if (Inst.m_DummyMaterial)
		{
			Inst.m_DummyMaterial.SetColor(m_EmissionColor, Inst.m_HitEffectColor);
			Inst.m_DummyRenderer.sharedMaterial = Inst.m_DummyMaterial;
		}
	}

	public static void ShowDummy(bool isShow)
	{
		if (Inst.m_Dummy)
			Inst.m_Dummy.SetActive(isShow);
	}

	public static bool HasDummy()
	{
		return Inst.m_Dummy;
	}

	public static void Init()
	{
		if (Inst.m_Dummy)
		{
			if (Inst.m_DummyMaterial == null)
			{
				Inst.m_DummyRenderer = Inst.m_Dummy.GetComponent<Renderer>();
				Inst.m_DummyMaterial = Inst.m_DummyRenderer.sharedMaterial;

				if (Inst.m_DummyMaterial != null)
					Inst.m_DummyMaterial.EnableKeyword(m_EmissionKeyword);
			}
		}
	}
}