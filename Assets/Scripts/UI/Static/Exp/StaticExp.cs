using System.Collections;
using UnityEngine;

public class StaticExp : BaseScript
{
	[SerializeField] private StaticExpBar m_Bar;
	[SerializeField] private StaticExpText m_Text;

	private float m_Percent;
	private float m_Exp;
	private float m_SaveExp;
	private float m_ExpMax;
	private bool m_NeedUpdate; // 경험치가 올라가서 업데이트가 필요한 경우

	private WaitUntil m_IsBarStopped;
	private WaitUntil m_IsHideAbility;
	private WaitUntil m_WaitPlayerAlive;
	private WaitUntil m_WaitNeedUpdate;

	public bool NeedLevelUp => m_Exp >= m_ExpMax;
	public bool NeedUpdate => m_NeedUpdate;

	public void ResetExp()
	{
		m_NeedUpdate = true;
		m_Exp = 0f;
	}

	private IEnumerator UpdateExpBar()
	{
		while (true)
		{
			// 플레이어가 살아있을 때 까지 대기
			yield return m_WaitPlayerAlive;

			// Update가 필요할 때까지 대기
			yield return m_WaitNeedUpdate;

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

				// 여기서 렙업을 했으니 ExpMax를 업데이트 해줘야 한다.
				DataManager.AddPlayerLevel();
				m_ExpMax = StageManager.GetPlayerExpMax();

				// 현재 경험치를 최대 경험치만큼 깎은 후 Bar, Text에 갱신한다.
				// 만약 여전히 Max와 같거나 넘는 경우 m_Exp는 0으로 두고, 마이너스한 값을 저장했다가 m_Exp에 넣어줘야 한다.
				// 즉, 100%를 넘는 경험치가 있는 경우 무조건 0%를 거쳐야 한다.
				m_SaveExp = m_Exp - m_ExpMax;

				if (m_SaveExp >= m_ExpMax)
				{
					m_Exp = 0f;
					UpdateUI();

					yield return m_IsBarStopped;
				}

				m_Exp = m_SaveExp;
				UpdateUI();
			}

			m_NeedUpdate = false;
		}
	}

	public void AddExp(float exp)
	{
		m_Exp += exp;
		m_NeedUpdate = true;
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

		Utility.CheckEmpty(m_Bar, "m_Bar");
		Utility.CheckEmpty(m_Text, "m_Text");

		m_Bar.SetText(m_Text);

		m_IsBarStopped = new WaitUntil(() => m_Bar.IsStopped);
		m_IsHideAbility = new WaitUntil(() => UIManager.IsHideAbility);
		m_WaitPlayerAlive = new WaitUntil(() => !StageManager.IsPlayerDeath);


		m_ExpMax = DataManager.CharData[(int)Char_Type.Player].Exp;
		m_WaitNeedUpdate = new WaitUntil(() => m_NeedUpdate);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		
		StartCoroutine(UpdateExpBar());
	}
}