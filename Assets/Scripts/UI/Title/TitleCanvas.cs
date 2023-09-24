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
		Debug.LogError("서버 요청에 실패했습니다!");
	}

	protected override void Start()
	{
		base.Start();

#if UNITY_EDITOR
		DataManager.Init(OnSuccessLoadData, OnFailLoadData);
#else
		DataManager.Init(OnSuccessLoadData);
#endif
	}

	protected override void Awake()
	{
		base.Awake();

#if UNITY_EDITOR
		if (!m_LoadingAnim)
			Debug.LogError("if (!m_LoadingAnim)");

		if (!m_StartButton)
			Debug.LogError("if (!m_StartButton)");

		if (!m_QuitButton)
			Debug.LogError("if (!m_QuitButton)");
#endif

		m_StartButton.onClick.AddListener(OnClickStartButton);
		m_QuitButton.onClick.AddListener(OnClickQuitButton);

		m_LoadingAnim.SetActive(true);
		m_StartButton.gameObject.SetActive(false);
		m_QuitButton.gameObject.SetActive(false);

		Application.targetFrameRate = 60;
	}
}