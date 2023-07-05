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

		// MapEditor���� ������Ʈ�� ���õǸ� ��� ȣ��ǹǷ� m_TileCoordList�� �ִ� ��� ���� �����ش�
		if (m_TileCoordList == null)
			m_TileCoordList = new List<Coord>();

		else
			m_TileCoordList.Clear();

		// �ݺ������� ��� m_CurMap.m_MapSize�� ȣ������ �ʰ� �̸� ������ �Ҵ��Ѵ�
		float maxX = m_CurMap.m_MapSize.x, maxY = m_CurMap.m_MapSize.y;

		// ��� ��ǥ�� ����Ѵ�
		for (int x = 0; x < maxX; ++x)
		{
			for (int y = 0; y < maxY; ++y)
			{
				m_TileCoordList.Add(new Coord(x, y));
			}
		}

		// ��� ��ǥ�� �����ְ� m_ShuffledTileCoord�� �־��ش�.
		m_ShuffledTileCoord = new Queue<Coord>(Utility.Shuffle(m_TileCoordList.ToArray(), m_CurMap.m_Seed));

		// ������ Ÿ�ϵ��� �ϳ��� �����ֱ� ���� m_ChildName �̸��� ���� ������Ʈ�� �ڽ����� �߰��ϰ�
		// �߰��� �ڽ��� Ÿ�ϵ��� �θ�� �����Ѵ�.
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
				// ����� ó�� �����ϸ� ��ó�� �������Ƿ� ���������� 90�� ȸ�������༭ �ٴ�ó�� ���̰� ����� �Ѵ�.
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

		// ���� �ƴ� ��ǥ�� �����ְ� m_ShuffledOpenTileCoord�� �־��ش�.
		m_ShuffledOpenTileCoord = new Queue<Coord>(Utility.Shuffle(openCoordList.ToArray(), m_CurMap.m_Seed));

		/*
		�� �ٱ��� �̵����� ���ϰ� Nav Floor�� ũ��� ���� �� ũ�⸦ �̿��� �������� m_NavMaskPrefeb�� �������ش�.

		��ġ�� ���,	(���� �� �� x, ������ �� y�̸� ������ ���� ����)
		m_MapSizeMax.x == 10, m_MapSize.x == 4 �� ��,

		m_MapSizeMax.x	(-5)�ѤѤѤѤѤѤѤѤѤ�-(5)
		m_MapSize.x			(-2)�ѤѤѤѤѤ�	(2)
		mask							   | . |

		���ϰ��� �ϴ� ���� mask�� . ��ġ�̴�.
		m_MapSize.x / 2 �� �ϸ� mask�� ���� �κ��� 2�� ������,
		(m_MapSizeMax.x - m_MapSize.x) / 2 �� �ϸ� mask�� ������ 3�� ������, 2�� �ѹ� �� ������ mask�� ���� ���̰� ���´�. �׷��Ƿ� (m_MapSizeMax.x - m_MapSize.x) / 4 �̴�.

		���������� mask�� ���� ��ġ + ���� ���� �� �ϸ� .�� ��ġ�� �����Ƿ�,
		m_MapSize.x / 2 + (m_MapSizeMax.x - m_MapSize.x) / 4
		= m_MapSize.x * 2 / 4 + (m_MapSizeMax.x - m_MapSize.x) / 4
		
		�� (m_MapSize.x + m_MapSizeMax.x) / 4
		�� ���´�.

		ũ���� ���, ������ ���ߴ� mask�� ������ (m_MapSizeMax.x - m_MapSize.x) / 2 �� x�κп� �־��ش�.
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

		// m_NavFloor�� ����� 90�� ȸ���߱� ������ z���� �ƴ� y���� �ٲ���� �Ѵ�.
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

		// �� ����� �׻� ����ֱ⿡ flag�� true�� ���ش�.
		flag[m_CurMap.MapCenter.x, m_CurMap.MapCenter.y] = true;

		Coord tile;
		int nearX, nearY;
		int blankTileCount = 1, targetblankTileCount = (int)(m_CurMap.m_MapSize.x * m_CurMap.m_MapSize.y - curWallCount);

		// ť�� ��ϵ� Ÿ�ϰ� ������ ���� �ƴ� Ÿ���� ã�� blankTileCount�� ������Ų��.
		// �� ������ ĳ���Ͱ� ���� �̵��� �� �ִ� Ÿ�ϸ� ã�� �ȴ�.
		while (queue.Count > 0)
		{
			tile = queue.Dequeue();

			for (int x = -1; x <= 1; ++x)
			{
				for (int y = -1; y <= 1; ++y)
				{
					nearX = tile.x + x;
					nearY = tile.y + y;

					// ���� Ÿ�� (tile)�� ���� ���� ����� �˻��Ѵ�.
					if (x == 0 || y == 0)
					{
						// �ε����� 0 ~ wallMapX,Y - 1 ������ �˻��Ѵ�.
						if (nearX >= 0 && nearX < wallMapX &&
							nearY >= 0 && nearY < wallMapY)
						{
							// �˻����� ���� Ÿ���̸鼭 ���� �ƴ� Ÿ���� ���
							if (!flag[nearX, nearY] && !wallMap[nearX, nearY])
							{
								// �˻������� ǥ���ϰ� ť�� ���� �� �� Ÿ�� ī��Ʈ�� ������Ų��.
								flag[nearX, nearY] = true;
								queue.Enqueue(new Coord(nearX, nearY));

								++blankTileCount;
							}
						}
					}
				}
			}
		}

		// ����ִ� Ÿ���� ������ �̵��� �� �ִ� Ÿ���� ������ ��ġ�ϴ� �� ���θ� ��ȯ�Ѵ�.
		return targetblankTileCount == blankTileCount;
	}

	private Vector3 CoordToRotation(int x, int y)
	{
		// �� �߾��� �������� �����Ѵ�.
		return new Vector3(-m_CurMap.m_MapSize.x * 0.5f + 0.5f + x, 0f, -m_CurMap.m_MapSize.y * 0.5f + 0.5f + y) * m_TileSize;
	}
}
