
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Monster
{
	public class Pattern
	{
		public float Percent; // 패턴이 실행될 확률
		public float Duration; // 지속 시간
		public Action InitEvent; // 처음 실행 될 이벤트
		public Action LoopEvent; // 반복 될 이벤트
		public float LoopDelay; // 반복 이벤트 간 딜레이

		public Pattern(float percent, float duration, Action initEvent, Action loopEvent = null, float loopDelay = 0f)
		{
			Percent = percent;
			Duration = duration;
			InitEvent = initEvent;
			LoopEvent = loopEvent;
			LoopDelay = loopDelay;
		}
	}

	private List<Pattern> m_PatternList; // 패턴을 저장할 리스트
	private float m_Percent; // 0% ~ Total% 중 랜덤한 값
	private float m_PatternTime; // 패턴 쿨타임 계산을 위한 누적될 시간
	private float m_PatternCoolDown = 3f; // 패턴 쿨타임, 패턴과 패턴 사이의 여유 시간
	private float m_PatternLoopDurTime; // 패턴 내 지속 시간
	private float m_PatternDurTime; // 패턴 종료할 때 지속 시간
	private float m_PatternTotalPercent; // 패턴의 전체 실행 확률
	private bool m_PatternSkip; // 패턴을 스킵하면 즉시 실행중이던 패턴을 끝내고 다시 반복돌린다.
	protected bool m_UseNavSystem; // 이것을 활성화하는 경우 OnEnable에서 InitNavSystem 함수를 실행한다.
	protected delegate void Event();

	protected virtual void PatternEndEvent() { }

	protected override void OnEnable()
	{
		base.OnEnable();

		InitNavSystem();
	}

	private void InitNavSystem()
	{
		if (m_UseNavSystem)
		{
			StartCoroutine(UpdatePath());
			StartCoroutine(VisibleTarget());
		}
	}

	protected void PatternSkip()
	{
		m_PatternSkip = true;
	}

	private IEnumerator CheckPattern()
	{
		bool hasLoopPattern;

		while (!m_Dead && !StageManager.IsPlayerDeath)
		{
			m_PatternTime += Time.deltaTime;

			if (m_PatternTime >= m_PatternCoolDown)
			{
				m_PatternTime = 0f;
				m_Percent = UnityEngine.Random.Range(0f, m_PatternTotalPercent);

				foreach (Pattern pattern in m_PatternList)
				{
					m_Percent -= pattern.Percent;

					if (m_Percent <= 0f)
					{
						hasLoopPattern = pattern.LoopEvent != null;

						if (hasLoopPattern)
							m_PatternLoopDurTime = pattern.LoopDelay;

						pattern.InitEvent();

						while (!m_Dead && !StageManager.IsPlayerDeath && !m_PatternSkip) // 패턴이 스킵되면 바로 종료.
						{
							m_PatternDurTime += Time.deltaTime;

							if (m_PatternDurTime >= pattern.Duration)
							{
								m_PatternDurTime = 0f;

								PatternEndEvent();
								break;
							}

							if (hasLoopPattern)
							{
								m_PatternLoopDurTime += Time.deltaTime;

								if (m_PatternLoopDurTime >= pattern.LoopDelay)
								{
									m_PatternLoopDurTime = 0f;

									pattern.LoopEvent();
								}
							}

							yield return null;
						}

						if (m_PatternSkip)
							m_PatternSkip = false;

						break;
					}
				}
			}

			yield return null;
		}
	}

	protected void AddPattern(float percent, float duration, Action initEvent, Action loopEvent = null, float loopDelay = 0f)
	{
		if (m_PatternList == null)
			return;

		m_PatternList.Add(new Pattern(percent, duration, initEvent, loopEvent, loopDelay));
	}

	protected override void Awake()
	{
		base.Awake();

		m_PatternList = new List<Pattern>();

		UIManager.SetBossHPOwner(this);
	}

	protected override void Start()
	{
		base.Start();

		if (m_PatternList.Count == 0)
			Utility.LogError("패턴은 반드시 한 개 이상은 있어야 합니다!");

		else
		{
			// 패턴은 반드시 하나라도 실행되어야 하기 때문에 전체 퍼센트를 구해준다.
			foreach (Pattern item in m_PatternList)
			{
				m_PatternTotalPercent += item.Percent;
			}

			StartCoroutine(CheckPattern());
		}
	}
}