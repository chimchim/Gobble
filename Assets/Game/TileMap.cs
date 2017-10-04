using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileMap
{
	public enum MineralType
	{
		Rock,
		Iron,
		Gold,
		Copper
	}
	public enum TileType
	{
		Air,
		Bot,
		BotLeftCorner,
		BotRightCorner,
		Middle,
		Middle2,
		Middle3,
		MiddleLeft,
		MiddleRight,
		Top,
		TopLeft,
		TopRight,
		Boundry
	}
	//private List<Vector2> _grassLevels = new List<Vector2>();
	//private List<Vector2> _boundries = new List<Vector2>();
	//private List<Vector2> _bottoms = new List<Vector2>();
	//private List<Vector2> _walls = new List<Vector2>();
	private Material[] _darkMats;
	private List<List<Vector2>> _islands = new List<List<Vector2>>();
	private Material diffMat;
	private TileType[,] BlockTypes;
	private MineralType[,] MineralTypes;
	private int[,] BlockIslandSize;
	// Side blocks, kolla längden och ta mitten
	public GameObject[,] Blocks;
	private MineralsGenVariables minsVariables;

	#region SpriteVariables
	private Sprite _rockBotMat;
	private Sprite _rockBotLeftCornerMat;
	private Sprite _rockBotRightCornerMat;
	private Sprite _rockMiddleMat;
	private Sprite _rockMiddle2Mat;
	private Sprite _rockMiddle3Mat;
	private Sprite _rockMiddleLeftMat;
	private Sprite _rockMiddleRightMat;
	private Sprite _rockTopMat;
	private Sprite _rockTopLeftMat;
	private Sprite _rockTopRightMat;

	private Sprite _goldBotMat;
	private Sprite _goldBotLeftCornerMat;
	private Sprite _goldBotRightCornerMat;
	private Sprite _goldMiddleMat;
	private Sprite _goldMiddle2Mat;
	private Sprite _goldMiddle3Mat;
	private Sprite _goldMiddleLeftMat;
	private Sprite _goldMiddleRightMat;
	private Sprite _goldTopMat;
	private Sprite _goldTopLeftMat;
	private Sprite _goldTopRightMat;

	private Sprite _ironBotMat;
	private Sprite _ironBotLeftCornerMat;
	private Sprite _ironBotRightCornerMat;
	private Sprite _ironMiddleMat;
	private Sprite _ironMiddle2Mat;
	private Sprite _ironMiddle3Mat;
	private Sprite _ironMiddleLeftMat;
	private Sprite _ironMiddleRightMat;
	private Sprite _ironTopMat;
	private Sprite _ironTopLeftMat;
	private Sprite _ironTopRightMat;

	private Sprite _copperBotMat;
	private Sprite _copperBotLeftCornerMat;
	private Sprite _copperBotRightCornerMat;
	private Sprite _copperMiddleMat;
	private Sprite _copperMiddle2Mat;
	private Sprite _copperMiddle3Mat;
	private Sprite _copperMiddleLeftMat;
	private Sprite _copperMiddleRightMat;
	private Sprite _copperTopMat;
	private Sprite _copperTopLeftMat;
	private Sprite _copperTopRightMat;
	#endregion

	private bool[,] _enlisted;
	private int _extraIron;
	private int _extraCopper;
	public void InitiateMap()
	{
		minsVariables = GameObject.FindObjectOfType<GameUnity>().MineralsGen;
		SetMaterials();
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;
		Blocks = new GameObject[fullWidhth, fullHeight];
		BlockTypes = new TileType[fullWidhth, fullHeight];
		MineralTypes = new MineralType[fullWidhth, fullHeight];
		_enlisted = new bool[GameUnity.FullWidth, GameUnity.FullHeight];
		BlockIslandSize = new int[fullWidhth, fullHeight];

		GameObject parentCube = new GameObject();
		parentCube.name = "Tiles";

		for (int x = 0; x < fullWidhth; x++)
		{
			for (int y = 0; y < fullHeight; y++)
			{

				BlockTypes[x, y] = TileType.Air;
				int rightLayerBound = (GameUnity.MapWidth + GameUnity.WidhtBound - 1);
				int topLayerBound = (GameUnity.MapHeight + GameUnity.HeightBound) + GameUnity.BottomBoundOffset + GameUnity.TopBoundOffset - 1;
				if ((x < GameUnity.WidhtBound || x > rightLayerBound) || (y < GameUnity.HeightBound || y > topLayerBound))
				{
					GameObject cube = new GameObject();
					cube.transform.parent = parentCube.transform;
					cube.AddComponent<SpriteRenderer>();
					cube.GetComponent<SpriteRenderer>().material = diffMat;
					cube.AddComponent<BoxCollider2D>();
					cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
					cube.layer = LayerMask.NameToLayer("Collideable");
					cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
					Blocks[x, y] = cube;
					//_boundries.Add(new Vector2(x, y));
					BlockTypes[x, y] = TileType.Boundry;
				}

			}
		}

		Vector2 shift = new Vector2(0, 0); // play with this to shift map around
		float zoom = 0.1f; // play with this to zoom into the noise field
		int startY = GameUnity.HeightBound + GameUnity.BottomBoundOffset;
		int maxY = GameUnity.MapHeight;
		int maxX = GameUnity.MapWidth;
		for (int x = 0; x < maxX; x++)
			for (int y = 0; y < maxY; y++)
			{
				Vector2 pos = zoom * (new Vector2(x, y)) + shift;
				float noise = Mathf.PerlinNoise(pos.x, pos.y);

				int posX = x + GameUnity.WidhtBound;
				int posY = y + startY;
				if (noise < 0.16f && GameUnity.GenerateSmallIsland)
				{
					GameObject cube = new GameObject();
					cube.transform.parent = parentCube.transform;
					cube.AddComponent<SpriteRenderer>();
					cube.GetComponent<SpriteRenderer>().material = diffMat;
					cube.AddComponent<BoxCollider2D>();
					cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
					cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
					cube.layer = LayerMask.NameToLayer("Collideable");
					Blocks[posX, posY] = cube;
				}
				else if (noise < 0.5f)
				{
				}
				else if (noise < 0.9f && noise > 0.5f)
				{
					GameObject cube = new GameObject();
					cube.transform.parent = parentCube.transform;
					cube.AddComponent<SpriteRenderer>();
					cube.GetComponent<SpriteRenderer>().material = diffMat;
					cube.AddComponent<BoxCollider2D>();
					cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
					cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
					cube.layer = LayerMask.NameToLayer("Collideable");
					Blocks[posX, posY] = cube;
				}
				else
				{
					GameObject cube = new GameObject();
					cube.transform.parent = parentCube.transform;
					cube.AddComponent<SpriteRenderer>();
					cube.GetComponent<SpriteRenderer>().material = diffMat;
					cube.AddComponent<BoxCollider2D>();
					cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
					cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
					cube.layer = LayerMask.NameToLayer("Collideable");
					Blocks[posX, posY] = cube;
				}



			}

		SetBlockType();

	}
	
	public void GenerateMinerals()
	{
		GetIslands();
		CreateRocks();
		CreateGold();
		CreateIron();
		CreateCopper();
	}

	public void CreateRocks()
	{
		int startX = GameUnity.WidhtBound;
		int endX = GameUnity.MapWidth + startX;
		int startY = GameUnity.HeightBound + GameUnity.BottomBoundOffset;
		int endY = GameUnity.MapHeight + startY;
		for (int x = startX; x < endX; x++)
		{
			for (int y = startY; y < endY; y++)
			{
				int chance = Random.Range(0, minsVariables.RockMiddleOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = MineralType.Rock;
					//if (BlockTypes[x, y] == TileType.Bot)
					//{
					//	Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockBotMat;
					//}
					//if (BlockTypes[x, y] == TileType.BotLeftCorner)
					//{
					//	Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockBotLeftCornerMat;
					//}
					//if (BlockTypes[x, y] == TileType.BotRightCorner)
					//{
					//	Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockBotRightCornerMat;
					//}
					if (BlockTypes[x, y] == TileType.Middle)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockMiddleMat;
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockMiddle2Mat;
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockMiddle3Mat;
					}
					if (BlockTypes[x, y] == TileType.MiddleLeft)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockMiddleLeftMat;
					}
					if (BlockTypes[x, y] == TileType.MiddleRight)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockMiddleRightMat;
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockTopMat;
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockTopLeftMat;
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _rockTopRightMat;
					}
				}
			}
		}
	}
	public void CreateGold()
	{
		int startX = GameUnity.WidhtBound;
		int endX = GameUnity.MapWidth + startX;
		int startY = GameUnity.HeightBound + GameUnity.BottomBoundOffset;
		int endY = GameUnity.MapHeight + startY;
		for (int x = startX; x < endX; x++)
		{
			for (int y = startY; y < endY; y++)
			{
				var type = BlockTypes[x, y];
				int chance = 1;
				int extraChance = 0;
				int smallIsland = BlockIslandSize[x, y];
				if (smallIsland < minsVariables.GoldChanceIslandLimit)
				{
					extraChance = minsVariables.MaxExtraGoldChance;
				}
				if (type == TileType.Bot)
				{
					chance = Random.Range(0, minsVariables.GoldBotOnIn);
					if (chance == 0)
					{
						MineralTypes[x, y] = MineralType.Gold;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldBotMat;
					}
				}
				if (type == TileType.BotLeftCorner)
				{
					chance = Random.Range(0, minsVariables.GoldLeftRightBot - extraChance);
					if (chance == 0)
					{
						MineralTypes[x, y] = MineralType.Gold;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldBotLeftCornerMat;
					}
				}
				if (type == TileType.BotRightCorner)
				{
					chance = Random.Range(0, minsVariables.GoldLeftRightBot - extraChance);
					if (chance == 0)
					{
						MineralTypes[x, y] = MineralType.Gold;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldBotRightCornerMat;
					}
				}
				chance = Random.Range(0, minsVariables.GoldRandomOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = MineralType.Gold;
					if (BlockTypes[x, y] == TileType.Middle)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldMiddleMat;
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldMiddle2Mat;
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldMiddle3Mat;
					}
					if (BlockTypes[x, y] == TileType.MiddleLeft)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldMiddleLeftMat;
					}
					if (BlockTypes[x, y] == TileType.MiddleRight)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldMiddleRightMat;
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldTopMat;
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldTopLeftMat;
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _goldTopRightMat;
					}
				}
			}
		}
	}
	public void CreateIron()
	{
		int startX = GameUnity.WidhtBound;
		int endX = GameUnity.MapWidth + startX;
		int startY = GameUnity.HeightBound + GameUnity.BottomBoundOffset;
		int endY = GameUnity.MapHeight + startY;
		int[,] level = new int[GameUnity.FullWidth, GameUnity.FullHeight];
		for (int i = 0; i < _islands.Count; i++)
		{
			var tierList = GetIslandLevel(_islands[i]);

			for (int j = 1; j < tierList.Count; j++)
			{
				//int tierIndex = Mathf.Clamp(j - 1, 3, 4);
				for (int k = 0; k < tierList[j].Count; k++)
				{
					int x = (int)tierList[j][k].x;
					int y = (int)tierList[j][k].y;
					level[x, y] = j;
				}
			}
		}

		for (int x = startX; x < endX; x++)
		{
			for (int y = startY; y < endY; y++)
			{
				var type = BlockTypes[x, y];
				int chance = 1;
				int extraChance = 0;

				if (MineralTypes[x, y] == MineralType.Gold)
				{
					_extraIron++;
					continue;
				}

				if (BlockTypes[x, y] == TileType.Middle)
				{
					int extra = level[x, y] * minsVariables.IronlevelChanceIncrease;	
					chance = Random.Range(0, minsVariables.IronMiddleOnIn - extra);

					if (chance == 0)
					{
						MineralTypes[x, y] = MineralType.Iron;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironMiddleMat;
					}
					continue;
				}
				
				chance = Random.Range(0, minsVariables.IronRandomOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = MineralType.Iron;
					if (BlockTypes[x, y] == TileType.Bot)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironBotMat;
					}
					if (BlockTypes[x, y] == TileType.BotLeftCorner)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironBotLeftCornerMat;
					}
					if (BlockTypes[x, y] == TileType.BotRightCorner)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironBotRightCornerMat;
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironMiddle2Mat;
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironMiddle3Mat;
					}
					if (BlockTypes[x, y] == TileType.MiddleLeft)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironMiddleLeftMat;
					}
					if (BlockTypes[x, y] == TileType.MiddleRight)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironMiddleRightMat;
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironTopMat;
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironTopLeftMat;
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _ironTopRightMat;
					}
				}
			}
		}
	}
	public void CreateCopper()
	{
		int startX = GameUnity.WidhtBound;
		int endX = GameUnity.MapWidth + startX;
		int startY = GameUnity.HeightBound + GameUnity.BottomBoundOffset;
		int endY = GameUnity.MapHeight + startY;

		for (int x = startX; x < endX; x++)
		{
			for (int y = startY; y < endY; y++)
			{
				var type = BlockTypes[x, y];
				int chance = 1;
				int extraChance = 0;

				if (MineralTypes[x, y] == MineralType.Gold || MineralTypes[x, y] == MineralType.Iron)
				{
					_extraCopper++;
					continue;
				}

				if (BlockTypes[x, y] == TileType.MiddleRight)
				{
					chance = Random.Range(0, minsVariables.CopperSideOneIn);

					if (chance == 0)
					{
						MineralTypes[x, y] = MineralType.Copper;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperMiddleRightMat;
					}
					continue;
				}

				if (BlockTypes[x, y] == TileType.MiddleLeft)
				{
					chance = Random.Range(0, minsVariables.CopperSideOneIn);

					if (chance == 0)
					{
						MineralTypes[x, y] = MineralType.Copper;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperMiddleLeftMat;
					}
					continue;
				}

				chance = Random.Range(0, minsVariables.CopperRandomOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = MineralType.Copper;
					if (BlockTypes[x, y] == TileType.Middle)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperMiddleMat;
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperMiddle2Mat;
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperMiddle3Mat;
					}
					if (BlockTypes[x, y] == TileType.Bot)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperBotMat;
					}
					if (BlockTypes[x, y] == TileType.BotLeftCorner)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperBotLeftCornerMat;
					}
					if (BlockTypes[x, y] == TileType.BotRightCorner)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperBotRightCornerMat;
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperTopMat;
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperTopLeftMat;
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = _copperTopRightMat;
					}
				}
			}
		}
	}
	// räkna med islands här, ge varje tile en island count
	//public int ExtraGoldStones(int x, int y, TileType type)
	//{
	//
	//}
	// Island
	private List<List<Vector2>> GetIslandLevel(List<Vector2> island)
	{
		var innerIsland = new List<List<Vector2>>();
		innerIsland.Add(new List<Vector2>());
		innerIsland[0].AddRange(island);

		int currentIndex = 0;
		while (innerIsland.Count > currentIndex)
		{
			var inner = innerIsland[currentIndex];
			bool first = true;
			for (int i = 0; i < inner.Count; i++)
			{
				var top = inner[i] + new Vector2(0, 1);
				var bot = inner[i] + new Vector2(0, -1);
				var right = inner[i] + new Vector2(1, 0);
				var left = inner[i] + new Vector2(-1, 0);
				var isMiddle = inner.Contains(top) && inner.Contains(bot) && inner.Contains(right) && inner.Contains(left);

				if (isMiddle)
				{
					if (first)
					{
						first = false;
						innerIsland.Add(new List<Vector2>());
					}
					innerIsland[currentIndex + 1].Add(inner[i]);
				}
			}
			currentIndex++;
		}
		return innerIsland;
	}
	private List<Vector2> MakeIsland(int x, int y)
	{
		List<Vector2> currentIsland = new List<Vector2>();
		bool foundBoundry = false;
		int maxtrax = 0;
		currentIsland.Add(new Vector2(x, y));
		var current = new Vector2(x, y);

		int currentIndex = 0;
		//Debug.Log("Left " + left + " leftpos " + left * 1.28f);
		while (currentIsland.Count > currentIndex)
		{
			if (currentIsland.Count > currentIndex)
			{
				current = currentIsland[currentIndex];
				var left = new Vector2(current.x - 1, current.y);
				var down = new Vector2(current.x, current.y - 1);
				var right = new Vector2(current.x + 1, current.y);
				var up = new Vector2(current.x, current.y + 1);

				var blockType = BlockTypes[(int)left.x, (int)left.y];
				bool enlisted = _enlisted[(int)left.x, (int)left.y];
				if (!enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)left.x, (int)left.y] = true;
					currentIsland.Add(left);
				}

				blockType = BlockTypes[(int)down.x, (int)down.y];
				enlisted = _enlisted[(int)down.x, (int)down.y];
				if (!enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)down.x, (int)down.y] = true;
					currentIsland.Add(down);

				}

				blockType = BlockTypes[(int)right.x, (int)right.y];
				enlisted = _enlisted[(int)right.x, (int)right.y];
				if (!enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)right.x, (int)right.y] = true;
					currentIsland.Add(right);

				}

				blockType = BlockTypes[(int)up.x, (int)up.y];
				enlisted = _enlisted[(int)up.x, (int)up.y];
				if (!enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)up.x, (int)up.y] = true;
					currentIsland.Add(up);

				}
				currentIndex++;
			}
		}
		for (int i = 0; i < currentIsland.Count; i++)
		{
			int xPos = (int)currentIsland[i].x;
			int yPos = (int)currentIsland[i].y;
			BlockIslandSize[xPos, yPos] = currentIsland.Count;
		}
		return currentIsland;
	}
	private void GetIslands()
	{
		if (!GameUnity.GenerateIslands)
			return;

		
		Vector2 firstPos = new Vector2(GameUnity.WidhtBound + (GameUnity.WidhtBound * 0.28f), GameUnity.HeightBound + (GameUnity.HeightBound * 0.28f));
		int islandIndex = 0;
		int startY = GameUnity.HeightBound + GameUnity.BottomBoundOffset;
		int maxY = GameUnity.FullHeight - GameUnity.HeightBound - GameUnity.TopBoundOffset;

		for (int x = GameUnity.WidhtBound; x < GameUnity.MapWidth; x++)
		{
			for (int y = startY; y < maxY; y++)
			{
				if (BlockTypes[x, y] == TileType.Air || _enlisted[x, y])
				{
					continue;
				}

				var newList = MakeIsland(x, y);

				if (newList != null)
				{
					_islands.Add(new List<Vector2>());
					_islands[islandIndex] = newList;
					islandIndex++;
				}
			}
		}
		_enlisted = null;
	}

	private void SetBlockType()
	{
		Sprite newMat = null;
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;
		for (int x = 0; x < fullWidhth; x++)
		{
			for (int y = 0; y < fullHeight; y++)
			{
				Vector2 pos = new Vector2(x, y);


				var top = ((y + 1) < fullHeight) && (Blocks[x, y + 1] != null);
				var bot = ((y - 1) >= 0) && (Blocks[x, y - 1] != null);
				var right = ((x + 1) < fullWidhth) && (Blocks[x + 1, y] != null);
				var left = ((x - 1) >= 0) && (Blocks[x - 1, y] != null);
				if (Blocks[x, y] == null)
				{
					continue;
				}
				if (!top && !right && !bot && !left)
				{
					GameObject.Destroy(Blocks[x, y]);
					BlockTypes[x, y] = TileType.Air;
				}
				if (!top && !right)
				{
					newMat = Resources.Load("Tiles/TopRight", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "TopRight";
					//_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.TopRight;
				}
				if (!top && !left)
				{
					newMat = Resources.Load("Tiles/TopLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "TopLeft";
					//_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.TopLeft;
				}
				if (!top && right && left)
				{
					newMat = Resources.Load("Tiles/Top", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Top";
					//_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Top;
				}
				if (top && right && left && bot)
				{
					newMat = Resources.Load("Tiles/Middle", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Middle";
					//_walls.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Middle;
				}
				if (top && !right && bot)
				{
					newMat = Resources.Load("Tiles/MiddleRight", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "MiddleRight";
					//_walls.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.MiddleRight;
				}
				if (top && !left && bot)
				{
					newMat = Resources.Load("Tiles/MiddleLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "MiddleLeft";
					//_walls.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.MiddleLeft;
				}
				if (top && !right && !bot)
				{
					newMat = Resources.Load("Tiles/BotRightCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotRightCorner";
					//_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.BotRightCorner;
				}
				if (top && !left && !bot)
				{
					newMat = Resources.Load("Tiles/BotLeftCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotLeftCorner";
					//_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.BotLeftCorner;
				}
				if (top && left && !bot && right)
				{
					newMat = Resources.Load("Tiles/Bot", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Bot";
					//_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Bot;
				}

				Blocks[x, y].GetComponent<SpriteRenderer>().sprite = newMat;
			}
		}


		for (int x = 0; x < fullWidhth; x++)
		{
			for (int y = 0; y < fullHeight; y++)
			{
				if (Blocks[x, y] == null)
				{
					continue;
				}
				if (Blocks[x, y].name == "Middle")
				{
					var middleRight = ((x + 1) < fullWidhth) && ((y + 1) < fullHeight) && (Blocks[x + 1, y + 1] != null);
					var middleLeft = ((x - 1) >= 0) && ((y + 1) < fullHeight) && (Blocks[x - 1, y + 1] != null);
					if (!middleRight)
					{
						newMat = Resources.Load("Tiles/Middle3", typeof(Sprite)) as Sprite;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = newMat;
						BlockTypes[x, y] = TileType.Middle3;
					}
					if (!middleLeft)
					{
						newMat = Resources.Load("Tiles/Middle2", typeof(Sprite)) as Sprite;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = newMat;
						BlockTypes[x, y] = TileType.Middle2;
					}
				}
			}
		}
	}
	private void SetMaterials()
	{
		_darkMats = new Material[5];
		for (int i = 1; i < 6; i++)
		{
			_darkMats[i - 1] = Resources.Load("Material/SpriteDiffuseDark"+i.ToString(), typeof(Material)) as Material;
		}
		diffMat = Resources.Load("Material/SpriteDiffuse", typeof(Material)) as Material;
		
		_rockBotMat = Resources.Load("Tiles/Rocks/Bot", typeof(Sprite)) as Sprite;
		_rockBotLeftCornerMat = Resources.Load("Tiles/Rocks/BotLeftCorner", typeof(Sprite)) as Sprite;
		_rockBotRightCornerMat = Resources.Load("Tiles/Rocks/BotRightCorner", typeof(Sprite)) as Sprite;
		_rockMiddleMat = Resources.Load("Tiles/Rocks/Middle", typeof(Sprite)) as Sprite;
		_rockMiddle2Mat = Resources.Load("Tiles/Rocks/Middle2", typeof(Sprite)) as Sprite;
		_rockMiddle3Mat = Resources.Load("Tiles/Rocks/Middle3", typeof(Sprite)) as Sprite;
		_rockMiddleLeftMat = Resources.Load("Tiles/Rocks/MiddleLeft", typeof(Sprite)) as Sprite;
		_rockMiddleRightMat = Resources.Load("Tiles/Rocks/MiddleRight", typeof(Sprite)) as Sprite;
		_rockTopMat = Resources.Load("Tiles/Rocks/Top", typeof(Sprite)) as Sprite;
		_rockTopLeftMat = Resources.Load("Tiles/Rocks/TopLeft", typeof(Sprite)) as Sprite;
		_rockTopRightMat = Resources.Load("Tiles/Rocks/TopRight", typeof(Sprite)) as Sprite;

		_goldBotMat = Resources.Load("Tiles/Minerals/Gold/Bot", typeof(Sprite)) as Sprite;
		_goldBotLeftCornerMat = Resources.Load("Tiles/Minerals/Gold/BotLeftCorner", typeof(Sprite)) as Sprite;
		_goldBotRightCornerMat = Resources.Load("Tiles/Minerals/Gold/BotRightCorner", typeof(Sprite)) as Sprite;
		_goldMiddleMat = Resources.Load("Tiles/Minerals/Gold/Middle", typeof(Sprite)) as Sprite;
		_goldMiddle2Mat = Resources.Load("Tiles/Minerals/Gold/Middle2", typeof(Sprite)) as Sprite;
		_goldMiddle3Mat = Resources.Load("Tiles/Minerals/Gold/Middle3", typeof(Sprite)) as Sprite;
		_goldMiddleLeftMat = Resources.Load("Tiles/Minerals/Gold/MiddleLeft", typeof(Sprite)) as Sprite;
		_goldMiddleRightMat = Resources.Load("Tiles/Minerals/Gold/MiddleRight", typeof(Sprite)) as Sprite;
		_goldTopMat = Resources.Load("Tiles/Minerals/Gold/Top", typeof(Sprite)) as Sprite;
		_goldTopLeftMat = Resources.Load("Tiles/Minerals/Gold/TopLeft", typeof(Sprite)) as Sprite;
		_goldTopRightMat = Resources.Load("Tiles/Minerals/Gold/TopRight", typeof(Sprite)) as Sprite;

		_copperBotMat = Resources.Load("Tiles/Minerals/Copper/Bot", typeof(Sprite)) as Sprite;
		_copperBotLeftCornerMat = Resources.Load("Tiles/Minerals/Copper/BotLeftCorner", typeof(Sprite)) as Sprite;
		_copperBotRightCornerMat = Resources.Load("Tiles/Minerals/Copper/BotRightCorner", typeof(Sprite)) as Sprite;
		_copperMiddleMat = Resources.Load("Tiles/Minerals/Copper/Middle", typeof(Sprite)) as Sprite;
		_copperMiddle2Mat = Resources.Load("Tiles/Minerals/Copper/Middle2", typeof(Sprite)) as Sprite;
		_copperMiddle3Mat = Resources.Load("Tiles/Minerals/Copper/Middle3", typeof(Sprite)) as Sprite;
		_copperMiddleLeftMat = Resources.Load("Tiles/Minerals/Copper/MiddleLeft", typeof(Sprite)) as Sprite;
		_copperMiddleRightMat = Resources.Load("Tiles/Minerals/Copper/MiddleRight", typeof(Sprite)) as Sprite;
		_copperTopMat = Resources.Load("Tiles/Minerals/Copper/Top", typeof(Sprite)) as Sprite;
		_copperTopLeftMat = Resources.Load("Tiles/Minerals/Copper/TopLeft", typeof(Sprite)) as Sprite;
		_copperTopRightMat = Resources.Load("Tiles/Minerals/Copper/TopRight", typeof(Sprite)) as Sprite;

		_ironBotMat = Resources.Load("Tiles/Minerals/Iron/Bot", typeof(Sprite)) as Sprite;
		_ironBotLeftCornerMat = Resources.Load("Tiles/Minerals/Iron/BotLeftCorner", typeof(Sprite)) as Sprite;
		_ironBotRightCornerMat = Resources.Load("Tiles/Minerals/Iron/BotRightCorner", typeof(Sprite)) as Sprite;
		_ironMiddleMat = Resources.Load("Tiles/Minerals/Iron/Middle", typeof(Sprite)) as Sprite;
		_ironMiddle2Mat = Resources.Load("Tiles/Minerals/Iron/Middle2", typeof(Sprite)) as Sprite;
		_ironMiddle3Mat = Resources.Load("Tiles/Minerals/Iron/Middle3", typeof(Sprite)) as Sprite;
		_ironMiddleLeftMat = Resources.Load("Tiles/Minerals/Iron/MiddleLeft", typeof(Sprite)) as Sprite;
		_ironMiddleRightMat = Resources.Load("Tiles/Minerals/Iron/MiddleRight", typeof(Sprite)) as Sprite;
		_ironTopMat = Resources.Load("Tiles/Minerals/Iron/Top", typeof(Sprite)) as Sprite;
		_ironTopLeftMat = Resources.Load("Tiles/Minerals/Iron/TopLeft", typeof(Sprite)) as Sprite;
		_ironTopRightMat = Resources.Load("Tiles/Minerals/Iron/TopRight", typeof(Sprite)) as Sprite;
	}
	public void SetDarkMaterials()
	{
		for (int i = 0; i < _islands.Count; i++)
		{
			var tierList = GetIslandLevel(_islands[i]);

			for (int j = 1; j < tierList.Count; j++)
			{   
				int tierIndex = Mathf.Clamp(j - 1, 3, 4);
				for (int k = 0; k < tierList[j].Count; k++)
				{
					int x = (int)tierList[j][k].x;
					int y = (int)tierList[j][k].y;
					//Blocks[x, y].GetComponent<SpriteRenderer>().material = _darkMats[tierIndex];
					//int x = (int)tierList[j][k].x; = 
				}
			}
		}
	}
}