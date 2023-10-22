using UnityEngine;

namespace DevionGames.UIWidgets
{
	public class UISlot<T> : MonoBehaviour where T : class
	{
		/// <summary>
		/// The item container that holds this slot
		/// </summary>
		public UIContainer<T> Container { get; set; }

		/// <summary>
		/// Index of item container
		/// </summary>
		public int Index { get; set; } = -1;

		private T m_Item;
		/// <summary>
		/// The item this slot is holding
		/// </summary>
		public virtual T ObservedItem
		{
			get
			{
				return m_Item;
			}
			set
			{
				m_Item = value;
				Repaint();
			}
		}

		/// <summary>
		/// Checks if the slot is empty ObservedItem == null
		/// </summary>
		public bool IsEmpty
		{
			get { return ObservedItem == null; }
		}

		/// <summary>
		/// Repaint slot visuals with item information
		/// </summary>
		public virtual void Repaint()
		{
		}

		/// <summary>
		/// Can the item be added to this slot. This does not check if the slot is empty.
		/// </summary>
		/// <param name="item">The item to test adding.</param>
		/// <returns>Returns true if the item can be added.</returns>
		public virtual bool CanAddItem(T item)
		{
			return true;
		}
	}
}