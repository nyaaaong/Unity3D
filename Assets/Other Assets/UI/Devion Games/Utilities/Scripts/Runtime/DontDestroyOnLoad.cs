using UnityEngine;

namespace DevionGames
{
	public class DontDestroyOnLoad : MonoBehaviour {
		private void Awake(){
			DontDestroyOnLoad (gameObject);
		}
	}
}