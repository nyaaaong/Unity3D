
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : BaseScript where T : BaseScript
{
	private static T m_Inst = null;

	public static T GetInst()
	{
		if (!m_Inst)
		{
			m_Inst = FindObjectOfType(typeof(T)) as T;

			if (!m_Inst)
			{
				GameObject obj = new GameObject(typeof(T).Name, typeof(T));
				m_Inst = obj.GetComponent<T>();
			}
		}

		return m_Inst;
	}

	protected override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(transform.root.gameObject);
	}
}