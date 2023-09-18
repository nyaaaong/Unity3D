using System.IO;
using UnityEditor;
using UnityEngine;

public static class Utility
{
	public static void Quit()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public static bool CheckEmpty<T>(T[] array, string arrayName)
	{
		if (array == null)
		{
			Debug.LogError("if (array == null)");
			return false;
		}

		else if (array.Length == 0)
		{
			Debug.LogError("if (" + arrayName + ".Length == 0)");
			return false;
		}

		else
		{
			foreach (T item in array)
			{
				if (item == null)
				{
					Debug.LogError("if (" + arrayName + " == null)");
					return false;
				}
			}
		}

		return true;
	}

	public static T[] Shuffle<T>(T[] array, int seed)
	{
		/*
		피셔 예이츠(Fisher-Yates) 셔플 알고리즘을 사용한다.
		현재 인덱스를 i, 랜덤한 인덱스를 idx라고 친다고 가정하자.
		idx는 Random의 Next 메서드를 이용하여 랜덤값을 받아오게 된다.
		그리고나서 for문을 통하여 i를 배열의 총 개수만큼 반복돌며 array[i]와 array[idx]을 스왑을 시키는데,
		i가 배열의 마지막 인덱스인 경우 idx와 동일한데 스왑을 시키는 불필요한 작업을 수행하게 되므로 마지막 인덱스는 제외시킨다.
		 */
		System.Random rnd = new System.Random(seed);
		int idx, Length = array.Length;
		T temp;

		int Size = Length - 1;

		for (int i = 0; i < Size; ++i)
		{
			idx = rnd.Next(i, Length);

			temp = array[idx];
			array[idx] = array[i];
			array[i] = temp;
		}

		return array;
	}
}
