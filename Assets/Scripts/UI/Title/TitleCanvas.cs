using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField] private Button m_StartButton;
	[ReadOnly(true)][SerializeField] private Button m_QuitButton;
	[ReadOnly(true)][SerializeField] private GameObject m_LoadingAnim;

	private void OnClickStartButton()
	{
		SceneManager.LoadScene("PlayScene");
	}

	private void OnClickQuitButton()
	{
		Utility.Quit();
	}

	private void OnSuccessLoadData()
	{
		m_LoadingAnim.SetActive(false);
		m_StartButton.gameObject.SetActive(true);
		m_QuitButton.gameObject.SetActive(true);
	}

	private void OnFailLoadData()
	{
		Utility.LogError("서버 요청에 실패했습니다!");
	}

	protected override void Start()
	{
		base.Start();

#if UNITY_EDITOR
		FileManager.Init(OnSuccessLoadData, OnFailLoadData);
#else
		DataManager.Init(OnSuccessLoadData);
#endif
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_LoadingAnim, "m_LoadingAnim");
		Utility.CheckEmpty(m_StartButton, "m_StartButton");
		Utility.CheckEmpty(m_QuitButton, "m_QuitButton");

		m_StartButton.onClick.AddListener(OnClickStartButton);
		m_QuitButton.onClick.AddListener(OnClickQuitButton);

		m_LoadingAnim.SetActive(true);
		m_StartButton.gameObject.SetActive(false);
		m_QuitButton.gameObject.SetActive(false);

		Application.targetFrameRate = 60;
	}
}