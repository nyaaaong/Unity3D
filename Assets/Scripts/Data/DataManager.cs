using System;
using System.IO;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
	// 직렬화 된 배열을 저장하기 위한 래핑 클래스
	[Serializable]
	public class Wrapper<T>
	{
		public T[] data;
	}

	private string m_Path;

	private string GetFullPath(Data_Type type)
	{
		if (m_Path == null || m_Path.Length == 0)
			m_Path = Utility.SettingPath();

		return m_Path + type.ToString() + ".json";
	}

	public static void SaveData<T>(T obj, Data_Type type) where T : class
	{
		string data = JsonUtility.ToJson(obj);

		File.WriteAllText(Inst.GetFullPath(type), data);
	}

	public static void SaveDataArray<T>(T[] obj, Data_Type type)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.data = obj;

		string jsonData = JsonUtility.ToJson(wrapper);
		File.WriteAllText(Inst.GetFullPath(type), jsonData);
	}

	public static T LoadData<T>(Data_Type type) where T : class
	{
		string path = Inst.GetFullPath(type);

		if (File.Exists(path))
			return JsonUtility.FromJson<T>(File.ReadAllText(path));

		return null;
	}

	public static T[] LoadDataArray<T>(Data_Type type)
	{
		string path = Inst.GetFullPath(type);

		if (File.Exists(path))
		{
			string jsonData = File.ReadAllText(path);
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonData);
			return wrapper.data;
		}

		else
		{
#if UNITY_EDITOR
			Debug.LogError("File not found: " + path);
#endif
			return null;
		}
	}
}