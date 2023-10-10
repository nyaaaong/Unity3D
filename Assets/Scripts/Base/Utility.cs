using UnityEditor;
using UnityEngine;

public static class Utility
{
	public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
	{
		T obj = Object.Instantiate(original, position, rotation);
		obj.name = original.name;

		return obj;
	}

	public static T Instantiate<T>(T original) where T : Object
	{
		T obj = Object.Instantiate(original);
		obj.name = original.name;

		return obj;
	}

	public static T Instantiate<T>(T original, Transform parent) where T : Object
	{
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

#if UNITY_EDITOR
	// �ݵ�� ���ǹ��� �� �� �� �ٷ� ���� ��쿡�� �ڵ���� �־�� �Ѵ�.
	public static void LogError(string msg)
	{
		Debug.LogError(msg);
	}

	// �ݵ�� ���ǹ��� �� �� �� �ٷ� ���� ��쿡�� �ڵ���� �־�� �Ѵ�.
	public static void Log(string msg)
	{
		Debug.Log(msg);
	}

	public static void CheckEmpty<T>(T obj, string objName)
	{
		if (obj == null || obj.Equals(default(T)))
			Debug.LogError(objName + "�� ����ֽ��ϴ�!");
	}

	public static void CheckEmpty<T>(T[] array, string arrayName)
	{
		if (array == null || array.Length == 0)
			Debug.LogError(arrayName + "�� ����ֽ��ϴ�!");

		else
		{
			foreach (T item in array)
			{
				if (item == null || item.Equals(default(T)))
					Debug.LogError(arrayName + "�� ��Ұ� ����ֽ��ϴ�!");
			}
		}
	}
#endif

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
