using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField] private Button m_StartButton;
	[ReadOnly(true)][SerializeField] private Button m_QuitButton;
	[ReadOnly(true)][SerializeField] private GameObject m_LoadingAnim;

	private bool m_Error;
	private string m_Path;
	private string[] m_URLInfo =
	{
		"https://gist.githubusercontent.com/nyaaaong/420b0926f26e527e06aff3c079120881/raw/9bdf9dc642f3fcf778aa393d1170adac843d95e8/AbilityInfo.json",
		"https://gist.githubusercontent.com/nyaaaong/558ae235d27049d9d41b4fc15e9954f4/raw/e76352edcbf98760d695b355b2c0bfaa6a2f6cb3/CharInfo.json",
		"https://gist.githubusercontent.com/nyaaaong/936d17164a8ee7937f4fc56fb6dbac7e/raw/cba831059d949b2eb6bd504d943f559321c4433f/StageInfo.json"
	};

	private IEnumerator LoadJSONFromServer()
	{
		m_Path = Utility.SettingPath();

		for (int i = 0; i < m_URLInfo.Length; ++i)
		{
			string url = m_URLInfo[i];
			Uri uri = new Uri(url);
			string fileName = Path.GetFileName(uri.LocalPath);

			// UnityWebRequest 생성
			using (UnityWebRequest www = UnityWebRequest.Get(url))
			{
				// 요청 보내기
				yield return www.SendWebRequest();

				// 요청이 성공한 경우
				if (www.result == UnityWebRequest.Result.Success)
					File.WriteAllText(m_Path + fileName, www.downloadHandler.text);

				else
					m_Error = true;
			}
		}

		if (!m_Error)
		{
			m_LoadingAnim.SetActive(false);
			m_StartButton.gameObject.SetActive(true);
			m_QuitButton.gameObject.SetActive(true);
		}
#if UNITY_EDITOR
		else
			Debug.LogError("Error!");
#endif
	}

	private void OnClickStartButton()
	{
		SceneManager.LoadScene("PlayScene");
	}

	private void OnClickQuitButton()
	{
		Utility.Quit();
	}

	protected override void Start()
	{
		base.Start();

		StartCoroutine(LoadJSONFromServer());
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