using UnityEngine;
using System.Collections;

public class Target : BaseScript
{
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

			yield return null;
		}
	}
}
