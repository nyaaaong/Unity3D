
using UnityEngine;
using UnityEngine.UI;

public class AbilityInfoUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Text m_Stack;
	[ReadOnly(true)][SerializeField] private Text m_Description;
	[ReadOnly(true)][SerializeField] private Ability_Type m_Type = Ability_Type.Max;

	public void SetActive(bool active)
	{
		gameObject.SetActive(active);
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_Stack, "m_Stack");
		Utility.CheckEmpty(m_Description, "m_Description");

		if (m_Type == Ability_Type.Max)
			Utility.LogError("Type를 제대로 설정해주세요!");
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_Stack.text = string.Format(m_Stack.text, DataManager.GetBuffStack(m_Type));
		m_Description.text = string.Format(m_Description.text, DataManager.GetBuffPercent(m_Type, false));
	}
}