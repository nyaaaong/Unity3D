using System;
using UnityEngine;

[Serializable]
public class StageInfo
{
	[Serializable]
	public class WaveInfo
	{
		[ReadOnly(true)][SerializeField] private int m_EnemyCount;
		[ReadOnly(true)][SerializeField] private float m_EnemyCountMultiplier;
		[ReadOnly(true)][SerializeField] private float m_SpawnTime;
		[ReadOnly(true)][SerializeField] private float m_SpawnTimeMax;

		public float SpawnTime { get { return m_SpawnTime; } }
		public float SpawnTimeMax { get { return m_SpawnTimeMax; } }

		// 1웨이브는 기본 EnemyCount를 반환하고, 2웨이브부터 EnemyCount에 EnemyCountMultiplier를 거듭 곱해준다.
		// 그리고 소숫점은 올려준다.
		public float EnemyCount(int wave, int waveCount)
		{
			if (wave == 1)
				return m_EnemyCount;

			else
			{
				float result = m_EnemyCount;

				for (; wave < waveCount; ++wave)
				{
					result *= m_EnemyCountMultiplier;
				}

				return Mathf.Ceil(result);
			}
		}
	}

	[SerializeField] private WaveInfo m_WaveInfo;

	[ReadOnly(true)][SerializeField] private int m_WaveCount = 3;
	[ReadOnly(true)][SerializeField] private float m_StageEnemyCountMultiplier;
	[ReadOnly(true)][SerializeField] private float m_StageEnemyHPMultiplier;
	[ReadOnly(true)][SerializeField] private float m_StageEnemyDamageMultiplier = 1.05f;
	[ReadOnly(true)][SerializeField] private float m_StageEnemyFireRateMultiplier;
	[ReadOnly(true)][SerializeField] private float m_StageSpawnTimeMultiplier = 1.2f;
	[ReadOnly(true)][SerializeField] private float m_StageExpMultiplier;

	public int WaveCount { get { return m_WaveCount; } }

	public void SetEnemyTotalStat(CharInfo enemyInfo)
	{
		float hp = enemyInfo.HPMax;
		float dmg = enemyInfo.Damage;
		float fireRate = enemyInfo.FireRateTime;
		float exp = enemyInfo.Exp;

		for (int i = StageManager.StageNum; i > 1; --i)
		{
			hp *= m_StageEnemyHPMultiplier;
			dmg *= m_StageEnemyDamageMultiplier;
			fireRate /= m_StageEnemyFireRateMultiplier;
			exp *= m_StageExpMultiplier;
		}

		enemyInfo.HP = hp;
		enemyInfo.HPMax = hp;
		enemyInfo.Damage = dmg;
		enemyInfo.FireRateTime = fireRate < 0.5f ? 0.5f : fireRate;
		enemyInfo.Exp = Mathf.CeilToInt(exp);
	}

	public float EnemyHP(in CharInfo enemyInfo)
	{
		float result = enemyInfo.HPMax;

		for (int i = StageManager.StageNum; i > 1; --i)
		{
			result *= m_StageEnemyHPMultiplier;
		}

		return result;
	}

	public float EnemyDamage(in CharInfo enemyInfo)
	{
		float result = enemyInfo.Damage;

		for (int i = StageManager.StageNum; i > 1; --i)
		{
			result *= m_StageEnemyDamageMultiplier;
		}

		return result;
	}

	// WaveInfo 내부에서 계산된 EnemyCount에 현재 스테이지에 따라 StageEnemyCountMultiplier를 곱해줄지 말지 정해준다.
	// 2스테이지부터 곱해서 소숫점 올림 후 int로 반환한다.
	public int EnemyCount(int wave)
	{
		float result = m_WaveInfo.EnemyCount(wave, m_WaveCount);

		for (int i = StageManager.StageNum; i > 1; --i)
		{
			result *= m_StageEnemyCountMultiplier;
		}

		return Mathf.CeilToInt(result);
	}

	// WaveInfo 에서 설정된 기본 SpawnTime에 현재 스테이지에 따라 배율을 곱할지 말지 정한다.
	// 만약 SpawnTimeMax를 초과하는 경우 SpawnTimeMax를 반환한다.
	public float SpawnTime()
	{
		int stageNum = StageManager.StageNum;
		float result = m_WaveInfo.SpawnTime;

		for (; stageNum > 1; --stageNum)
		{
			result = m_StageSpawnTimeMultiplier > 1f ? result / m_StageSpawnTimeMultiplier : result / (1f + m_StageSpawnTimeMultiplier);
		}

		result = result < m_WaveInfo.SpawnTimeMax ? m_WaveInfo.SpawnTimeMax : result;

		return result;
	}
}