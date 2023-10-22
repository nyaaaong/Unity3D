using System;

namespace DevionGames
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CategoryAttribute : Attribute
	{
		public string Category { get; }

		public CategoryAttribute(string category)
		{
			Category = category;
		}
	}
}