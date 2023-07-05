using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : BaseScript
{
	#region Coord
	[Serializable]
	public struct Coord
	{
		public int x;
		public int y;

		public Coord(int _x, int _y)
		{
			x = _x;
			y = _y;
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
		[Range(0, 1)] public float m_WallPercent;
		public int m_Seed;
		public float m_WallHeightMin;
		public float m_WallHeightMax;
		public Color m_ForgroundColor;
		public Color m_BackgroundColor;

		public Coord MapCenter { get { return new Coord(m_MapSize.x / 2, m_MapSize.y / 2); } }
	}
	#endregion

	[SerializeField] private Map[] m_Maps;
	[SerializeField] private int m_MapIndex;
	[SerializeField] private Transform m_TilePrefeb;
	[SerializeField] private Transform m_WallPrefeb;
	[SerializeField] private Transform m_NavFloor;
	[SerializeField] private Transform m_NavMaskPrefeb;
	[SerializeField] private Vector2 m_MapSizeMax;
	[SerializeField, Range(0, 1)] private float m_OutLinePercent;
	[SerializeField] private string m_ChildName = "Generated Map";
	[SerializeField] private float m_TileSize;

	private List<Coord> m_TileCoordList = null;
	private Queue<Coord> m_ShuffledTileCoord;
	private Queue<Coord> m_ShuffledOpenTileCoord;
	private Map m_CurMap;
	private Transform[,] m_TileMap;

	public Coord MapSize { get { return m_CurMap.m_MapSize; } }

	public void Generator()
	{
		m_CurMap = m_Maps[m_MapIndex];
		m_TileMap = new Transform[m_CurMap.m_MapSize.x, m_CurMap.m_MapSize.y];
		System.Random rand = new System.Random(m_CurMap.m_Seed);

		GetComponent<BoxCollider>().size = new Vector3(m_CurMap.m_MapSize.x * m_TileSize, 0.05f, m_CurMap.m_MapSize.y * m_TileSize);

		// MapEditor에서 오브젝트가 선택되면 계속 호출되므로 m_TileCoordList가 있는 경우 전부 지워준다
		if (m_TileCoordList == null)
			m_TileCoordList = new List<Coord>();

		else
			m_TileCoordList.Clear();

		// 반복문에서 계속 m_CurMap.m_MapSize를 호출하지 않게 미리 변수를 할당한다
		float maxX = m_CurMap.m_MapSize.x, maxY = m_CurMap.m_MapSize.y;

		// 모든 좌표를 등록한다
		for (int x = 0; x < maxX; ++x)
		{
			for (int y = 0; y < maxY; ++y)
			{
				m_TileCoordList.Add(new Coord(x, y));
			}
		}

		// 모든 좌표를 섞어주고 m_ShuffledTileCoord에 넣어준다.
		m_ShuffledTileCoord = new Queue<Coord>(Utility.Shuffle(m_TileCoordList.ToArray(), m_CurMap.m_Seed));

		// 생성된 타일들을 하나로 묶어주기 위해 m_ChildName 이름을 가진 오브젝트를 자식으로 추가하고
		// 추가한 자식을 타일들의 부모로 지정한다.
		Transform child = transform.Find(m_ChildName);

		if (child)
			DestroyImmediate(child.gameObject);

		Transform newMap = new GameObject(m_ChildName).transform;
		newMap.parent = transform;

		for (int x = 0; x < maxX; ++x)
		{
			for (int y = 0; y < maxY; ++y)
			{
				Vector3 tilePos = CoordToRotation(x, y);
				// 쿼드는 처음 생성하면 벽처럼 세워지므로 오른쪽으로 90도 회전시켜줘서 바닥처럼 보이게 해줘야 한다.
				Transform newTile = Instantiate(m_TilePrefeb, tilePos, Quaternion.Euler(Vector3.right * 90f));
				newTile.localScale = Vector3.one * (1 - m_OutLinePercent) * m_TileSize;
				newTile.parent = newMap;

				m_TileMap[x, y] = newTile;
			}
		}

		// 
		bool[,] wallMap = new bool[(int)maxX, (int)maxY];

		int wallCount = (int)(maxX * maxY * m_CurMap.m_WallPercent);
		int curWallCount = 0;
		List<Coord> openCoordList = new List<Coord>(m_TileCoordList);

		for (int i = 0; i < wallCount; ++i)
		{
			Coord randCoord = GetRandomCoord();
			wallMap[randCoord.x, randCoord.y] = true;
			++curWallCount;

			if (randCoord != m_CurMap.MapCenter && IsAllAccessPossible(wallMap, curWallCount))
			{
				float wallHeight = Mathf.Lerp(m_CurMap.m_WallHeightMin, m_CurMap.m_WallHeightMax, (float)rand.NextDouble());
				Vector3 wallPos = CoordToRotation(randCoord.x, randCoord.y);
				Transform newWall = Instantiate(m_WallPrefeb, wallPos + Vector3.up * wallHeight * 0.5f, Quaternion.identity);
				newWall.localScale = new Vector3((1 - m_OutLinePercent) * m_TileSize, wallHeight, (1 - m_OutLinePercent) * m_TileSize);
				newWall.parent = newMap;

				Renderer wallRenderer = newWall.GetComponent<Renderer>();
				Material wallMarterial = new Material(wallRenderer.sharedMaterial);
				float colorPercent = randCoord.y / (float)m_CurMap.m_MapSize.y;
				wallMarterial.color = Color.Lerp(m_CurMap.m_ForgroundColor, m_CurMap.m_BackgroundColor, colorPercent);
				wallRenderer.sharedMaterial = wallMarterial;

				openCoordList.Remove(randCoord);
			}

			else
			{
				wallMap[randCoord.x, randCoord.y] = false;
				--curWallCount;
			}
		}

		// 벽이 아닌 좌표를 섞어주고 m_ShuffledOpenTileCoord에 넣어준다.
		m_ShuffledOpenTileCoord = new Queue<Coord>(Utility.Shuffle(openCoordList.ToArray(), m_CurMap.m_Seed));

		/*
		맵 바깥은 이동하지 못하게 Nav Floor의 크기와 실제 맵 크기를 이용한 연산으로 m_NavMaskPrefeb를 생성해준다.

		위치의 경우,	(가로 일 때 x, 세로일 때 y이며 설명은 가로 기준)
		m_MapSizeMax.x == 10, m_MapSize.x == 4 일 때,

		m_MapSizeMax.x	(-5)ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ-(5)
		m_MapSize.x			(-2)ㅡㅡㅡㅡㅡㅡ	(2)
		mask							   | . |

		구하고자 하는 것은 mask의 . 위치이다.
		m_MapSize.x / 2 를 하면 mask의 시작 부분인 2가 나오고,
		(m_MapSizeMax.x - m_MapSize.x) / 2 를 하면 mask의 길이인 3이 나오며, 2로 한번 더 나누면 mask의 절반 길이가 나온다. 그러므로 (m_MapSizeMax.x - m_MapSize.x) / 4 이다.

		최종적으로 mask의 시작 위치 + 절반 길이 를 하면 .의 위치가 나오므로,
		m_MapSize.x / 2 + (m_MapSizeMax.x - m_MapSize.x) / 4
		= m_MapSize.x * 2 / 4 + (m_MapSizeMax.x - m_MapSize.x) / 4
		
		∴ (m_MapSize.x + m_MapSizeMax.x) / 4
		가 나온다.

		크기의 경우, 이전에 구했던 mask의 길이인 (m_MapSizeMax.x - m_MapSize.x) / 2 를 x부분에 넣어준다.
		*/
		Transform maskLeft = Instantiate(m_NavMaskPrefeb, Vector3.left * (m_CurMap.m_MapSize.x + m_MapSizeMax.x) * 0.25f * m_TileSize, Quaternion.identity);
		maskLeft.localScale = new Vector3((m_MapSizeMax.x - m_CurMap.m_MapSize.x) * 0.5f, 5f, m_CurMap.m_MapSize.y) * m_TileSize;
		maskLeft.parent = newMap;

		Transform maskRight = Instantiate(m_NavMaskPrefeb, Vector3.right * (m_CurMap.m_MapSize.x + m_MapSizeMax.x) * 0.25f * m_TileSize, Quaternion.identity);
		maskRight.localScale = new Vector3((m_MapSizeMax.x - m_CurMap.m_MapSize.x) * 0.5f, 5f, m_CurMap.m_MapSize.y) * m_TileSize;
		maskRight.parent = newMap;

		Transform maskTop = Instantiate(m_NavMaskPrefeb, Vector3.forward * (m_CurMap.m_MapSize.y + m_MapSizeMax.y) * 0.25f * m_TileSize, Quaternion.identity);
		maskTop.localScale = new Vector3(m_MapSizeMax.x, 5f, (m_MapSizeMax.y - m_CurMap.m_MapSize.y) * 0.5f) * m_TileSize;
		maskTop.parent = newMap;

		Transform maskBottom = Instantiate(m_NavMaskPrefeb, Vector3.back * (m_CurMap.m_MapSize.y + m_MapSizeMax.y) * 0.25f * m_TileSize, Quaternion.identity);
		maskBottom.localScale = new Vector3(m_MapSizeMax.x, 5f, (m_MapSizeMax.y - m_CurMap.m_MapSize.y) * 0.5f) * m_TileSize;
		maskBottom.parent = newMap;

		// m_NavFloor는 쿼드라서 90도 회전했기 때문에 z축이 아닌 y축을 바꿔줘야 한다.
		m_NavFloor.localScale = new Vector3(m_MapSizeMax.x, m_MapSizeMax.y) * m_TileSize;
	}

	public Transform GetRandomOpenTile()
	{
		Coord randCoord = m_ShuffledOpenTileCoord.Dequeue();
		m_ShuffledOpenTileCoord.Enqueue(randCoord);

		return m_TileMap[randCoord.x, randCoord.y];
	}

	public Coord GetRandomCoord()
	{
		Coord randCoord = m_ShuffledTileCoord.Dequeue();
		m_ShuffledTileCoord.Enqueue(randCoord);

		return randCoord;
	}

	protected override void Awake()
	{
		base.Awake();

		if (!m_TilePrefeb)
			Debug.LogError("if (!m_TilePrefeb)");

		Generator();
	}

	private bool IsAllAccessPossible(bool[,] wallMap, int curWallCount)
	{
		int wallMapX = wallMap.GetLength(0);
		int wallMapY = wallMap.GetLength(1);

		bool[,] flag = new bool[wallMapX, wallMapY];
		Queue<Coord> queue = new Queue<Coord>();

		queue.Enqueue(m_CurMap.MapCenter);

		// 맵 가운데는 항상 비어있기에 flag를 true로 해준다.
		flag[m_CurMap.MapCenter.x, m_CurMap.MapCenter.y] = true;

		Coord tile;
		int nearX, nearY;
		int blankTileCount = 1, targetblankTileCount = (int)(m_CurMap.m_MapSize.x * m_CurMap.m_MapSize.y - curWallCount);

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
		return new Vector3(-m_CurMap.m_MapSize.x * 0.5f + 0.5f + x, 0f, -m_CurMap.m_MapSize.y * 0.5f + 0.5f + y) * m_TileSize;
	}
}
