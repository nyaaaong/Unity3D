using UnityEngine;
using System.Collections;

public class Target : BaseScript
{
	private WaitForEndOfFrame m_Timer = new WaitForEndOfFrame();

	protected override void OnEnable()
	{
		base.OnEnable();

		StartCoroutine(CheckRotation());
	}

	private IEnumerator CheckRotation()
	{
		while (true)
		{
			transform.rotation = Quaternion.identity;

			yield return m_Timer;
		}
	}
}
