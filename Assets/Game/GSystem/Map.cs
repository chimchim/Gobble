using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
	public class Map : ISystem
	{

		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
		private Dictionary<Vector2, GameObject> Tiles = new Dictionary<Vector2, GameObject>();

		int mapheightEdges = 55;
		int mapwidthEdges = 110;
		int mapheight = 50;
		int mapwidth = 100;
		public void Update(GameManager game)
		{
		}

		public void Initiate(GameManager game)
		{
			

			for (int x = (mapwidth - mapwidthEdges); x < mapwidthEdges; x++)
			{
				for (int y = (mapheight - mapheightEdges); y < mapheightEdges; y++)
				{
					if ((x < 0 || x > mapwidth+1) || (y < 0 || y > mapheight+1))
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
						Tiles.Add(new Vector2(x, y), cube);
					}

				}
			}

			Vector2 shift = new Vector2(0, 0); // play with this to shift map around
			float zoom = 0.1f; // play with this to zoom into the noise field

			for (int x = 0; x < mapwidth; x++)
				for (int y = 0; y < mapheight; y++)
				{
					Vector2 pos = zoom * (new Vector2(x, y)) + shift;
					float noise = Mathf.PerlinNoise(pos.x, pos.y);
					
					if (noise < 0.3f)
					{

					}
					else if (noise < 0.5f)
					{

					}
					else if (noise < 0.9f && noise > 0.5f)
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
						Tiles.Add(new Vector2(x, y), cube);
					}
					else
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						Tiles.Add(new Vector2(x, y), cube);
					}

					

			}

			Sprite newMat = null;
			foreach (Vector2 pos in Tiles.Keys)
			{
				var top = Tiles.ContainsKey(new Vector2(0, 1)+ pos);
				var bot = Tiles.ContainsKey(new Vector2(0, -1) + pos);
				var right = Tiles.ContainsKey(new Vector2(1, 0) + pos);
				var left = Tiles.ContainsKey(new Vector2(-1, 0) + pos);

				if (!top && !right)
				{
					newMat = Resources.Load("Tiles/TopRight", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "TopRight";
				}
				if (!top && !left)
				{
					newMat = Resources.Load("Tiles/TopLeft", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "TopLeft";
				}
				if (!top && right && left)
				{
					newMat = Resources.Load("Tiles/Top", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "Top";
				}
				if (top && right && left && bot)
				{
					newMat = Resources.Load("Tiles/Middle", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "Middle";
				}
				if (top && !right && bot)
				{
					newMat = Resources.Load("Tiles/MiddleRight", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "MiddleRight";
				}
				if (top && !left && bot)
				{
					newMat = Resources.Load("Tiles/MiddleLeft", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "MiddleLeft";
				}
				if (top && !right && !bot)
				{
					newMat = Resources.Load("Tiles/BotRightCorner", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "BotRightCorner";
				}
				if (top && !left && !bot)
				{
					newMat = Resources.Load("Tiles/BotLeftCorner", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "BotLeftCorner";
				}
				if (top && left && !bot && right)
				{
					newMat = Resources.Load("Tiles/Bot", typeof(Sprite)) as Sprite;
					Tiles[pos].name = "Bot";
				}

				Tiles[pos].GetComponent<SpriteRenderer>().sprite = newMat;
			}

			foreach (Vector2 pos in Tiles.Keys)
			{
				var top = Tiles.ContainsKey(new Vector2(0, 1) + pos);
				var bot = Tiles.ContainsKey(new Vector2(0, -1) + pos);
				var right = Tiles.ContainsKey(new Vector2(1, 0) + pos);
				var left = Tiles.ContainsKey(new Vector2(-1, 0) + pos);

				if (Tiles[pos].name == "Middle")
				{
					var middleRight = Tiles.ContainsKey(new Vector2(1, 1) + pos);
					var middleLeft = Tiles.ContainsKey(new Vector2(-1, 1) + pos);
					if (!middleRight)
					{
						newMat = Resources.Load("Tiles/Middle3", typeof(Sprite)) as Sprite;
						Tiles[pos].GetComponent<SpriteRenderer>().sprite = newMat;
					}
					if (!middleLeft)
					{
						newMat = Resources.Load("Tiles/Middle2", typeof(Sprite)) as Sprite;
						Tiles[pos].GetComponent<SpriteRenderer>().sprite = newMat;
					}
					
				}
			}

			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var go = game.Entities.GetEntity(entity);
				go.gameObject.transform.position = GameUnity.StartingPosition;
			}
			CreateWater(3);
		}

		private void CreateWater(int height)
		{
			//int mapheight = 50;
			//int mapwidth = 102;
			List<List<Vector2>> waters = new List<List<Vector2>>();
			bool foundWater = false;
			int waterCount = -1;
			for (int y = 0; y < 1; y++)
			{
				for (int x = 0; x < mapwidth + 2; x++)
				{
					var pos = new Vector2(x, y);
					if (!Tiles.ContainsKey(pos))
					{
						if (foundWater == false)
						{
							waterCount++;
                            waters.Add(new List<Vector2>());
							foundWater = true;
						}
						waters[waterCount].Add(pos);
                    }
					else
					{
						foundWater = false;
					}
				}
			}
			
			for (int i = 0; i < waters.Count; i++)
			{
				List<Vector2> newLevels = new List<Vector2>();
				int count = waters[i].Count;
				if (!AddWaterHorizontal(waters[i], newLevels))
				{
					continue;
				}

				Vector2 fp = waters[i][0];
				Vector2 lp = waters[i][waters[i].Count - 1];

				for (int j = 0; j < count; j++)
				{
					var pos = waters[i][j];
					var highpos = new Vector2(pos.x, pos.y + 1);
					if (!Tiles.ContainsKey(highpos))
					{
						waters[i].Add(highpos);
						newLevels.Add(highpos);
                    }
				}

				List<Vector2> newLevels2 = new List<Vector2>();
				int levelamount = 1;
                for (int j = 0; j < 8; j++)
				{
					if (newLevels.Count == 0)
					{
						break;
					}
					levelamount++;
                    int count2 = newLevels.Count;

					if (!AddWaterHorizontal(newLevels, newLevels2))
					{
						continue;
					}
					for (int k = 0; k < count2; k++)
					{
						var pos = newLevels[k];
						var highpos = new Vector2(pos.x, pos.y + 1);
						if (!Tiles.ContainsKey(highpos))
						{
							newLevels2.Add(highpos);
						}
					}
					waters[i].AddRange(newLevels2);
					newLevels.Clear();
					newLevels.AddRange(newLevels2);
					newLevels2.Clear();
				}
			}
			for (int j = 0; j < levelamount; j++)
			{

			}
			Sprite newMat = null;
			for (int i = 0; i < waters.Count; i++)
			{
				for (int j = 0; j < waters[i].Count; j++)
				{
					var pos = waters[i][j];
					if (!Tiles.ContainsKey(pos))
                    {
						float x = pos.x;
						float y = pos.y;
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						Tiles.Add(new Vector2(x, y), cube);
						newMat = Resources.Load("Tiles/MiddleWater", typeof(Sprite)) as Sprite;
						cube.GetComponent<SpriteRenderer>().sprite = newMat;
						cube.name = "TopWater";
					}
				}
			}
		}

		private bool AddWaterHorizontal(List<Vector2> waters, List<Vector2> newLevel)
		{

			var firstPos = waters[0] + new Vector2(0, 1);
			var lastPos = waters[waters.Count - 1] + new Vector2(0, 1);
			bool leftCool = false;
			bool rightCool = false;
			var newWaters = new List<Vector2>();
			var nextpos = new Vector2(-1, 0) + firstPos;
			while (true)
			{
				
				if (!Tiles.ContainsKey(nextpos))
				{
					var leftDown = new Vector2(0, -1) + nextpos;
					if (Tiles.ContainsKey(leftDown))
					{
						newWaters.Add(nextpos);
						newLevel.Add(nextpos);
						nextpos += new Vector2(-1, 0);  
					}
					else
					{
						Debug.Log("left failed");
						newLevel.Clear();
                        newWaters.Clear();
						break;
					}
				}
				else
				{
					leftCool = true;
					break;
				}

			}
			var nextpos2 = new Vector2(1, 0) + lastPos;
			while (true)
			{
				if (!Tiles.ContainsKey(nextpos2))
				{
					var rightDown = new Vector2(0, -1) + nextpos2;
					if (Tiles.ContainsKey(rightDown))
					{
						newWaters.Add(nextpos2);
						newLevel.Add(nextpos2);
						nextpos2 += new Vector2(1, 0);
						
					}
					else
					{
						Debug.Log("right failed");
						newWaters.Clear();
						newLevel.Clear();
                        break;
					}
				}
				else
				{
					rightCool = true;
					break;
				}
			}
			if (rightCool && leftCool)
			{
				waters.AddRange(newWaters);
				return true;
            }
			return false;
		}

		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}