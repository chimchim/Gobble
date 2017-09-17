using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileMap
{
	public enum TileType
	{
		Air,
		Wall,
		Grass,
		Bottom,
		Middle,
		Enlisted,
		Boundry
	}
	private List<Vector2> _grassLevels = new List<Vector2>();
	private List<Vector2> _boundries = new List<Vector2>();
	private List<Vector2> _bottoms = new List<Vector2>();
	private List<Vector2> _walls = new List<Vector2>();
	private List<List<Vector2>> _islands = new List<List<Vector2>>();
	private Material diffMat;
	private TileType[,] BlockTypes;

	// Side blocks, kolla längden och ta mitten
	public GameObject[,] Blocks;

	public void InitiateMap()
	{
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;
		topWaterSprite = Resources.Load("Tiles/TopWater", typeof(Sprite)) as Sprite;
		waterSprite = Resources.Load("Tiles/Middlewater", typeof(Sprite)) as Sprite;
		diffMat = Resources.Load("Material/SpriteDiffuse", typeof(Material)) as Material;
		Blocks = new GameObject[fullWidhth, fullHeight];
		BlockTypes = new TileType[fullWidhth, fullHeight];

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
				if (noise < 0.3f)
				{
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

				if (!top && !right)
				{
					newMat = Resources.Load("Tiles/TopRight", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "TopRight";
					_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Grass;
				}
				if (!top && !left)
				{
					newMat = Resources.Load("Tiles/TopLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "TopLeft";
					_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Grass;
				}
				if (!top && right && left)
				{
					newMat = Resources.Load("Tiles/Top", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Top";
					_grassLevels.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Grass;
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
						BlockTypes[x, y] = TileType.Wall;
				}
				if (top && !left && bot)
				{
					newMat = Resources.Load("Tiles/MiddleLeft", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "MiddleLeft";
					_walls.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Wall;
				}
				if (top && !right && !bot)
				{
					newMat = Resources.Load("Tiles/BotRightCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotRightCorner";
					_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Bottom;
				}
				if (top && !left && !bot)
				{
					newMat = Resources.Load("Tiles/BotLeftCorner", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "BotLeftCorner";
					_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Bottom;
				}
				if (top && left && !bot && right)
				{
					newMat = Resources.Load("Tiles/Bot", typeof(Sprite)) as Sprite;
					Blocks[x, y].name = "Bot";
					_bottoms.Add(new Vector2(x, y));
					if (BlockTypes[x, y] != TileType.Boundry)
						BlockTypes[x, y] = TileType.Bottom;
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
					}
					if (!middleLeft)
					{
						newMat = Resources.Load("Tiles/Middle2", typeof(Sprite)) as Sprite;
						Blocks[x, y].GetComponent<SpriteRenderer>().sprite = newMat;
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

				if (blockType != TileType.Enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					BlockTypes[(int)left.x, (int)left.y] = TileType.Enlisted;
					currentIsland.Add(left);
				}

				blockType = BlockTypes[(int)down.x, (int)down.y];
				if (blockType != TileType.Enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					BlockTypes[(int)down.x, (int)down.y] = TileType.Enlisted;
					currentIsland.Add(down);

				}

				blockType = BlockTypes[(int)right.x, (int)right.y];
				if (blockType != TileType.Enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					BlockTypes[(int)right.x, (int)right.y] = TileType.Enlisted;
					currentIsland.Add(right);

				}

				blockType = BlockTypes[(int)up.x, (int)up.y];
				if (blockType != TileType.Enlisted && blockType != TileType.Air && blockType != TileType.Boundry)
				{

					BlockTypes[(int)up.x, (int)up.y] = TileType.Enlisted;
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
				if (BlockTypes[x, y] == TileType.Air || BlockTypes[x, y] == TileType.Enlisted)
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

}