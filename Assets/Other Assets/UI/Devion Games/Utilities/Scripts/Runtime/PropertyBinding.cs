using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace DevionGames
{
	public class PropertyBinding : MonoBehaviour
	{
		[SerializeField]
		private PropertyRef m_Source = null;
		[SerializeField]
		private PropertyRef m_Target = null;
		[SerializeField]
		private Execution m_Execution = Execution.Update;
		[SerializeField]
		private float m_Interval = 0.3f;

		private void Start()
		{
			if (m_Execution == Execution.Start)
			{
				UpdateTarget();
			}

			if (m_Execution == Execution.Interval)
			{
				StartCoroutine(IntervalUpdate());
			}
		}

		private void Update()
		{
			if (m_Execution == Execution.Update)
			{
				UpdateTarget();
			}
		}

		private void LateUpdate()
		{
			if (m_Execution == Execution.LateUpdate)
			{
				UpdateTarget();
			}
		}

		private void FixedUpdate()
		{
			if (m_Execution == Execution.FixedUpdate)
			{
				UpdateTarget();
			}
		}

		private IEnumerator IntervalUpdate()
		{
			while (true)
			{
				yield return new WaitForSeconds(m_Interval);
				UpdateTarget();
			}
		}

		public void UpdateTarget()
		{
			m_Target.SetValue(m_Source.GetValue());
		}

		public enum Execution
		{
			Start,
			Update,
			LateUpdate,
			FixedUpdate,
			Interval
		}

		[System.Serializable]
		public class PropertyRef
		{
			[SerializeField]
			private Component m_Component = null;
			public Component component
			{
				get
				{
					return m_Component;
				}
			}

			private FieldInfo m_Field;
			private PropertyInfo m_Property;

			[SerializeField]
			private string m_PropertyPath = string.Empty;
			public string propertyPath
			{
				get
				{
					return m_PropertyPath;
				}
			}

			public object GetValue()
			{
				if (m_Field == null && m_Property == null)
				{
					CacheProperty();
				}

				if (m_Property != null)
				{
					if (m_Property.CanRead)
						return m_Property.GetValue(m_Component, null);
				}
				else if (m_Field != null)
				{
					return m_Field.GetValue(m_Component);
				}

				return null;
			}

			public bool SetValue(object value)
			{
				if (m_Field == null && m_Property == null && !CacheProperty())
				{
					return false;
				}

				if (m_Field != null)
				{
					m_Field.SetValue(m_Component, value);
					return true;
				}
				else if (m_Property.CanWrite)
				{
					m_Property.SetValue(m_Component, value, null);
					return true;
				}

				return false;
			}

			private bool CacheProperty()
			{
				if (m_Component != null && !string.IsNullOrEmpty(m_PropertyPath))
				{
					Type type = m_Component.GetType();
#if NETFX_CORE
					this.m_Field = type.GetRuntimeField(this.m_PropertyPath);
					this.m_Property = type.GetRuntimeProperty(this.m_PropertyPath);
#else
					m_Field = type.GetField(m_PropertyPath);
					m_Property = type.GetProperty(m_PropertyPath);
#endif
				}
				else
				{
					m_Field = null;
					m_Property = null;
				}

				return m_Field != null || m_Property != null;
			}
		}
	}
}