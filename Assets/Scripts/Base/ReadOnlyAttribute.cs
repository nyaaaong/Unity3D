﻿
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
	[SerializeField] private readonly bool m_RuntimeOnly;

	public bool RuntimeOnly => m_RuntimeOnly;

	public ReadOnlyAttribute(bool runtimeOnly = false)
	{
		m_RuntimeOnly = runtimeOnly;
	}
}