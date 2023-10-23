using UnityEngine;

namespace DevionGames
{
	public interface ICollectionEditor
	{
		string ToolbarName { get; }
		void OnGUI(Rect position);
		void OnEnable();
		void OnDisable();
		void OnDestroy();

	}
}