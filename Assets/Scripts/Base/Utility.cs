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
			Debug.LogError($"{objName}�� ����ֽ��ϴ�!");
#endif
	}

	public static void CheckEmpty<T>(T[] array, string arrayName)
	{
#if UNITY_EDITOR
		if (array == null || array.Length == 0)
			Debug.LogError($"{arrayName}�� ����ֽ��ϴ�!");

		else
		{
			foreach (T item in array)
			{
				if (item == null || item.Equals(default(T)))
					Debug.LogError($"{arrayName}�� ��Ұ� ����ֽ��ϴ�!");
			}
		}
#endif
	}

	public static T[] Shuffle<T>(T[] array, int seed)
	{
		/*
		�Ǽ� ������(Fisher-Yates) ���� �˰����� ����Ѵ�.
		���� �ε����� i, ������ �ε����� idx��� ģ�ٰ� ��������.
		idx�� Random�� Next �޼��带 �̿��Ͽ� �������� �޾ƿ��� �ȴ�.
		�׸����� for���� ���Ͽ� i�� �迭�� �� ������ŭ �ݺ����� array[i]�� array[idx]�� ������ ��Ű�µ�,
		i�� �迭�� ������ �ε����� ��� idx�� �����ѵ� ������ ��Ű�� ���ʿ��� �۾��� �����ϰ� �ǹǷ� ������ �ε����� ���ܽ�Ų��.
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
