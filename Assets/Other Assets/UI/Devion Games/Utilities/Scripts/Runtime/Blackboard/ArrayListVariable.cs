﻿using System.Collections;

namespace DevionGames
{
	[System.Serializable]
	public class ArrayListVariable : Variable
	{

		private ArrayList m_Value = new ArrayList();

		public ArrayList Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}

		public override object RawValue
		{
			get
			{
				m_Value ??= new ArrayList();
				return m_Value;
			}
			set
			{
				m_Value = (ArrayList)value;
			}
		}

		public override System.Type type
		{
			get
			{
				return typeof(ArrayList);
			}
		}

		public ArrayListVariable()
		{
		}

		public ArrayListVariable(string name) : base(name)
		{
		}

		public static implicit operator ArrayListVariable(ArrayList value)
		{
			return new ArrayListVariable()
			{
				Value = value
			};
		}

		public static implicit operator ArrayList(ArrayListVariable value)
		{
			return value.Value;
		}
	}
}