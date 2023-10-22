using UnityEngine;
using UnityEngine.UI;

public class Togglescript : MonoBehaviour
{
	private Toggle toggle;

	private void Start()
	{
		toggle = GetComponent<Toggle>();
	}

	public GameObject Slider;

	private void Update()
	{
		if (toggle.isOn)
		{
			Slider.SetActive(false);
		}
		else
		{
			Slider.SetActive(true);
		}
	}
}
