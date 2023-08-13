using System.Collections;
using UnityEngine;

public class Target : BaseScript
{
	private Quaternion m_Rot;
	private WaitForEndOfFrame m_NextFrame;

	protected override void Awake()
	{
		base.Awake();

		m_Rot = transform.rotation;
		m_NextFrame = new WaitForEndOfFrame();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		StartCoroutine(CheckRotation());
	}

	private IEnumerator CheckRotation()
	{
		while (true)
		{
			transform.rotation = m_Rot;

			yield return m_NextFrame;
		}
	}
}
