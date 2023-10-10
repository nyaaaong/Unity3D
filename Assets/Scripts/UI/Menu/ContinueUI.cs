
using UnityEngine;
using UnityEngine.UI;

public class ContinueUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Button m_YesButton;
	[ReadOnly(true)][SerializeField] private Button m_NoButton;

	private void OnYesClickEvent()
	{
		UIManager.HideMenu(Menu_Type.Continue);
		StageManager.ResetStage();
	}

	private void OnNoClickEvent()
	{
		UIManager.HideMenu(Menu_Type.Continue);
		Utility.Quit();
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_YesButton, "m_YesButton");
		Utility.CheckEmpty(m_NoButton, "m_NoButton");

		m_YesButton.onClick.AddListener(OnYesClickEvent);
		m_NoButton.onClick.AddListener(OnNoClickEvent);
	}
}