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
