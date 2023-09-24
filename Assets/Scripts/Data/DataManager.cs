using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : Singleton<DataManager>
{
	// 직렬화 된 배열을 저장하기 위한 래핑 클래스
	[Serializable]
	public class Wrapper<T>
	{
		public T[] data;
	}

	private Action OnSuccessLoadData;
	private Action OnFailLoadData;
	private string m_Path;
	private string m_SavePath;
	private string[] m_URLInfo =
	{
		"https://raw.githubusercontent.com/nyaaaong/Unity3D/main/Json/AbilityInfo.json",
		"https://raw.githubusercontent.com/nyaaaong/Unity3D/main/Json/CharInfo.json",
		"https://raw.githubusercontent.com/nyaaaong/Unity3D/main/Json/StageInfo.json"
	};

	public static void Init(Action success, Action fail = null)
	{
		// 중복이 아닐때만 추가
		if (Inst.OnSuccessLoadData == null || (Inst.OnSuccessLoadData != null && !Inst.OnSuccessLoadData.GetInvocationList().Contains(success)))
			Inst.OnSuccessLoadData += success;

		if (fail != null && Inst.OnFailLoadData == null || (Inst.OnFailLoadData != null && !Inst.OnFailLoadData.GetInvocationList().Contains(fail)))
			Inst.OnFailLoadData += fail;

		Inst.StartCoroutine(Inst.LoadJSONFromServer());
	}

	private IEnumerator LoadJSONFromServer()
	{
		SettingPath();

		string url, fileName;
		Uri uri;
		bool fail = false;

		for (int i = 0; i < m_URLInfo.Length; ++i)
		{
			url = m_URLInfo[i];
			uri = new Uri(url);
			fileName = Path.GetFileName(uri.LocalPath);

			// UnityWebRequest 생성
			using (UnityWebRequest www = UnityWebRequest.Get(url))
			{
				// 요청 보내기
				yield return www.SendWebRequest();

				// 요청이 성공한 경우
				if (www.result == UnityWebRequest.Result.Success)
					File.WriteAllText(m_Path + fileName, www.downloadHandler.text);

				else
				{
					fail = true;
					break;
				}
			}
		}

		if (!fail)
		{
			if (Inst.OnSuccessLoadData != null)
				Inst.OnSuccessLoadData();
		}

		else
		{
			if (Inst.OnFailLoadData != null)
				Inst.OnFailLoadData();
		}
	}

	private void SettingPath()
	{
		if (string.IsNullOrEmpty(m_Path))
		{
			m_Path = Application.persistentDataPath + "/Data/";

			if (!Directory.Exists(m_Path))
				Directory.CreateDirectory(m_Path);
		}

#if UNITY_EDITOR
		if (string.IsNullOrEmpty(m_SavePath))
		{
			m_SavePath = Directory.GetParent(Application.dataPath).FullName + "/Json/";

			if (!Directory.Exists(m_SavePath))
				Directory.CreateDirectory(m_SavePath);
		}
#endif
	}

	private static string GetFileName(Data_Type type)
	{
		return type.ToString() + ".json";
	}

	public static void SaveData<T>(T obj, Data_Type type) where T : class
	{
		Inst.SettingPath();

		string data = JsonUtility.ToJson(obj);
		string fileName = GetFileName(type);

		File.WriteAllText(Inst.m_Path + fileName, data);
#if UNITY_EDITOR
		if (type != Data_Type.Audio)
			File.WriteAllText(Inst.m_SavePath + fileName, data);
#endif
	}

	public static void SaveDataArray<T>(T[] obj, Data_Type type)
	{
		Inst.SettingPath();

		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.data = obj;

		string jsonData = JsonUtility.ToJson(wrapper);
		string fileName = GetFileName(type);

		File.WriteAllText(Inst.m_Path + fileName, jsonData);
#if UNITY_EDITOR
		if (type != Data_Type.Audio)
			File.WriteAllText(Inst.m_SavePath + fileName, jsonData);
#endif
	}

	public static T LoadData<T>(Data_Type type) where T : class
	{
		Inst.SettingPath();

		string path = Inst.m_Path + GetFileName(type);

		if (File.Exists(path))
			return JsonUtility.FromJson<T>(File.ReadAllText(path));

		return null;
	}

	public static T[] LoadDataArray<T>(Data_Type type)
	{
		Inst.SettingPath();

		string path = Inst.m_Path + GetFileName(type);

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