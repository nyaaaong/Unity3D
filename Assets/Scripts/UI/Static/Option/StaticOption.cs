using UnityEngine.UI;

public class StaticOption : BaseScript
{
	private Button m_Button;

	private void OnClickEvent()
	{
		UIManager.ShowMenu(Menu_Type.Menu);
		AudioManager.PauseNeedBossSpawnAudio();
	}

	protected override void Awake()
	{
		base.Awake();

		m_Button = GetComponent<Button>();

		m_Button.onClick.AddListener(OnClickEvent);
	}
}