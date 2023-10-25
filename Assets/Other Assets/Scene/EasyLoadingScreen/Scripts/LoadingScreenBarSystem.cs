using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenBarSystem : MonoBehaviour
{
	public GameObject bar;
	public Text loadingText;
	public GameObject[] backgroundImages;
	[Range(0, 1f)] public float vignetteEfectVolue; // Must be a value between 0 and 1
	AsyncOperation async;
	Image vignetteEfect;
	private WaitForSeconds m_Wait = new WaitForSeconds(1f);


	public void loadingScreen()
	{
		this.gameObject.SetActive(true);
		StartCoroutine(Loading());
	}

	private void Start()
	{
		vignetteEfect = transform.Find("VignetteEfect").GetComponent<Image>();
		vignetteEfect.color = new Color(vignetteEfect.color.r, vignetteEfect.color.g, vignetteEfect.color.b, vignetteEfectVolue);

		StartCoroutine(transitionImage());
	}


	// The pictures change according to the time of
	IEnumerator transitionImage()
	{
		int idx = 0, max = backgroundImages.Length;

		while (true)
		{
			yield return m_Wait;

			backgroundImages[idx].SetActive(true);
			backgroundImages[1 + idx++].SetActive(false);

			if (idx == max)
				idx = 0;
		}
	}

	// Activate the scene 
	IEnumerator Loading()
	{
		async = SceneManager.LoadSceneAsync("PlayScene");
		async.allowSceneActivation = false;

		// Continue until the installation is completed
		while (async.isDone == false)
		{
			bar.transform.localScale = new Vector3(async.progress, 0.9f, 1);

			if (loadingText != null)
				loadingText.text = $"{100 * bar.transform.localScale.x:F1}%";

			if (async.progress == 0.9f)
			{
				bar.transform.localScale = new Vector3(1, 0.9f, 1);
				async.allowSceneActivation = true;
			}
			yield return null;
		}
	}
}
