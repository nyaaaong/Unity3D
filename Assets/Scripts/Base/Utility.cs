using System;

public static class Utility
{
	public static T[] Shuffle<T>(T[] array, int seed)
	{
		Random rnd = new Random(seed);
		int idx, Length = array.Length;
		T temp;

		// 마지막은 섞지 않는다.
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
