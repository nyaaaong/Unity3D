
using System;
using UnityEngine;

public class BossSpawnEffect : BaseScript
{
	[ReadOnly(true)][SerializeField] private float m_FadeTime = 1f;

	private float m_Time;

	public event Action OnAfterDestroy;

	private void WaitOneSecond()
	{
		while (true)
		{
			m_Time += Time.deltaTime;

			if (m_Time >= 1f)
			{
				Destroy(gameObject);
				break;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		FadeIn(m_FadeTime);
	}

	public void FadeOut()
	{
		FadeOut(m_FadeTime, () => WaitOneSecond());
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (OnAfterDestroy != null)
			OnAfterDestroy();
	}
}