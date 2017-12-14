using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileMap
{
	public enum IngredientType
	{
		Normal,
		Rock,
		Copper,
		Iron,
		Gold
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

	private List<List<Vector2>> _islands = new List<List<Vector2>>();
	private Material diffMat;

	private Material[] _darkMats;
	private TileType[,] BlockTypes;
	private IngredientType[,] MineralTypes;
	private int[,] BlockIslandSize;
	public GameObject[,] Blocks;
	public Transform[,] Minerals;
	private int[] Mods = new int[5];
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
	private Transform _mineralParent;


	public GameObject CreateBlock(int x, int y)
	{
		GameObject cube = new GameObject();
		GameObject crackObj = new GameObject();
		crackObj.transform.parent = cube.transform;
		crackObj.AddComponent<SpriteRenderer>();
		crackObj.transform.localPosition = new Vector3(0, 0, -0.3f);
		cube.AddComponent<SpriteRenderer>();
		cube.GetComponent<SpriteRenderer>().material = diffMat;
		cube.AddComponent<BoxCollider2D>();
		cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
		cube.AddComponent<BlockComponent>().IngredientType = IngredientType.Normal;
		cube.GetComponent<BlockComponent>().Mod = minsVariables.NormalMod;
		cube.GetComponent<BlockComponent>().X = x;
		cube.GetComponent<BlockComponent>().Y = y;
		cube.GetComponent<BlockComponent>().Renderer = crackObj.GetComponent<SpriteRenderer>();
		cube.layer = LayerMask.NameToLayer("Collideable");
		cube.tag = "cube";
		return cube;
	}
	public void InitiateMap(GameManager game)
	{
		minsVariables = GameObject.FindObjectOfType<GameUnity>().MineralsGen;
		SetMaterials();
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;
		Minerals = new Transform[fullWidhth, fullHeight];
		Blocks = new GameObject[fullWidhth, fullHeight];
		BlockTypes = new TileType[fullWidhth, fullHeight];
		MineralTypes = new IngredientType[fullWidhth, fullHeight];
		_enlisted = new bool[GameUnity.FullWidth, GameUnity.FullHeight];
		BlockIslandSize = new int[fullWidhth, fullHeight];
		Mods[0] = minsVariables.NormalMod;
		Mods[1] = minsVariables.RocksMod;
		Mods[2] = minsVariables.CopperMod;
		Mods[3] = minsVariables.IronMod;
		Mods[4] = minsVariables.GoldMod;
		GameObject parentCube = new GameObject();
		parentCube.name = "Tiles";
		_mineralParent = parentCube.transform;
		for (int x = 0; x < fullWidhth; x++)
		{
			for (int y = 0; y < fullHeight; y++)
			{

				BlockTypes[x, y] = TileType.Air;
				int rightLayerBound = (GameUnity.MapWidth + GameUnity.WidhtBound - 1);
				int topLayerBound = (GameUnity.MapHeight + GameUnity.HeightBound) + GameUnity.BottomBoundOffset + GameUnity.TopBoundOffset - 1;
				if ((x < GameUnity.WidhtBound || x > rightLayerBound) || (y < GameUnity.HeightBound || y > topLayerBound))
				{
					var cube = CreateBlock(x, y);
					cube.GetComponent<BlockComponent>().TileType = TileType.Boundry;
					cube.transform.parent = parentCube.transform;
					cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
					Blocks[x, y] = cube;
					BlockTypes[x, y] = TileType.Boundry;
				}

			}
		}

		Vector2 shift = new Vector2(0, 0); // play with this to shift map around
		float zoom = GameUnity.PerlinZoom; // play with this to zoom into the noise field
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
					var cube = CreateBlock(posX, posY);
					cube.transform.parent = parentCube.transform;
					cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
					Blocks[posX, posY] = cube;
				}
				else if (noise < 0.5f)
				{
				}
				else if (noise < 0.9f && noise > 0.5f)
				{
					var cube = CreateBlock(posX, posY);
					cube.transform.parent = parentCube.transform;
					cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
					Blocks[posX, posY] = cube;
				}
				else
				{
					var cube = CreateBlock(posX, posY);
					cube.transform.parent = parentCube.transform;
					cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
					Blocks[posX, posY] = cube;
				}
			}

		SetBlockType();
		GetIslands(game);
		CreateRocks(game);
		CreateGold(game);
		CreateIron(game);
		CreateCopper(game);
		_enlisted = null;
		MineralTypes = null;
		BlockIslandSize = null;
		Minerals = null;
		CreateTrees(game);
	}

	public void CreateTrees(GameManager game)
	{
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;
		int chance = 0;
		_enlisted = new bool[GameUnity.FullWidth, GameUnity.FullHeight];
		List<Vector2> okTops = new List<Vector2>();
		for (int y = 0; y < fullHeight - 2; y++)
		{
			for (int x = 0; x < fullWidhth; x++)
			{
				var type = BlockTypes[x, y];
				
				var toper = (type == TileType.Top || type == TileType.TopRight || type == TileType.TopLeft) && !_enlisted[x, y];
				if (toper)
				{
					if (CheckAlone(x, y))
						continue;
					
					for (int i = 3; i > 0; i--)
					{
						var otherType = BlockTypes[x - i, y];
						var isTop = (otherType == TileType.Top || otherType == TileType.TopRight || otherType == TileType.TopLeft) && !_enlisted[x - i, y];
						if (isTop)
						{
							chance += 1;
							okTops.Add(new Vector2(x - i, y));
						}
						else
							okTops.Clear();
					}
					okTops.Add(new Vector2(x, y));
					for (int i = 1; i < 4; i++)
					{
						if (okTops.Count == 4)
							break;
						var otherType = BlockTypes[x + i, y];
						var isTop = (otherType == TileType.Top || otherType == TileType.TopRight || otherType == TileType.TopLeft) && !_enlisted[x + i, y];
						if (isTop)
						{
							chance += 1;
							okTops.Add(new Vector2(x + i, y));
						}
						else
							break;	
					}
					int create = game.CurrentRandom.Next(0, minsVariables.TreeOneIn - chance);
					if ( okTops.Count > 1)
					{
						CreateTree(game, okTops);
					}
					okTops.Clear();
					chance = 0;
				}
			}
		}
	}

	void CreateTree(GameManager game, List<Vector2> tops)
	{
		GameObject go = null;
		var leftType = BlockTypes[(int)tops[0].x -1, (int)tops[0].y + 1];
		var rightType = BlockTypes[(int)tops[0].x + 1, (int)tops[0].y + 1];
		int start = 0;
		if (leftType != TileType.Air)
		{
			start = 4 - tops.Count;
		}
		#region Ground
		for (int i = 0; i < tops.Count; i++)
		{
			_enlisted[(int)tops[i].x, (int)tops[i].y] = true;
			
			if (i+start == 0)
			{
				go = GameObject.Instantiate(game.GameResources.Prefabs.Level1_1.gameObject);
			}
			if (i + start == 1)
			{
				go = GameObject.Instantiate(game.GameResources.Prefabs.Level1_2.gameObject);
				var up = BlockTypes[(int)tops[0].x, (int)tops[0].y + 2];
				if (up == TileType.Air)
				{
					var go1 = GameObject.Instantiate(game.GameResources.Prefabs.Level2_1.gameObject);
					go1.transform.position = new Vector3(tops[i].x, tops[i].y + 2, 0) * 1.28f;
					go1.transform.position -= new Vector3(0, 0.49f, 0);
					for (int j = 1; j < 8; j++)
					{
						up = BlockTypes[(int)tops[0].x, (int)tops[0].y + 2 + j +2];
						if (up == TileType.Air)
						{
							int mod = j % 2;
							if(mod == 1)go1 = GameObject.Instantiate(game.GameResources.Prefabs.Level3_1.gameObject);
							if (mod == 0) go1 = GameObject.Instantiate(game.GameResources.Prefabs.Level4_1.gameObject);
							go1.transform.position = new Vector3(tops[i].x, tops[i].y + 2 + j, 0) * 1.28f;
							go1.transform.position -= new Vector3(0, 0.49f, 0);
						}
						else
							break;
					}
				}
			}
			if (i + start == 2)
			{
				go = GameObject.Instantiate(game.GameResources.Prefabs.Level1_3.gameObject);
				var up = BlockTypes[(int)tops[0].x, (int)tops[0].y + 2];
				if (up == TileType.Air)
				{
					var go1 = GameObject.Instantiate(game.GameResources.Prefabs.Level2_2.gameObject);
					go1.transform.position = new Vector3(tops[i].x, tops[i].y + 2, 0) * 1.28f;
					go1.transform.position -= new Vector3(0, 0.49f, 0);
					for (int j = 1; j < 8; j++)
					{
						up = BlockTypes[(int)tops[0].x, (int)tops[0].y + 2 + j +2];
						if (up == TileType.Air)
						{
							int mod = j % 2;
							if (mod == 1) go1 = GameObject.Instantiate(game.GameResources.Prefabs.Level3_2.gameObject);
							if (mod == 0) go1 = GameObject.Instantiate(game.GameResources.Prefabs.Level4_2.gameObject);
							go1.transform.position = new Vector3(tops[i].x, tops[i].y + 2 + j, 0) * 1.28f;
							go1.transform.position -= new Vector3(0, 0.49f, 0);
						}
						else
							break;
					}
				}
			}
			if (i + start == 3)
			{
				go = GameObject.Instantiate(game.GameResources.Prefabs.Level1_4.gameObject);
			}
			go.transform.position = new Vector3(tops[i].x, tops[i].y + 1, 0) * 1.28f;
			go.transform.position -= new Vector3(0, 0.49f, 0);
		} 
		#endregion

	}

	bool CheckAlone(int x, int y)
	{
		var leftType = BlockTypes[x - 1, y];
		var rightType = BlockTypes[x + 1, y];
		var right = (rightType == TileType.Top || rightType == TileType.TopRight) && !_enlisted[x + 1, y];
		var left = (leftType == TileType.Top || leftType == TileType.TopLeft) && !_enlisted[x - 1, y];
		var alone = !right && !left;
		return alone;
	}
	public void CreateRocks(GameManager game)
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
				if (type == TileType.Air)
					continue;

				int chance = game.CurrentRandom.Next(0, minsVariables.RockMiddleOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = IngredientType.Rock;

					if (BlockTypes[x, y] == TileType.Middle)
					{
						CreateMineral(game, x, y, _rockMiddleMat, IngredientType.Rock);
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						CreateMineral(game, x, y, _rockMiddle2Mat, IngredientType.Rock);
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						CreateMineral(game, x, y, _rockMiddle3Mat, IngredientType.Rock);
					}
					if (BlockTypes[x, y] == TileType.MiddleLeft)
					{
						CreateMineral(game, x, y, _rockMiddleLeftMat, IngredientType.Rock);
					}
					if (BlockTypes[x, y] == TileType.MiddleRight)
					{
						CreateMineral(game, x, y, _rockMiddleRightMat, IngredientType.Rock);
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						CreateMineral(game, x, y, _rockTopMat, IngredientType.Rock);
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						CreateMineral(game, x, y, _rockTopLeftMat, IngredientType.Rock);
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						CreateMineral(game, x, y, _rockTopRightMat, IngredientType.Rock);
					}
				}
			}
		}
	}
	public void CreateGold(GameManager game)
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
				if (type == TileType.Air)
					continue;
				int chance = 1;
				int extraChance = 0;
				int smallIsland = BlockIslandSize[x, y];
				if (smallIsland < minsVariables.GoldChanceIslandLimit)
				{
					extraChance = minsVariables.MaxExtraGoldChance;
				}
				if (type == TileType.Bot)
				{
					chance = game.CurrentRandom.Next(0, minsVariables.GoldBotOnIn);
					if (chance == 0)
					{
						CreateMineral(game, x, y, _goldBotMat, IngredientType.Gold);
						MineralTypes[x, y] = IngredientType.Gold;
					}
				}
				if (type == TileType.BotLeftCorner)
				{
					chance = game.CurrentRandom.Next(0, minsVariables.GoldLeftRightBot - extraChance);
					if (chance == 0)
					{
						CreateMineral(game, x, y, _goldBotLeftCornerMat, IngredientType.Gold);
						MineralTypes[x, y] = IngredientType.Gold;
					}
				}
				if (type == TileType.BotRightCorner)
				{
					chance = game.CurrentRandom.Next(0, minsVariables.GoldLeftRightBot - extraChance);
					if (chance == 0)
					{
						CreateMineral(game, x, y, _goldBotRightCornerMat, IngredientType.Gold);
						MineralTypes[x, y] = IngredientType.Gold;
					}
				}
				chance = game.CurrentRandom.Next(0, minsVariables.GoldRandomOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = IngredientType.Gold;
					if (BlockTypes[x, y] == TileType.Middle)
					{
						CreateMineral(game, x, y, _goldMiddleMat, IngredientType.Gold);
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						CreateMineral(game, x, y, _goldMiddle2Mat, IngredientType.Gold);
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						CreateMineral(game, x, y, _goldMiddle3Mat, IngredientType.Gold);
					}
					if (BlockTypes[x, y] == TileType.MiddleLeft)
					{
						CreateMineral(game, x, y, _goldMiddleLeftMat, IngredientType.Gold);
					}
					if (BlockTypes[x, y] == TileType.MiddleRight)
					{
						CreateMineral(game, x, y, _goldMiddleRightMat, IngredientType.Gold);
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						CreateMineral(game, x, y, _goldTopMat, IngredientType.Gold);
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						CreateMineral(game, x, y, _goldTopLeftMat, IngredientType.Gold);
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						CreateMineral(game, x, y, _goldTopRightMat, IngredientType.Gold);
					}
				}
			}
		}
	}

	private void CreateMineral(GameManager game, int x, int y, Sprite sprite, IngredientType type = IngredientType.Normal)
	{
		var go = GameObject.Instantiate(game.GameResources.Prefabs.SpriteDiffuse);
		var parent = Blocks[x, y].transform;
		Blocks[x, y].GetComponent<BlockComponent>().Mod = Mods[(int)type];
		Blocks[x, y].GetComponent<BlockComponent>().IngredientType = type;
		go.transform.parent = parent;
		go.transform.localPosition = new Vector3(0, 0, -0.1f);
		go.GetComponent<SpriteRenderer>().sprite = sprite;

		if (Minerals[x, y] != null)
		{
			GameObject.Destroy(Minerals[x, y].gameObject);
		}
		Minerals[x, y] = go.transform;
	}

	public void CreateIron(GameManager game)
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
				if (type == TileType.Air)
					continue;
				int chance = 1;

				if (MineralTypes[x, y] == IngredientType.Gold)
				{
					_extraIron++;
					continue;
				}

				if (BlockTypes[x, y] == TileType.Middle)
				{
					int extra = level[x, y] * minsVariables.IronlevelChanceIncrease;
					int maxNext = Mathf.Max(0, minsVariables.IronMiddleOnIn - extra);
					chance = game.CurrentRandom.Next(0, maxNext);

					if (chance == 0)
					{
						MineralTypes[x, y] = IngredientType.Iron;
						CreateMineral(game, x, y, _ironMiddleMat, IngredientType.Iron);
					}
					continue;
				}
				
				chance = game.CurrentRandom.Next(0, minsVariables.IronRandomOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = IngredientType.Iron;
					if (BlockTypes[x, y] == TileType.Bot)
					{
						CreateMineral(game, x, y, _ironBotMat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.BotLeftCorner)
					{
						CreateMineral(game, x, y, _ironBotLeftCornerMat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.BotRightCorner)
					{
						CreateMineral(game, x, y, _ironBotRightCornerMat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						CreateMineral(game, x, y, _ironMiddle2Mat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						CreateMineral(game, x, y, _ironMiddle3Mat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.MiddleLeft)
					{
						CreateMineral(game, x, y, _ironMiddleLeftMat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.MiddleRight)
					{
						CreateMineral(game, x, y, _ironMiddleRightMat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						CreateMineral(game, x, y, _ironTopMat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						CreateMineral(game, x, y, _ironTopLeftMat, IngredientType.Iron);
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						CreateMineral(game, x, y, _ironTopRightMat, IngredientType.Iron);
					}
				}
			}
		}
	}
	public void CreateCopper(GameManager game)
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
				if (type == TileType.Air)
					continue;
				int chance = 1;

				if (MineralTypes[x, y] == IngredientType.Gold || MineralTypes[x, y] == IngredientType.Iron)
				{
					_extraCopper++;
					continue;
				}

				if (BlockTypes[x, y] == TileType.MiddleRight)
				{
					chance = game.CurrentRandom.Next(0, minsVariables.CopperSideOneIn);

					if (chance == 0)
					{
						CreateMineral(game, x, y, _copperMiddleRightMat, IngredientType.Copper);
						MineralTypes[x, y] = IngredientType.Copper;
					}
					continue;
				}

				if (BlockTypes[x, y] == TileType.MiddleLeft)
				{
					chance = game.CurrentRandom.Next(0, minsVariables.CopperSideOneIn);

					if (chance == 0)
					{
						CreateMineral(game, x, y, _copperMiddleLeftMat, IngredientType.Copper);
						MineralTypes[x, y] = IngredientType.Copper;
					}
					continue;
				}

				chance = game.CurrentRandom.Next(0, minsVariables.CopperRandomOneIn);
				if (chance == 0)
				{
					MineralTypes[x, y] = IngredientType.Copper;
					if (BlockTypes[x, y] == TileType.Middle)
					{
						CreateMineral(game, x, y, _copperMiddleMat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.Middle2)
					{
						CreateMineral(game, x, y, _copperMiddle2Mat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.Middle3)
					{
						CreateMineral(game, x, y, _copperMiddle3Mat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.Bot)
					{
						CreateMineral(game, x, y, _copperBotMat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.BotLeftCorner)
					{
						CreateMineral(game, x, y, _copperBotLeftCornerMat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.BotRightCorner)
					{
						CreateMineral(game, x, y, _copperBotRightCornerMat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.Top)
					{
						CreateMineral(game, x, y, _copperTopMat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.TopLeft)
					{
						CreateMineral(game, x, y, _copperTopLeftMat, IngredientType.Copper);
					}
					if (BlockTypes[x, y] == TileType.TopRight)
					{
						CreateMineral(game, x, y, _copperTopRightMat, IngredientType.Copper);
					}
				}
			}
		}
	}

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
	private void GetIslands(GameManager game)
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
					//if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.TopRight;
				}
				if (!top && !left)
				{
					newMat = Resources.Load("Tiles/TopLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "TopLeft";
					//if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.TopLeft;
				}
				if (!top && right && left)
				{
					newMat = Resources.Load("Tiles/Top", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Top";
					//if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Top;
				}
				if (top && right && left && bot)
				{
					newMat = Resources.Load("Tiles/Middle", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Middle";
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Middle;
				}
				if (top && !right && bot)
				{
					newMat = Resources.Load("Tiles/MiddleRight", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "MiddleRight";
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.MiddleRight;
				}
				if (top && !left && bot)
				{
					newMat = Resources.Load("Tiles/MiddleLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "MiddleLeft";
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.MiddleLeft;
				}
				if (top && !right && !bot)
				{
					newMat = Resources.Load("Tiles/BotRightCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotRightCorner";
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.BotRightCorner;
				}
				if (top && !left && !bot)
				{
					newMat = Resources.Load("Tiles/BotLeftCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotLeftCorner";
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.BotLeftCorner;
				}
				if (top && left && !bot && right)
				{
					newMat = Resources.Load("Tiles/Bot", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Bot";
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Bot;
				}

				Blocks[x, y].GetComponent<SpriteRenderer>().sprite = newMat;
				Blocks[x, y].GetComponent<BlockComponent>().TileType = BlockTypes[x, y];
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
				Blocks[x, y].GetComponent<BlockComponent>().TileType = BlockTypes[x, y];
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
		
		_rockBotMat = Resources.Load("Tiles/Minerals/Rocks/Bot", typeof(Sprite)) as Sprite;
		_rockBotLeftCornerMat = Resources.Load("Tiles/Minerals/Rocks/BotLeftCorner", typeof(Sprite)) as Sprite;
		_rockBotRightCornerMat = Resources.Load("Tiles/Minerals/Rocks/BotRightCorner", typeof(Sprite)) as Sprite;
		_rockMiddleMat = Resources.Load("Tiles/Minerals/Rocks/Middle", typeof(Sprite)) as Sprite;
		_rockMiddle2Mat = Resources.Load("Tiles/Minerals/Rocks/Middle2", typeof(Sprite)) as Sprite;
		_rockMiddle3Mat = Resources.Load("Tiles/Minerals/Rocks/Middle3", typeof(Sprite)) as Sprite;
		_rockMiddleLeftMat = Resources.Load("Tiles/Minerals/Rocks/MiddleLeft", typeof(Sprite)) as Sprite;
		_rockMiddleRightMat = Resources.Load("Tiles/Minerals/Rocks/MiddleRight", typeof(Sprite)) as Sprite;
		_rockTopMat = Resources.Load("Tiles/Minerals/Rocks/Top", typeof(Sprite)) as Sprite;
		_rockTopLeftMat = Resources.Load("Tiles/Minerals/Rocks/TopLeft", typeof(Sprite)) as Sprite;
		_rockTopRightMat = Resources.Load("Tiles/Minerals/Rocks/TopRight", typeof(Sprite)) as Sprite;

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
}