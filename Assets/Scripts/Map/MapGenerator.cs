using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : BaseScript
{
	#region Coord
	public struct Coord
	{
		public int x;
		public int y;

		public Coord(int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public Coord(Vector2 v)
		{
			x = (int)v.x;
			y = (int)v.y;
		}

		public static bool operator ==(Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator !=(Coord c1, Coord c2)
		{
			return !(c1 == c2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Coord))
				return false;

			Coord other = (Coord)obj;

			return x == other.x && y == other.y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;

				hash = hash * 23 + x.GetHashCode();
				hash = hash * 23 + y.GetHashCode();

				return hash;
			}
		}
	}
	#endregion
	#region Map
	[Serializable]
	public class Map
	{
		public Coord m_MapSize;
		public float m_WallPercent;
		public int m_Seed;
		public Transform[] m_Wall;
		public Queue<Transform> m_ShuffledWall;

		public Coord MapCenter { get { return new Coord(m_MapSize.x / 2, m_MapSize.y / 2); } }
	}
	#endregion
	#region MapSetting
	[Serializable]
	public class MapSetting
	{
		[ReadOnly(true)][SerializeField][Range(0.1f, 0.5f)] private float m_WallPercentMin;
		[ReadOnly(true)][SerializeField][Range(0.1f, 0.5f)] private float m_WallPercentMax;
		[ReadOnly(true)][SerializeField] private Vector2 m_MapSizeMin;
		[ReadOnly(true)][SerializeField] private Vector2 m_MapSizeMax;
		[ReadOnly(true)][SerializeField] private string m_ChildName = "Generated Map";
		[ReadOnly(true)][SerializeField] private float m_TileSize;
		[ReadOnly(true)][SerializeField] private float m_WallHeight;
		[ReadOnly(true)][SerializeField] private Transform m_TilePrefeb;
		[ReadOnly(true)][SerializeField] private Transform[] m_WallPrefeb;
		[ReadOnly(true)][SerializeField] private Transform m_NavFloor;
		[ReadOnly(true)][SerializeField] private Transform m_NavMaskPrefeb;

		public float WallPercentMin { get { return m_WallPercentMin; } }
		public float WallPercentMax { get { return m_WallPercentMax; } }
		public Vector2 MapSizeMin { get { return m_MapSizeMin; } }
		public Vector2 MapSizeMax { get { return m_MapSizeMax; } }
		public string ChildName { get { return m_ChildName; } }
		public float TileSize { get { return m_TileSize; } }
		public float WallHeight { get { return m_WallHeight; } }
		public Transform TilePrefeb { get { return m_TilePrefeb; } }
		public Transform[] WallPrefeb { get { return m_WallPrefeb; } }
		public Transform NavFloor { get { return m_NavFloor; } }
		public Transform NavMaskPrefeb { get { return m_NavMaskPrefeb; } }
	}
	#endregion

	[SerializeField] private MapSetting m_Setting = new MapSetting();

	private Map m_Map;
	private List<Coord> m_TileCoordList = null;
	private Queue<Coord> m_ShuffledTile;
	private Queue<Coord> m_ShuffledOpenTile;
	private Transform[,] m_TileMap;
	private Transform[] m_NavMask;
	private Vector3 m_TileHalfScale;

	public Coord MapSize { get { return m_Map.m_MapSize; } }

	private Vector2 RandomRange(Vector2 v1, Vector2 v2)
	{
		Vector2 result = new Vector2();

		result.x = UnityEngine.Random.Range(v1.x, v2.x);
		result.y = UnityEngine.Random.Range(v1.y, v2.y);

		return result;
	}

	public void CreateRandomMap()
	{
		m_Map.m_MapSize = new Coord(RandomRange(m_Setting.MapSizeMin, m_Setting.MapSizeMax));
		m_Map.m_WallPercent = UnityEngine.Random.Range(m_Setting.WallPercentMin, m_Setting.WallPercentMax);
		m_Map.m_Seed = UnityEngine.Random.Range(0, 10000);

		int length = m_Setting.WallPrefeb.Length;

		m_Map.m_Wall = new Transform[length];

		for (int i = 0; i < length; ++i)
		{
			m_Map.m_Wall[i] = m_Setting.WallPrefeb[i];
		}

		m_Map.m_ShuffledWall = new Queue<Transform>(Utility.Shuffle(m_Map.m_Wall, m_Map.m_Seed));

		m_TileHalfScale = m_Setting.TilePrefeb.localScale * 0.5f;
	}

	public bool Init()
	{
#if UNITY_EDITOR
		if (!m_Setting.TilePrefeb)
		{
			Debug.LogError("if (!m_Setting.TilePrefeb)");
			return false;
		}

		if (!Utility.CheckEmpty(m_Setting.WallPrefeb, "m_Setting.WallPrefeb"))
			return false;

		if (!m_Setting.NavFloor)
		{
			Debug.LogError("if (!m_Setting.NavFloor)");
			return false;
		}

		if (!m_Setting.NavMaskPrefeb)
		{
			Debug.LogError("if (!m_Setting.NavMaskPrefeb)");
			return false;
		}
#endif

		m_Map = new Map();
		m_NavMask = new Transform[(int)NavMask_Position.Max];

		CreateRandomMap();

		return true;
	}

	public void Generator()
	{
		m_TileMap = new Transform[m_Map.m_MapSize.x, m_Map.m_MapSize.y];

		GetComponent<BoxCollider>().size = new Vector3(m_Map.m_MapSize.x * m_Setting.TileSize, 0.05f, m_Map.m_MapSize.y * m_Setting.TileSize);

		// MapEditor에서 오브젝트가 선택되면 계속 호출되므로 m_TileCoordList가 있는 경우 전부 지워준다
		if (m_TileCoordList == null)
			m_TileCoordList = new List<Coord>();

		else
			m_TileCoordList.Clear();

		// 반복문에서 계속 m_Map.m_MapSize를 호출하지 않게 미리 변수를 할당한다
		float maxX = m_Map.m_MapSize.x, maxY = m_Map.m_MapSize.y;

		// 모든 좌표를 등록한다
		for (int x = 0; x < maxX; ++x)
		{
			for (int y = 0; y < maxY; ++y)
			{
				m_TileCoordList.Add(new Coord(x, y));
			}
		}

		// 모든 좌표를 섞어주고 m_ShuffledTile에 넣어준다.
		m_ShuffledTile = new Queue<Coord>(Utility.Shuffle(m_TileCoordList.ToArray(), m_Map.m_Seed));

		// 생성된 타일들을 하나로 묶어주기 위해 ChildName 이름을 가진 오브젝트를 자식으로 추가하고
		// 추가한 자식을 타일들의 부모로 지정한다.
		Transform child = transform.Find(m_Setting.ChildName);

		if (child)
			DestroyImmediate(child.gameObject);

		Transform newMap = new GameObject(m_Setting.ChildName).transform;
		newMap.parent = transform;

		for (int x = 0; x < maxX; ++x)
		{
			for (int y = 0; y < maxY; ++y)
			{
				Vector3 tilePos = CoordToRotation(x, y);
				Transform newTile = Instantiate(m_Setting.TilePrefeb, tilePos, Quaternion.Euler(Vector3.right * 90f));
				newTile.localScale = Vector3.one * m_Setting.TileSize;
				newTile.parent = newMap;
				newTile.name = "Tile";

				m_TileMap[x, y] = newTile;
			}
		}

		// 
		bool[,] wallMap = new bool[(int)maxX, (int)maxY];

		int wallCount = (int)(maxX * maxY * m_Map.m_WallPercent);
		int curWallCount = 0;
		List<Coord> openCoordList = new List<Coord>(m_TileCoordList);

		for (int i = 0; i < wallCount; ++i)
		{
			Coord randCoord = GetRandomCoord();
			wallMap[randCoord.x, randCoord.y] = true;
			++curWallCount;

			if (randCoord != m_Map.MapCenter && IsAllAccessPossible(wallMap, curWallCount))
			{
				Vector3 wallPos = CoordToRotation(randCoord.x, randCoord.y);

				Transform newWall = Instantiate(GetRandomWall(), wallPos, Quaternion.identity);
				newWall.localScale = Vector3.one * m_Setting.TileSize;
				newWall.parent = newMap;
				newWall.name = "Wall";

				openCoordList.Remove(randCoord);
			}

			else
			{
				wallMap[randCoord.x, randCoord.y] = false;
				--curWallCount;
			}
		}

		// 벽이 아닌 좌표를 섞어주고 m_ShuffledOpenTile에 넣어준다.
		m_ShuffledOpenTile = new Queue<Coord>(Utility.Shuffle(openCoordList.ToArray(), m_Map.m_Seed));

		// 벽과 근접한 타일을 거른다. (몬스터가 이 곳에 생성되는 경우 벽과 끼어버리는 현상이 발생한다)
		int count = m_ShuffledOpenTile.Count;
		Coord coord;

		for (int i = 0; i < count; ++i)
		{
			coord = m_ShuffledOpenTile.Dequeue();

			if (coord.x != 0 && coord.x != maxX - 1 &&
				coord.y != 0 && coord.y != maxY - 1)
				m_ShuffledOpenTile.Enqueue(coord);
		}

		CreateNavMask(NavMask_Position.Left, newMap, "NavMask Left");
		CreateNavMask(NavMask_Position.Right, newMap, "NavMask Right");
		CreateNavMask(NavMask_Position.Top, newMap, "NavMask Top");
		CreateNavMask(NavMask_Position.Bottom, newMap, "NavMask Bottom");

		// NavFloor는 쿼드라서 90도 회전했기 때문에 z축이 아닌 y축을 바꿔줘야 한다.
		m_Setting.NavFloor.localScale = new Vector3(m_Setting.MapSizeMax.x, m_Setting.MapSizeMax.y) * m_Setting.TileSize;
	}

	private void CreateNavMask(NavMask_Position maskPos, Transform newMap, string name)
	{
		/*
		맵 바깥은 이동하지 못하게 Nav Floor의 크기와 실제 맵 크기를 이용한 연산으로 NavMaskPrefeb를 생성해준다.

		위치의 경우,	(가로 일 때 x, 세로일 때 y이며 설명은 가로 기준)
		MapSizeMax.x == 10, m_MapSize.x == 4 일 때,

		MapSizeMax.x	(-5)ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ-(5)
		MapSize.x			(-2)ㅡㅡㅡㅡㅡㅡ	(2)
		mask							   | . |

		구하고자 하는 것은 mask의 . 위치이다.
		MapSize.x / 2 를 하면 mask의 시작 부분인 2가 나오고,
		(MapSizeMax.x - MapSize.x) / 2 를 하면 mask의 길이인 3이 나오며, 2로 한번 더 나누면 mask의 절반 길이가 나온다. 그러므로 (MapSizeMax.x - MapSize.x) / 4 이다.

		최종적으로 mask의 시작 위치 + 절반 길이 를 하면 .의 위치가 나오므로,
		MapSize.x / 2 + (MapSizeMax.x - MapSize.x) / 4
		= MapSize.x * 2 / 4 + (MapSizeMax.x - MapSize.x) / 4

		∴ (MapSize.x + MapSizeMax.x) / 4
		가 나온다.

		크기의 경우, 이전에 구했던 mask의 길이인 (MapSizeMax.x - MapSize.x) / 2 를 x부분에 넣어준다.
		*/

		int index = (int)maskPos;
		Vector3 pos;
		Vector3 scale;

		if (maskPos == NavMask_Position.Left || maskPos == NavMask_Position.Right)
		{
			if (maskPos == NavMask_Position.Left)
				pos = Vector3.left;

			else
				pos = Vector3.right;

			pos *= (m_Map.m_MapSize.x + m_Setting.MapSizeMax.x) * 0.25f * m_Setting.TileSize;
			scale = new Vector3((m_Setting.MapSizeMax.x - m_Map.m_MapSize.x) * 0.5f, m_Setting.WallHeight, m_Map.m_MapSize.y) * m_Setting.TileSize;
		}

		else
		{
			if (maskPos == NavMask_Position.Top)
				pos = Vector3.forward;

			else
				pos = Vector3.back;

			pos *= (m_Map.m_MapSize.y + m_Setting.MapSizeMax.y) * 0.25f * m_Setting.TileSize;
			scale = new Vector3(m_Setting.MapSizeMax.x, m_Setting.WallHeight, (m_Setting.MapSizeMax.y - m_Map.m_MapSize.y) * 0.5f) * m_Setting.TileSize;
		}


		m_NavMask[index] = Instantiate(m_Setting.NavMaskPrefeb, pos, Quaternion.identity);
		m_NavMask[index].localScale = scale;
		m_NavMask[index].parent = newMap;
		m_NavMask[index].name = name;
	}

	private Transform GetRandomWall()
	{
		if (m_Map.m_ShuffledWall == null)
			return null;

		Transform randTr = m_Map.m_ShuffledWall.Dequeue();
		m_Map.m_ShuffledWall.Enqueue(randTr);

		return randTr;
	}

	public Transform GetRandomOpenTile()
	{
		if (m_ShuffledOpenTile == null)
			return null;

		Coord randCoord = m_ShuffledOpenTile.Dequeue();
		m_ShuffledOpenTile.Enqueue(randCoord);

		return m_TileMap[randCoord.x, randCoord.y];
	}

	public Coord GetRandomCoord()
	{
		Coord randCoord = m_ShuffledTile.Dequeue();
		m_ShuffledTile.Enqueue(randCoord);

		return randCoord;
	}

	private bool IsAllAccessPossible(bool[,] wallMap, int curWallCount)
	{
		int wallMapX = wallMap.GetLength(0);
		int wallMapY = wallMap.GetLength(1);

		bool[,] flag = new bool[wallMapX, wallMapY];
		Queue<Coord> queue = new Queue<Coord>();

		queue.Enqueue(m_Map.MapCenter);

		// 맵 가운데는 항상 비어있기에 flag를 true로 해준다.
		flag[m_Map.MapCenter.x, m_Map.MapCenter.y] = true;

		Coord tile;
		int nearX, nearY;
		int blankTileCount = 1, targetblankTileCount = m_Map.m_MapSize.x * m_Map.m_MapSize.y - curWallCount;

		// 큐에 등록된 타일과 인접한 벽이 아닌 타일을 찾고 blankTileCount를 증가시킨다.
		// 이 것으로 캐릭터가 실제 이동될 수 있는 타일만 찾게 된다.
		while (queue.Count > 0)
		{
			tile = queue.Dequeue();

			for (int x = -1; x <= 1; ++x)
			{
				for (int y = -1; y <= 1; ++y)
				{
					nearX = tile.x + x;
					nearY = tile.y + y;

					// 현재 타일 (tile)을 기준 십자 모양을 검사한다.
					if (x == 0 || y == 0)
					{
						// 인덱스는 0 ~ wallMapX,Y - 1 까지만 검사한다.
						if (nearX >= 0 && nearX < wallMapX &&
							nearY >= 0 && nearY < wallMapY)
						{
							// 검사하지 않은 타일이면서 벽이 아닌 타일인 경우
							if (!flag[nearX, nearY] && !wallMap[nearX, nearY])
							{
								// 검사했음을 표시하고 큐에 넣은 후 빈 타일 카운트를 증가시킨다.
								flag[nearX, nearY] = true;
								queue.Enqueue(new Coord(nearX, nearY));

								++blankTileCount;
							}
						}
					}
				}
			}
		}

		// 비어있는 타일의 개수와 이동할 수 있는 타일의 개수가 일치하는 지 여부를 반환한다.
		return targetblankTileCount == blankTileCount;
	}

	private Vector3 CoordToRotation(int x, int y)
	{
		// 맵 중앙을 기준으로 리턴한다.
		return new Vector3(-m_Map.m_MapSize.x * 0.5f + m_TileHalfScale.x + x, 0f, -m_Map.m_MapSize.y * 0.5f + m_TileHalfScale.y + y) * m_Setting.TileSize;
	}
}
