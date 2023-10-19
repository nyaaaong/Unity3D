using UnityEditor;
using UnityEngine;

public static class Utility
{
	public static void ClearLog()
	{
#if UNITY_EDITOR
		System.Type type = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
		System.Reflection.MethodInfo info = type.GetMethod("Clear");
		info.Invoke(null, null);
#endif
	}

	public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	{
		if (original == null)
			return default(T);

		T obj = Object.Instantiate(original, position, rotation);
		obj.name = original.name;

		return obj;
	}

	public static T Instantiate<T>(T original) where T : Object
	{
		if (original == null)
			return default(T);

		T obj = Object.Instantiate(original);
		obj.name = original.name;

		return obj;
	}

	public static T Instantiate<T>(T original, Transform parent) where T : Object
	{
		if (original == null)
			return default(T);

		T obj = Object.Instantiate(original, parent);
		obj.name = original.name;

		return obj;
	}

	public static void Quit()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public static void LogError(string msg)
	{
#if UNITY_EDITOR
		Debug.LogError(msg);
#endif
	}

	public static void Log(string msg)
	{
#if UNITY_EDITOR
		Debug.Log(msg);
#endif
	}

	public static void CheckEmpty<T>(T obj, string objName)
	{
#if UNITY_EDITOR
		if (obj == null || obj.Equals(default(T)))
			Debug.LogError($"{objName}가 비어있습니다!");
#endif
	}

	public static void CheckEmpty<T>(T[] array, string arrayName)
	{
#if UNITY_EDITOR
		if (array == null || array.Length == 0)
			Debug.LogError($"{arrayName}가 비어있습니다!");

		else
		{
			foreach (T item in array)
			{
				if (item == null || item.Equals(default(T)))
					Debug.LogError($"{arrayName}의 요소가 비어있습니다!");
			}
		}
#endif
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
