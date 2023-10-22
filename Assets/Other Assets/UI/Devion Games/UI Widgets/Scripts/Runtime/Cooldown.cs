using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour, IPointerClickHandler
{
	public Image overlay;

	public bool IsCoolDown { get; private set; }
	private float coolDownDuration;
	private float coolDownInitTime;
	public void Update()
	{
		if (overlay != null)
		{
			overlay.fillAmount = Time.time - coolDownInitTime < coolDownDuration ? Mathf.Clamp01(1 - ((Time.time - coolDownInitTime) / coolDownDuration)) : 0;
			IsCoolDown = overlay.fillAmount > 0;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		DoCooldown(3f, 1.5f);
	}

	private void DoCooldown(float coolDown, float globalCoolDown)
	{
		if (!IsCoolDown)
		{
			coolDownDuration = coolDown;
			coolDownInitTime = Time.time;
			IsCoolDown = true;
			transform.root.BroadcastMessage("DoGlobalCooldown", globalCoolDown, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void DoGlobalCooldown(float coolDown)
	{
		if (((Time.time + (coolDownInitTime * coolDownDuration)) < (Time.time + (coolDownInitTime * coolDown))) || !IsCoolDown)
		{
			coolDownDuration = coolDown;
			coolDownInitTime = Time.time;
		}
	}
}
