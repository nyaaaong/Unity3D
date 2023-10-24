using System;
using UnityEngine;

[Serializable]
public class StageData
{
	[Serializable]
	public class WaveData
	{
		[ReadOnly(true)][SerializeField] private int m_MonsterCount;
		[ReadOnly(true)][SerializeField] private float m_MonsterCountMultiplier;
		[ReadOnly(true)][SerializeField] private float m_SpawnTime;

		public float SpawnTime => m_SpawnTime;

		// 1웨이브는 기본 MonsterCount를 반환하고, 2웨이브부터 MonsterCount에 MonsterCountMultiplier를 거듭 곱해준다.
		// 그리고 소숫점은 올려준다.
		public float MonsterCount(int waveCount)
		{
			int wave = StageManager.Wave;

			if (wave == 1)
				return m_MonsterCount;

			else
			{
				float result = m_MonsterCount;

				for (; wave < waveCount; ++wave)
				{
					result *= m_MonsterCountMultiplier;
				}

				return Mathf.Ceil(result);
			}
		}
	}

	[SerializeField] private WaveData m_WaveData;

	[ReadOnly(true)][SerializeField] private int m_WaveCount = 3;
	[ReadOnly(true)][SerializeField] private float m_MonsterCountMultiplier;
	[ReadOnly(true)][SerializeField] private float m_MonsterHPMultiplier;
	[ReadOnly(true)][SerializeField] private float m_MonsterDamageMultiplier = 1.05f;
	[ReadOnly(true)][SerializeField] private float m_MonsterFireRateMultiplier;
	[ReadOnly(true)][SerializeField] private float m_SpawnTimeMultiplier = 1.2f;
	[ReadOnly(true)][SerializeField] private float m_PlayerExpMaxMultiplier;
	[ReadOnly(true)][SerializeField] private float m_PlayerPerLevelExpMaxMultiplier = 1.1f;

	public int WaveCount => m_WaveCount;

	private int GetPlayerExpMax(CharData PlayerData)
	{
		float exp = PlayerData.Exp;

		for (int i = PlayerData.Level; i > 1; --i)
		{
			exp *= m_PlayerPerLevelExpMaxMultiplier;
		}

		return Mathf.CeilToInt(exp);
	}

	public int RefreshPlayerExpMax(CharData PlayerData)
	{
		float exp = GetPlayerExpMax(PlayerData);

		for (int i = StageManager.Stage; i > 1; --i)
		{
			exp *= m_PlayerExpMaxMultiplier;
		}

		PlayerData.DynamicExp = Mathf.CeilToInt(exp);

		return PlayerData.DynamicExp;
	}

	public void SetMonsterTotalStat(CharData MonsterData)
	{
		float hp = MonsterData.HPMax;
		float dmg = MonsterData.Damage;
		float fireRate = MonsterData.FireRateTime;

		for (int i = StageManager.Stage; i > 1; --i)
		{
			hp *= m_MonsterHPMultiplier;
			dmg *= m_MonsterDamageMultiplier;
			fireRate /= m_MonsterFireRateMultiplier;
		}

		MonsterData.HP = hp;
		MonsterData.HPMax = hp;
		MonsterData.Damage = dmg;
		MonsterData.FireRateTime = fireRate < 0.5f ? 0.5f : fireRate;
	}

	public float MonsterHP(in CharData MonsterData)
	{
		float result = MonsterData.HPMax;

		for (int i = StageManager.Stage; i > 1; --i)
		{
			result *= m_MonsterHPMultiplier;
		}

		return result;
	}

	public float MonsterDamage(in CharData MonsterData)
	{
		float result = MonsterData.Damage;

		for (int i = StageManager.Stage; i > 1; --i)
		{
			result *= m_MonsterDamageMultiplier;
		}

		return result;
	}

	// WaveData 내부에서 계산된 MonsterCount에 현재 스테이지에 따라 MonsterCountMultiplier를 곱해줄지 말지 정해준다.
	// 2스테이지부터 곱해서 소숫점 올림 후 int로 반환한다.
	public int MonsterCount()
	{
		float result = m_WaveData.MonsterCount(m_WaveCount);

		for (int i = StageManager.Stage; i > 1; --i)
		{
			result *= m_MonsterCountMultiplier;
		}

		return Mathf.CeilToInt(result);
	}

	// WaveData 에서 설정된 기본 SpawnTime에 현재 스테이지에 따라 배율을 곱할지 말지 정한다.
	// 만약 SpawnTimeMax를 초과하는 경우 SpawnTimeMax를 반환한다.
	public float SpawnTime()
	{
		int stageNum = StageManager.Stage;
		float result = m_WaveData.SpawnTime;

		for (; stageNum > 1; --stageNum)
		{
			result /= m_SpawnTimeMultiplier;
		}

		result = result < 0f ? 0f : result;

		return result;
	}
}