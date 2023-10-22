﻿using UnityEngine;

namespace DevionGames
{
	[System.Serializable]
	public class StringVariable : Variable
	{
		[SerializeField]
		private string m_Value = string.Empty;

		public string Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}

		public override object RawValue
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = (string)value;
			}
		}

		public override System.Type type
		{
			get
			{
				return typeof(string);
			}
		}

		public StringVariable()
		{
		}

		public StringVariable(string name) : base(name)
		{
		}

		public static implicit operator StringVariable(string value)
		{
			return new StringVariable()
			{
				Value = value
			};
		}
		public static implicit operator string(StringVariable value)
		{
			return value.Value;
		}
	}
}