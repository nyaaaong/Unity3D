using UnityEngine.Events;

namespace DevionGames.UIWidgets
{
	internal struct FloatTween : ITweenValue
	{
		private FloatTween.FloatTweenCallback m_Target;
		private FloatTween.FloatTweenFinishCallback m_OnFinish;

		public EasingEquations.EaseType easeType { get; set; }
		public float startValue { get; set; }

		public float targetValue { get; set; }
		public float duration { get; set; }

		public bool ignoreTimeScale { get; set; }

		public bool ValidTarget()
		{
			return m_Target != null;
		}

		public void TweenValue(float floatPercentage)
		{
			if (!ValidTarget())
			{
				return;
			}

			float value = EasingEquations.GetValue(easeType, startValue, targetValue, floatPercentage);
			m_Target.Invoke(value);

		}

		public void AddOnChangedCallback(UnityAction<float> callback)
		{
			m_Target ??= new FloatTweenCallback();

			m_Target.AddListener(callback);
		}

		public void AddOnFinishCallback(UnityAction callback)
		{
			m_OnFinish ??= new FloatTweenFinishCallback();

			m_OnFinish.AddListener(callback);
		}

		public void OnFinish()
		{
			if (m_OnFinish != null)
				m_OnFinish.Invoke();
		}

		public class FloatTweenCallback : UnityEvent<float>
		{
			public FloatTweenCallback()
			{
			}
		}

		public class FloatTweenFinishCallback : UnityEvent
		{
			public FloatTweenFinishCallback()
			{
			}
		}
	}
}