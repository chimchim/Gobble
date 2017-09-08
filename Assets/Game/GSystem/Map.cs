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
		private GameObject[,] Blocks;

		int mapheightEdges = 55;
		int mapwidthEdges = 110;
		int mapheight = 50;
		int mapwidth = 100;
		int heightBound = 10;
		int widhtBound = 10;

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

		float MaxSpeed = 1;   //max units of water moved out of one block to another, per timestep

		float MinFlow = 0.01f;

		//ne map dimensions and data structures
		int map_width = 16;
		int map_height = 16;

		int[,] blocks;

		float[,] mass;
		float[,] new_mass;


		public void Update(GameManager game)
		{
			
		}

		public void Initiate(GameManager game)
		{
			int fullWidhth = mapwidth + (widhtBound * 2);
			int fullHeight = mapheight + (heightBound * 2);

			Blocks = new GameObject[fullWidhth, mapheight + (heightBound * 2)];

			for (int x = 0; x < fullWidhth; x++)
			{
				for (int y = 0; y < fullHeight; y++)
				{
					if ((x < widhtBound || x > (mapwidth + widhtBound)) || (y < heightBound || y > (mapheight + heightBound)))
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
						//Tiles.Add(new Vector2(x, y), cube);
						Blocks[x, y] = cube;
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

					int posX = x + widhtBound;
					int posY = y + heightBound;
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
						cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
						Blocks[posX, posY] = cube;
					}
					else
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						Blocks[posX, posY] = cube;
					}

					

			}

			Sprite newMat = null;
			
			for (int x = 0; x < fullWidhth; x++)
			{
				for (int y = 0; y < fullHeight; y++)
				{
					Vector2 pos = new Vector2(x, y);


					var top = ((y + 1) < fullHeight) && (Blocks[x, y + 1] != null);
					var bot = ((y - 1) >  0) && (Blocks[x, y - 1] != null);
					var right = ((x + 1) < fullWidhth) && (Blocks[x + 1, y ] != null);
					var left = ((x - 1) > 0) && (Blocks[x - 1, y] != null);
					if (Blocks[x, y] == null)
					{
						continue;
					}

					if (!top && !right)
					{
						newMat = Resources.Load("Tiles/TopRight", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "TopRight";
					}
					if (!top && !left)
					{
						newMat = Resources.Load("Tiles/TopLeft", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "TopLeft";
					}
					if (!top && right && left)
					{
						newMat = Resources.Load("Tiles/Top", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "Top";
					}
					if (top && right && left && bot)
					{
						newMat = Resources.Load("Tiles/Middle", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "Middle";
					}
					if (top && !right && bot)
					{
						newMat = Resources.Load("Tiles/MiddleRight", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "MiddleRight";
					}
					if (top && !left && bot)
					{
						newMat = Resources.Load("Tiles/MiddleLeft", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "MiddleLeft";
					}
					if (top && !right && !bot)
					{
						newMat = Resources.Load("Tiles/BotRightCorner", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "BotRightCorner";
					}
					if (top && !left && !bot)
					{
						newMat = Resources.Load("Tiles/BotLeftCorner", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "BotLeftCorner";
					}
					if (top && left && !bot && right)
					{
						newMat = Resources.Load("Tiles/Bot", typeof(Sprite)) as Sprite;
						Blocks[x, y].name = "Bot";
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
						var middleLeft = ((x - 1) > 0) && ((y + 1) < fullHeight) && (Blocks[x - 1, y + 1] != null);
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

			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var go = game.Entities.GetEntity(entity);
				go.gameObject.transform.position = GameUnity.StartingPosition;
			}
			InitiateWater();
		}

		

		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
		void InitiateWater()
		{
			int fullWidhth = mapwidth + (widhtBound * 2);
			int fullHeight = mapheight + (heightBound * 2);

			for (int x = 0; x < fullWidhth + 2; x++)
			{
				for (int y = 0; y < fullHeight + 2; y++)
				{
					blocks[x, y] = Random.Range(0, 3);// int(random(0, 3));

					if (GROUND == blocks[x, y])
					{
						//var water = Instantiate(Ground);
						var water = new GameObject();
						water.transform.position = new Vector3(x, y, 0);
					}
					mass[x, y] = blocks[x, y] == WATER ? MaxMass : 0.0f;
					new_mass[x, y] = blocks[x, y] == WATER ? MaxMass : 0.0f;
				}
			}

			for (int x = 0; x < map_width + 2; x++)
			{
				blocks[x, 0] = AIR;
				blocks[x, map_height + 1] = AIR;
			}

			for (int y = 1; y < map_height + 1; y++)
			{
				blocks[0, y] = AIR;
				blocks[map_width + 1, y] = AIR;
			}

		}
		float get_stable_state_b(float total_mass)
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
		Color waterColor(float m)
		{
			Color c = new Color();

			m = Mathf.Clamp(m, MinDraw, MaxDraw);

			int r = 50, g = 50;
			int b;

			if (m < 1)
			{
				b = (int)(map(m, 0.01f, 1, 255, 200));
				r = (int)(map(m, 0.01f, 1, 240, 50));
				r = Mathf.Clamp(r, 50, 240);
				g = r;
			}
			else
			{
				b = (int)(map(m, 1, 1.1f, 190, 140));
			}

			b = Mathf.Clamp(b, 140, 255);
			c.b = b;
			c.g = g;
			c.r = r;
			c.g = g;
			return c;
		}
		float map(float s, float a1, float a2, float b1, float b2)
		{
			return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
		}

		int currentDrawIndex = 0;
		public List<GameObject> objs = new List<GameObject>();
		public List<GameObject> drawed = new List<GameObject>();
		private void draw_block(int x, int y, float color, float mass)
		{
			GameObject go;
			if (currentDrawIndex >= objs.Count)
			{
				//go = Instantiate(Water);
				go = new GameObject();
				objs.Add(go);
				drawed.Add(go);
				currentDrawIndex++;
			}
			else
			{

				go = objs[currentDrawIndex];
				if (!drawed.Contains(go))
					drawed.Add(go);
				currentDrawIndex++;
			}

			float scaledcolor = Mathf.Min(1, color);
			float scaledMass = Mathf.Min(1, mass);
			float yOffset = 1 - (scaledMass);
			int watertransparency = ((int)(scaledcolor * 10));
			watertransparency = Mathf.Min(watertransparency, 9);

			go.GetComponent<Renderer>().material = Waters[watertransparency];
			go.transform.position = new Vector3(x, y - (yOffset / 2), 0);
			go.transform.localScale = new Vector3(go.transform.localScale.x, scaledMass, 1);
		}
		void simulate_compression()
		{
			float Flow = 0;
			float remaining_mass;

			//Calculate and apply flow for each block
			for (int x = 1; x <= map_width; x++)
			{
				for (int y = 1; y <= map_height; y++)
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
						Flow = get_stable_state_b(remaining_mass + mass[x, y - 1]) - mass[x, y - 1];
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
						Flow = remaining_mass - get_stable_state_b(remaining_mass + mass[x, y + 1]);
						if (Flow > MinFlow) { Flow *= 0.5f; }
						Flow = Mathf.Clamp(Flow, 0, Mathf.Min(MaxSpeed, remaining_mass));

						new_mass[x, y] -= Flow;
						new_mass[x, y + 1] += Flow;
						remaining_mass -= Flow;
					}


				}
			}

			//Copy the new mass values to the mass array
			for (int x = 0; x < map_width + 2; x++)
			{
				for (int y = 0; y < map_height + 2; y++)
				{
					mass[x, y] = new_mass[x, y];
				}
			}

			for (int x = 1; x <= map_width; x++)
			{
				for (int y = 1; y <= map_height; y++)
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
			for (int x = 0; x < map_width + 2; x++)
			{
				mass[x, 0] = 0;
				mass[x, map_height + 1] = 0;
			}
			for (int y = 1; y < map_height + 1; y++)
			{
				mass[0, y] = 0;
				mass[map_width + 1, y] = 0;
			}

		}
	}
}