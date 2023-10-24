using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : BaseScript
{
	[ReadOnly(true)][SerializeField] private bool m_BossHPBar;

	private Character m_Owner;
	private Slider m_HPSlider;
	private Image m_HPBar;
	private float m_HP;
	private float m_HPMax;
	private float m_LerpSpeed = 10f;
	private float m_LerpTime = 0f;

	public event Action OnHide;

	public void SetHPBar(float hp, float hpMax)
	{
		m_HP = hp;
		m_HPMax = hpMax;
	}

	private float Bar
	{
		get
		{
			if (m_HPSlider != null)
				return m_HPSlider.value;

			return m_HPBar.fillAmount;
		}

		set
		{
			if (m_HPSlider != null)
				m_HPSlider.value = value;

			else
				m_HPBar.fillAmount = value;
		}
	}

	public void SetOwner(Character owner = null, Action onHide = null)
	{
		if (owner == null)
			m_Owner = transform.parent.GetComponentInChildren<Character>();

		else
		{
			m_Owner = owner;
			OnHide += onHide;
		}

		if (m_Owner.Type == Char_Type.Player)
			m_HPMax = StageManager.GetPlayerHPMax(m_Owner);

		else
			m_HPMax = m_Owner.HPMax;

		m_HP = m_HPMax;

		StartCoroutine(BarUpdate());
	}

	protected override void Awake()
	{
		base.Awake();

		Transform hpbar = transform.Find("HPBar");

		m_HPSlider = hpbar.GetComponent<Slider>();
		m_HPBar = hpbar.GetComponent<Image>();

		if (!m_BossHPBar)
			SetOwner();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		Bar = 1f;

		if (m_Owner && !m_BossHPBar)
		{
			if (m_Owner.Type == Char_Type.Player)
				m_HPMax = StageManager.GetPlayerHPMax(m_Owner);

			else
				m_HPMax = m_Owner.HPMax;

			m_HP = m_HPMax;

			StartCoroutine(BarUpdate());
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (m_BossHPBar)
		{
			m_Owner = null;
			OnHide = null;
		}
	}

	private IEnumerator BarUpdate()
	{
		while (true)
		{
			if (!m_BossHPBar)
				transform.position = m_Owner.transform.position;

			m_HP = m_Owner.HP;

			m_LerpTime = Mathf.Clamp(Time.deltaTime * m_LerpSpeed, 0f, 1f);

			if (m_HP > 0f)
				Bar = Mathf.Lerp(Bar, m_HP / m_HPMax, m_LerpTime);

			else
			{
				if (Bar > 1f)
					Bar = Mathf.Lerp(Bar, 0f, m_LerpTime);

				else if (m_BossHPBar)
					OnHide();

				else
					gameObject.SetActive(false);
			}

			yield return null;
		}
	}
}