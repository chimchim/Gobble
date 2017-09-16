using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
	public class Map : ISystem
	{
		public enum TileType
		{
			Air,
			Wall,
			Grass,
			Bottom,
			Middle,
			Middle1,
			Middle2,
			Middle3,
			Middle4,
			Boundry
		}
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
		private List<Vector2> _grassLevels = new List<Vector2>();
		private List<Vector2> _boundries = new List<Vector2>();
		private List<Vector2> _bottoms = new List<Vector2>();
		private List<Vector2> _walls = new List<Vector2>();
		private List<List<Vector2>> _islands = new List<List<Vector2>>();

		private TileType[,] BlockTypes;

		// Side blocks, kolla längden och ta mitten
		private GameObject[,] Blocks;
		bool[,] foundTile;

		GameObject Water;
		private Material[] Waters = new Material[10];
		int AIR = 0;
		int GROUND = 1;
		int WATER = 2;

		//r properties
		float MaxMass = 1.0f;
		float MaxCompress = 0.02f;
		float MinMass = 0.0001f;

		float MinDraw = 0.01f;
		float MaxDraw = 1.1f;

		float MaxSpeed = 1;

		float MinFlow = 0.01f;

		int[,] blocks;
		Transform[,] waters;

		float[,] mass;
		float[,] new_mass;

		Sprite topWaterSprite;
		Sprite waterSprite;
		Material diffMat;

		public void Update(GameManager game)
		{
			if (GameUnity.CreateWater)
			{
				UpdateWater();
			}
			UpdateMiniMap(game);
		}

		public void Initiate(GameManager game)
		{
			InitiateMap(game);
			InitiateWater();
			GetIslands();
			CheckIslandLevels();
		}

		private void CheckIslandLevels()
		{
			for (int i = 0; i < _islands.Count; i++)
			{
				var island = _islands[i];
				var islandLevels = GetIslandLevel(island);
				Debug.Log("island nr " + i);
				for (int j = 0; j < islandLevels.Count; j++)
				{
					var level = islandLevels[j];
					Debug.Log("Level nr " + j);
					for (int k = 0; k < level.Count; k++)
					{
						Debug.Log("island nr " + level[k] * 1.28f);
					}
				}
			}
			//Debug.Log("fad " +_islands[1][3] * 1.28f);
		}
		// kolla se på inte sitter ihop med boundry
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
			while (currentIsland.Count > currentIndex)
			{
				if (currentIsland.Count > currentIndex)
				{
					current = currentIsland[currentIndex];
					var left = new Vector2(current.x - 1, current.y);
					var down = new Vector2(current.x, current.y - 1);
					var right = new Vector2(current.x + 1, current.y);
					var up = new Vector2(current.x, current.y + 1);
					var leftVector = new Vector2((int)left.x, (int)left.y);
					GameObject block = Blocks[(int)left.x, (int)left.y];
					if (!currentIsland.Contains(left) && block != null)
					{
						if (_boundries.Contains(leftVector))
						{
							currentIsland.Clear();
							return null;
						}


						currentIsland.Add(left);
					}

					block = Blocks[(int)down.x, (int)down.y];
					var downVector = new Vector2((int)down.x, (int)down.y);
					if (!currentIsland.Contains(down) && block != null)
					{
						if (_boundries.Contains(downVector))
						{
							currentIsland.Clear();
							return null;
						}
						currentIsland.Add(down);

					}

					block = Blocks[(int)right.x, (int)right.y];
					var rightVector = new Vector2((int)right.x, (int)right.y);
					if (!currentIsland.Contains(right) && block != null)
					{
						if (_boundries.Contains(rightVector))
						{
							currentIsland.Clear();
							return null;
						}
						currentIsland.Add(right);

					}

					block = Blocks[(int)up.x, (int)up.y];
					var upVector = new Vector2((int)up.x, (int)up.y);
					if (!currentIsland.Contains(up) && block != null)
					{
						if (_boundries.Contains(upVector))
						{
							currentIsland.Clear();
							return null;
						}
						currentIsland.Add(up);

					}
					currentIndex++;
				}
			}

			return currentIsland;
		}

		private void GetIslands()
		{
			int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
			int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);
			Vector2 firstPos = new Vector2(GameUnity.WidhtBound + (GameUnity.WidhtBound * 0.28f), GameUnity.HeightBound + (GameUnity.HeightBound * 0.28f));
			List<Vector2> enlisted = new List<Vector2>();
			int islandIndex = 0;
			for (int x = GameUnity.WidhtBound; x < GameUnity.MapWidth; x++)
			{
				for (int y = GameUnity.HeightBound; y < GameUnity.MapHeight; y++)
				{
					if (Blocks[x, y] != null && !enlisted.Contains(new Vector2(x, y)))
					{
						var block = Blocks[x, y];
						var newList = MakeIsland(x, y);
						if (newList != null)
						{

							_islands.Add(new List<Vector2>());// = new List<GameObject>();
							_islands[islandIndex] = newList;
							enlisted.AddRange(newList);
							islandIndex++;
						}
					}
				}
			}
		}
		//private void Get

		private void UpdateMiniMap(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
			int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);

			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				if (player.Owner)
				{
					var entTransform = game.Entities.GetEntity(e).gameObject.transform;
					int startX = (int)(entTransform.position.x / 1.28f);
					int startY = (int)(entTransform.position.y / 1.28f);
					for (int x = -GameUnity.MiniMapBoundryX; x < GameUnity.MiniMapBoundryX+3; x++)
					{
						for (int y = -GameUnity.MiniMapBoundryY; y < GameUnity.MiniMapBoundryY; y++)
						{
							int currentX = startX + (x);
							int currentY = startY + (y);

							currentX = Mathf.Clamp(currentX, 0, fullWidhth - 1);
							currentY = Mathf.Clamp(currentY, 0, fullHeight - 1);
							if (!foundTile[currentX, currentY])
							{
								foundTile[currentX, currentY] = true;
								if (Blocks[currentX, currentY] != null)
								{
									var transform = Blocks[currentX, currentY].transform;
									transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
								}
								if (GameUnity.CreateWater && waters[currentX, currentY] != null)
								{
									var transform = waters[currentX, currentY].transform;
									transform.position = new Vector3(transform.position.x, transform.position.y, -0.25f);
								}
							}
						}
					}
				}
			}
		}

		public void InitiateMap(GameManager game)
		{
			int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
			int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);
			topWaterSprite = Resources.Load("Tiles/TopWater", typeof(Sprite)) as Sprite;
			waterSprite = Resources.Load("Tiles/Middlewater", typeof(Sprite)) as Sprite;
			diffMat = Resources.Load("Material/SpriteDiffuse", typeof(Material)) as Material;
			Blocks = new GameObject[fullWidhth, fullWidhth];
			foundTile = new bool[fullWidhth, fullWidhth];
			BlockTypes = new TileType[fullWidhth, fullWidhth];
			GameObject parentCube = new GameObject();
			parentCube.name = "Tiles";
			for (int x = 0; x < fullWidhth; x++)
			{
				for (int y = 0; y < fullHeight; y++)
				{
					if ((x < GameUnity.WidhtBound || x > (GameUnity.MapWidth + GameUnity.WidhtBound)) || (y < GameUnity.HeightBound || y > (GameUnity.MapHeight + GameUnity.HeightBound)))
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
					}

				}
			}

			Vector2 shift = new Vector2(0, 0); // play with this to shift map around
			float zoom = 0.1f; // play with this to zoom into the noise field

			for (int x = 0; x < GameUnity.MapWidth; x++)
				for (int y = 1; y < GameUnity.MapHeight; y++)
				{
					Vector2 pos = zoom * (new Vector2(x, y)) + shift;
					float noise = Mathf.PerlinNoise(pos.x, pos.y);

					int posX = x + GameUnity.WidhtBound;
					int posY = y + GameUnity.HeightBound;
					if (noise < 0.3f)
					{
						BlockTypes[x, y] = TileType.Air;
					}
					else if (noise < 0.5f)
					{
						BlockTypes[x, y] = TileType.Air;
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

			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var go = game.Entities.GetEntity(entity);
				go.gameObject.transform.position = GameUnity.StartingPosition;
			}
		}


		private void SetBlockType()
		{
			Sprite newMat = null;
			int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
			int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);
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
						BlockTypes[x, y] = TileType.Grass;
					}
					if (!top && !left)
					{
						newMat = Resources.Load("Tiles/TopLeft", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "TopLeft";
						_grassLevels.Add(new Vector2(x, y));
						BlockTypes[x, y] = TileType.Grass;
					}
					if (!top && right && left)
					{
						newMat = Resources.Load("Tiles/Top", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "Top";
						_grassLevels.Add(new Vector2(x, y));
						BlockTypes[x, y] = TileType.Grass;
					}
					if (top && right && left && bot)
					{
						newMat = Resources.Load("Tiles/Middle", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "Middle";
						_walls.Add(new Vector2(x, y));
						BlockTypes[x, y] = TileType.Middle;
					}
					if (top && !right && bot)
					{
						newMat = Resources.Load("Tiles/MiddleRight", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "MiddleRight";
						_walls.Add(new Vector2(x, y));
						BlockTypes[x, y] = TileType.Wall;
					}
					if (top && !left && bot)
					{
						newMat = Resources.Load("Tiles/MiddleLeft", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "MiddleLeft";
						_walls.Add(new Vector2(x, y));
						BlockTypes[x, y] = TileType.Wall;
					}
					if (top && !right && !bot)
					{
						newMat = Resources.Load("Tiles/BotRightCorner", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "BotRightCorner";
						_bottoms.Add(new Vector2(x, y));
						BlockTypes[x, y] = TileType.Bottom;
					}
					if (top && !left && !bot)
					{
						newMat = Resources.Load("Tiles/BotLeftCorner", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "BotLeftCorner";
						_bottoms.Add(new Vector2(x, y));
						BlockTypes[x, y] = TileType.Bottom;
					}
					if (top && left && !bot && right)
					{
						newMat = Resources.Load("Tiles/Bot", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "Bot";
						_bottoms.Add(new Vector2(x, y));
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
		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
		void InitiateWater()
		{
			if (!GameUnity.CreateWater)
			{
				return;
			}
			
			Water = GameObject.Instantiate(Resources.Load("Prefabs/Water", typeof(GameObject))) as GameObject;
			Water.transform.position = new Vector3(-1111, -1111, 0);

			int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
			int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);

			blocks = new int[fullWidhth + 2, fullHeight + 2];
			mass = new float[fullWidhth + 2, fullHeight + 2];
			new_mass = new float[fullWidhth + 2, fullHeight + 2];
			waters = new Transform[fullWidhth + 2, fullHeight + 2];
			for (int x = 0; x < fullWidhth + 2; x++)
			{
				for (int y = 0; y < fullHeight + 2; y++)
				{
					blocks[x, y] = Random.Range(0, GameUnity.WaterAmountOneIn);
					if (blocks[x, y] == GROUND || blocks[x, y] > WATER)
					{
						blocks[x, y] = 0;
					}
					if ((y < fullHeight) && (x < fullWidhth) && Blocks[x, y] != null)
					{
						blocks[x, y] = 1;
					}
					mass[x, y] = blocks[x, y] == WATER ? MaxMass : 0.0f;
					new_mass[x, y] = blocks[x, y] == WATER ? MaxMass : 0.0f;
				}
			}

			for (int x = 0; x < fullWidhth + 2; x++)
			{
				blocks[x, 0] = AIR;
				blocks[x, fullHeight + 1] = AIR;
			}

			for (int y = 1; y < fullHeight + 1; y++)
			{
				blocks[0, y] = AIR;
				blocks[fullWidhth + 1, y] = AIR;
			}

			for (int i = 0; i < GameUnity.WaterSimulations; i++)
			{
				SimulateCompression();
			}

		}
		float GetStableState(float total_mass)
		{
			if (total_mass <= 1)
			{
				return 1;
			}
			else if (total_mass < 2 * MaxMass + MaxCompress)
			{
				return (MaxMass * MaxMass + total_mass * MaxCompress) / (MaxMass + MaxCompress);
			}
			else
			{
				return (total_mass + MaxCompress) / 2;
			}
		}
		void SimulateCompression()
		{
			float Flow = 0;
			float remaining_mass;
			int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
			int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);
			//Calculate and apply flow for each block
			for (int x = 1; x <= fullWidhth; x++)
			{
				for (int y = 1; y <= fullHeight; y++)
				{
					//Skip inert ground blocks
					if (blocks[x, y] == GROUND) continue;

					//Custom push-only flow
					Flow = 0;
					remaining_mass = mass[x, y];
					if (remaining_mass <= 0) continue;

					//The block below this one
					if ((blocks[x, y - 1] != GROUND))
					{
						Flow = GetStableState(remaining_mass + mass[x, y - 1]) - mass[x, y - 1];
						if (Flow > MinFlow)
						{
							Flow *= 0.5f; //leads to smoother flow
						}
						Flow = Mathf.Clamp(Flow, 0, Mathf.Min(MaxSpeed, remaining_mass));

						new_mass[x, y] -= Flow;
						new_mass[x, y - 1] += Flow;
						remaining_mass -= Flow;
					}

					if (remaining_mass <= 0) continue;

					//Left
					if (blocks[x - 1, y] != GROUND)
					{
						//Equalize the amount of water in this block and it's neighbour
						Flow = (mass[x, y] - mass[x - 1, y]) / 4;
						if (Flow > MinFlow) { Flow *= 0.5f; }
						Flow = Mathf.Clamp(Flow, 0, remaining_mass);

						new_mass[x, y] -= Flow;
						new_mass[x - 1, y] += Flow;
						remaining_mass -= Flow;
					}

					if (remaining_mass <= 0) continue;

					//Right
					if (blocks[x + 1, y] != GROUND)
					{
						//Equalize the amount of water in this block and it's neighbour
						Flow = (mass[x, y] - mass[x + 1, y]) / 4;
						if (Flow > MinFlow) { Flow *= 0.5f; }
						Flow = Mathf.Clamp(Flow, 0, remaining_mass);

						new_mass[x, y] -= Flow;
						new_mass[x + 1, y] += Flow;
						remaining_mass -= Flow;
					}

					if (remaining_mass <= 0) continue;

					//Up. Only compressed water flows upwards.
					if (blocks[x, y + 1] != GROUND)
					{
						Flow = remaining_mass - GetStableState(remaining_mass + mass[x, y + 1]);
						if (Flow > MinFlow) { Flow *= 0.5f; }
						Flow = Mathf.Clamp(Flow, 0, Mathf.Min(MaxSpeed, remaining_mass));

						new_mass[x, y] -= Flow;
						new_mass[x, y + 1] += Flow;
						remaining_mass -= Flow;
					}


				}
			}

			//Copy the new mass values to the mass array
			for (int x = 0; x < fullWidhth + 2; x++)
			{
				for (int y = 0; y < fullHeight + 2; y++)
				{
					mass[x, y] = new_mass[x, y];
				}
			}

			for (int x = 1; x <= fullWidhth; x++)
			{
				for (int y = 1; y <= fullHeight; y++)
				{
					//Skip ground blocks
					if (blocks[x, y] == GROUND) continue;
					//Flag/unflag water blocks
					if (mass[x, y] > MinMass)
					{
						blocks[x, y] = WATER;
					}
					else
					{
						blocks[x, y] = AIR;
					}
				}
			}

			//Remove any water that has left the map
			for (int x = 0; x < fullWidhth + 2; x++)
			{
				mass[x, 0] = 0;
				mass[x, fullHeight + 1] = 0;
			}
			for (int y = 1; y < fullHeight + 1; y++)
			{
				mass[0, y] = 0;
				mass[fullWidhth + 1, y] = 0;
			}

		}
		public void UpdateWater()
		{
			int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
			int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);

			for (int i = 0; i < GameUnity.WaterSimulationsPerUpdate; i++)
			{
				SimulateCompression();
			}

			for (int x = 1; x < fullWidhth + 2; x++)
			{
				for (int y = 1; y < fullHeight + 2; y++)
				{
					if (blocks[x, y] == WATER)
					{
						if (mass[x, y] < MinDraw)
						{
							if (waters[x, y] != null)
							{
								GameObject.Destroy(waters[x, y].gameObject);
							}
						}
						else
						{
							if (mass[x, y] < MaxMass)
							{
								//Draw a half-full block. Block size is dependent on the amount of water in it.
								float scaledSize = mass[x, y];
								if (mass[x, y + 1] >= MinDraw)
								{
									scaledSize = 1;
								}
								DrawBlock(x, y, mass[x, y], scaledSize);
							}
							else
							{

								DrawBlock(x, y, mass[x, y], 1);
							}
						}
					}
					else
					{
						if (waters[x, y] != null)
						{
							GameObject.Destroy(waters[x, y].gameObject);
						}
					}
				}
			}
		}
		private void DrawBlock(int x, int y, float color, float waterMass)
		{
			GameObject go;
			if (waters[x, y] == null)
			{
				go = GameObject.Instantiate(Water);
				waters[x, y] = go.transform;
			}

			float scaledcolor = Mathf.Min(1, color);
			float scaledMass = Mathf.Min(1, waterMass);
			float yOffset = (1 - (scaledMass)) * 1.28f;
			scaledMass = (scaledMass) * 1.28f;
			int watertransparency = ((int)(scaledcolor * 10));
			watertransparency = Mathf.Min(watertransparency, 9);
			float topWaterOffset = 0;
			if ((blocks[x, y + 1] != WATER || mass[x, y + 1] < MinDraw) && waterMass < MaxMass)
			{

				waters[x, y].GetComponent<SpriteRenderer>().sprite = topWaterSprite;
				topWaterOffset = 0.05f;
			}
			else
			{
				waters[x, y].GetComponent<SpriteRenderer>().sprite = waterSprite;
				scaledMass = 1;
			}
			var pos = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
			waters[x, y].position = new Vector3(pos.x, pos.y - (yOffset / 2) - topWaterOffset, waters[x, y].position.z);
			waters[x, y].localScale = new Vector3(waters[x, y].localScale.x, scaledMass, 0.01f);
		}
	}
}