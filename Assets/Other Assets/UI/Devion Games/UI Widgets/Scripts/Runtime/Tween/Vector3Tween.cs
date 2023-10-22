using UnityEngine;
using UnityEngine.Events;

namespace DevionGames.UIWidgets
{
	internal struct Vector3Tween : ITweenValue
	{
		private Vector3Tween.Vector3TweenCallback m_Target;
		private Vector3Tween.Vector3TweenFinishCallback m_OnFinish;

		public EasingEquations.EaseType easeType { get; set; }
		private Vector3 m_StartValue;
		public Vector3 startValue
		{
			get { return m_StartValue; }
			set { m_StartValue = value; }
		}
		private Vector3 m_TargetValue;
		public Vector3 targetValue
		{
			get { return m_TargetValue; }
			set { m_TargetValue = value; }
		}

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

			float x = EasingEquations.GetValue(easeType, startValue.x, targetValue.x, floatPercentage);
			float y = EasingEquations.GetValue(easeType, startValue.y, targetValue.y, floatPercentage);
			float z = EasingEquations.GetValue(easeType, startValue.z, targetValue.z, floatPercentage);
			m_Target.Invoke(new Vector3(x, y, z));
		}

		public void AddOnChangedCallback(UnityAction<Vector3> callback)
		{
			m_Target ??= new Vector3TweenCallback();

			m_Target.AddListener(callback);
		}

		public void AddOnFinishCallback(UnityAction callback)
		{
			m_OnFinish ??= new Vector3TweenFinishCallback();

			m_OnFinish.AddListener(callback);
		}

		public void OnFinish()
		{
			if (m_OnFinish != null)
				m_OnFinish.Invoke();
		}

		public class Vector3TweenCallback : UnityEvent<Vector3>
		{
			public Vector3TweenCallback()
			{
			}
		}

		public class Vector3TweenFinishCallback : UnityEvent
		{
			public Vector3TweenFinishCallback()
			{
			}
		}
	}
}