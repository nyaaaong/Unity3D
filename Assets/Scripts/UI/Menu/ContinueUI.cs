
using UnityEngine;
using UnityEngine.UI;

public class ContinueUI : BaseScript
{
	[ReadOnly(true)][SerializeField] private Button m_ContinueButton;
	[ReadOnly(true)][SerializeField] private Button m_ExitButton;

	private void OnContinueClickEvent()
	{
		UIManager.HideMenu(Menu_Type.Continue);
		StageManager.ResetStage();
	}

	private void OnExitClickEvent()
	{
		UIManager.HideMenu(Menu_Type.Continue);
		Utility.Quit();
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_ContinueButton, "m_ContinueButton");
		Utility.CheckEmpty(m_ExitButton, "m_ExitButton");

		m_ContinueButton.onClick.AddListener(OnContinueClickEvent);
		m_ExitButton.onClick.AddListener(OnExitClickEvent);
	}
}