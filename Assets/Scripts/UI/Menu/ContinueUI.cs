
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

#if UNITY_EDITOR
		if (!m_YesButton)
			Debug.LogError("if (!m_YesButton)");

		if (!m_NoButton)
			Debug.LogError("if (!m_NoButton)");
#endif

		m_YesButton.onClick.AddListener(OnYesClickEvent);
		m_NoButton.onClick.AddListener(OnNoClickEvent);
	}
}