using System;

namespace DevionGames
{
	public sealed class ComponentMenu : Attribute
	{
		public string componentMenu { get; }

		public ComponentMenu(string menuName)
		{
			componentMenu = menuName;
		}
	}
}