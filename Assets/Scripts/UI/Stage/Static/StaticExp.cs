
using System.Collections;
using UnityEngine;

public class StaticExp : BaseScript
{
	[SerializeField] private StaticExpBar m_Bar;
	[SerializeField] private StaticExpText m_Text;

	private float m_Percent;
	private float m_Exp;
	private float m_ExpMax = 100f;
	private bool m_IsUpdate;

	private WaitUntil m_IsBarStopped;
	private WaitUntil m_IsHideAbility;
	private WaitForSeconds m_UpdateTime = new WaitForSeconds(0.3f);

	public void UpdateExp()
	{
		if (!m_IsUpdate)
			StartCoroutine(UpdateExpBar());
	}

	public void ResetExp()
	{
		m_Exp = 0f;
		UpdateUI();
	}

	private IEnumerator UpdateExpBar()
	{
		m_IsUpdate = true;

		while (!StageManager.IsPlayerDeath)
		{
			// 업데이트를 특정 주기마다 반복한다.
			yield return m_UpdateTime;

			// 경험치에 따라 Bar와 Text를 지정해주고 Bar는 채워지는 애니메이션을 보여준다
			UpdateUI();

			// 만약 경험치가 다 찼다면
			while (m_Exp >= m_ExpMax)
			{
				// Bar의 애니메이션이 끝났을 때까지 대기하고
				yield return m_IsBarStopped;

				// 어빌리티 창을 띄운다.
				UIManager.ShowAbility();

				// 어빌리티 창을 닫았는지 검사
				yield return m_IsHideAbility;

				// 현재 경험치를 최대 경험치만큼 깎은 후 Bar, Text에 갱신한다.
				m_Exp -= m_ExpMax;
				UpdateUI();
			}
		}

		m_IsUpdate = false;
	}

	public void AddExp(float exp)
	{
		m_Exp += exp;
	}

	private void UpdateUI()
	{
		if (m_Exp == 0f)
			m_Percent = 0f;

		else if (m_Exp >= m_ExpMax)
			m_Percent = 1f;

		else
			m_Percent = m_Exp / m_ExpMax;

		m_Bar.SetExp(m_Percent);
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (m_Bar == null || m_Text == null)
			Debug.LogError("if (m_Bar == null || m_Text == null)");
#endif
		m_Bar.SetText(m_Text);

		m_IsBarStopped = new WaitUntil(() => m_Bar.IsStopped);
		m_IsHideAbility = new WaitUntil(() => UIManager.IsHideAbility);
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(UpdateExpBar());
	}
}