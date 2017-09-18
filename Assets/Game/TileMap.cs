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
	private List<Vector2> _grassLevels = new List<Vector2>();
	private List<Vector2> _boundries = new List<Vector2>();
	private List<Vector2> _bottoms = new List<Vector2>();
	private List<Vector2> _walls = new List<Vector2>();
	private List<List<Vector2>> _islands = new List<List<Vector2>>();
	private Material diffMat;
	private TileType[,] BlockTypes;
	private MineralType[,] MineralTypes;
	// Side blocks, kolla längden och ta mitten
	public GameObject[,] Blocks;

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

	private bool[,] _enlisted;
	public void InitiateMap()
	{
		SetMaterials();
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;
		Blocks = new GameObject[fullWidhth, fullHeight];
		BlockTypes = new TileType[fullWidhth, fullHeight];
		MineralTypes = new MineralType[fullWidhth, fullHeight];
		_enlisted = new bool[GameUnity.FullWidth, GameUnity.FullHeight];

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
					_boundries.Add(new Vector2(x, y));
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
				if (noise < 0.16f)
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
					_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.TopRight;
				}
				if (!top && !left)
				{
					newMat = Resources.Load("Tiles/TopLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "TopLeft";
					_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.TopLeft;
				}
				if (!top && right && left)
				{
					newMat = Resources.Load("Tiles/Top", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Top";
					_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Top;
				}
				if (top && right && left && bot)
				{
					newMat = Resources.Load("Tiles/Middle", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Middle";
					_walls.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Middle;
				}
				if (top && !right && bot)
				{
					newMat = Resources.Load("Tiles/MiddleRight", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "MiddleRight";
					_walls.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.MiddleRight;
				}
				if (top && !left && bot)
				{
					newMat = Resources.Load("Tiles/MiddleLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "MiddleLeft";
					_walls.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.MiddleLeft;
				}
				if (top && !right && !bot)
				{
					newMat = Resources.Load("Tiles/BotRightCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotRightCorner";
					_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.BotRightCorner;
				}
				if (top && !left && !bot)
				{
					newMat = Resources.Load("Tiles/BotLeftCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotLeftCorner";
					_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.BotLeftCorner;
				}
				if (top && left && !bot && right)
				{
					newMat = Resources.Load("Tiles/Bot", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Bot";
					_bottoms.Add(new Vector2(x, y));
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

	public void GenerateMinerals()
	{
		CreateRocks();
		GetIslands();
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
				int chance = Random.Range(0, GameUnity.RockMiddleOneIn);
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
				if (enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)left.x, (int)left.y] = true;
					currentIsland.Add(left);
				}

				blockType = BlockTypes[(int)down.x, (int)down.y];
				if (enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)down.x, (int)down.y] = true;
					currentIsland.Add(down);

				}

				blockType = BlockTypes[(int)right.x, (int)right.y];
				if (enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)right.x, (int)right.y] = true;
					currentIsland.Add(right);

				}

				blockType = BlockTypes[(int)up.x, (int)up.y];
				if (enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					_enlisted[(int)up.x, (int)up.y] = true;
					currentIsland.Add(up);

				}
				currentIndex++;
			}
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

	private void SetMaterials()
	{
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
	}
}