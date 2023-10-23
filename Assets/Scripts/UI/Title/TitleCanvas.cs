using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleCanvas : BaseScript
{
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Fadeable_Type))] private Fadeable[] m_Fadeable = new Fadeable[(int)Fadeable_Type.Max];
	[ReadOnly(true)][SerializeField][EnumArray(typeof(Button_Type))] private Button[] m_Button = new Button[(int)Button_Type.Max];
	[ReadOnly(true)][SerializeField] private GameObject m_LoadingAnim;
	[ReadOnly(true)][SerializeField] private float m_LoadingTime = 3f;

	private float m_Time;
	private AudioSource m_Audio;

	private void OnSuccessLoadData()
	{
		// 다운로드가 굉장히 빨리 되기 때문에 로딩 UI를 보기 위해 일부러 시간을 지연시킨다.(다운로드 될 파일이 몇 개 안된다)
		StartCoroutine(WaitLoadingUI());
	}

	private IEnumerator WaitLoadingUI()
	{
		while (m_Time < m_LoadingTime)
		{
			m_Time += Time.deltaTime;

			yield return null;
		}

		m_LoadingAnim.SetActive(false);

		m_Fadeable[(int)Fadeable_Type.Background].StartFade();

		m_Audio.Play();
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
		FileManager.Init(OnSuccessLoadData);
#endif
	}

	public void OnClickedStart()
	{
		//SceneManager.LoadScene("LoadingScene");
	}

	public void OnClickedQuit()
	{
		Utility.Quit();
	}

	protected override void Awake()
	{
		base.Awake();

		Utility.CheckEmpty(m_LoadingAnim, "m_LoadingAnim");
		Utility.CheckEmpty(m_Fadeable, "m_Fadeable");
		Utility.CheckEmpty(m_Button, "m_Button");

		m_Audio = GetComponent<AudioSource>();

		Utility.CheckEmpty(m_Audio, "m_Audio");

		m_Audio.volume = 0.1f;

		int count = (int)Fadeable_Type.Max;
		Fadeable nextFadeable = null;

		for (int i = 0; i < count; ++i)
		{
			if (i == count - 1)
				nextFadeable = null;

			else
				nextFadeable = m_Fadeable[i + 1];

			m_Fadeable[i].NextFadeable = nextFadeable;
		}

		m_Fadeable[(int)Fadeable_Type.StartText].CallBack += () => m_Button[(int)Button_Type.Start].enabled = true;
		m_Fadeable[(int)Fadeable_Type.QuitText].CallBack += () => m_Button[(int)Button_Type.Quit].enabled = true;

		count = (int)Button_Type.Max;

		for (int i = 0; i < count; ++i)
		{
			m_Button[i].enabled = false;
		}
		
		m_LoadingAnim.SetActive(true);

		Application.targetFrameRate = 60;
	}
}