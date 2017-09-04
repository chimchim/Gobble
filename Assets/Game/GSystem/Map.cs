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
		private GameObject[,] Blocks;
		int mapheightEdges = 55;
		int mapwidthEdges = 110;
		int mapheight = 50;
		int mapwidth = 100;

		int heightBound = 10;
		int widhtBound = 10;

		float[,] mass;
		float[,] new_mass;
		int[,] blocks;

		public List<GameObject> objs = new List<GameObject>();
		public List<GameObject> drawed = new List<GameObject>();

		int AIR = 0;
		int GROUND = 1;
		int WATER = 2;

		float MaxMass = 1.0f;
		float MaxCompress = 0.02f;
		float MinMass = 0.0001f;

		float MinDraw = 0.01f;
		float MaxDraw = 1.1f;

		float MaxSpeed = 1f;   //max units of water moved out of one block to another, per timestep

		float MinFlow = 0.01f;

		public void Update(GameManager game)
		{
			simulate_compression();
			int w = mapwidth + (widhtBound * 2);
			int h = mapheight + (heightBound * 2);
			for (int x = 1; x < w; x++)
			{
				for (int y = 1; y < h; y++)
				{
					if (blocks[x, y] == WATER)
					{

						//Skip cells that contain very little water
						if (mass[x, y] < MinDraw) continue;

						//Draw water
						if (true && (mass[x, y] < MaxMass))
						{
							//Draw a half-full block. Block size is dependent on the amount of water in it.
							if (mass[x, y + 1] >= MinDraw)
							{
								draw_block(x, y, mass[x, y + 1]);
							}
							draw_block(x, y, mass[x, y]);
						}
						else
						{
							//Draw a full block
							//h = 1;
							//c = waterColor(mass[x,y]);
							draw_block(x, y, mass[x, y]);
						}

					}
					else
					{
						//Draw any other block
						//draw_block(x, y, block_colors[blocks[x,y]], 1);
					}

				}
			}
			drawed.Clear();
			currentDrawIndex = 0;
		}
		int currentDrawIndex = 0;
		private void draw_block(int x, int y, float mass)
		{
			//Debug.Log("go.transform.position() " + objs[0].transform.position);
			if (currentDrawIndex >= objs.Count)
			{


				GameObject water = new GameObject();
				water.AddComponent<SpriteRenderer>();
				var newMat = Resources.Load("Tiles/MiddleWater", typeof(Sprite)) as Sprite;
				water.GetComponent<SpriteRenderer>().sprite = newMat;
				water.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
				objs.Add(water);
				drawed.Add(water);
				currentDrawIndex++;
			}
			else
			{
				var go = objs[currentDrawIndex];
				go.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
				drawed.Add(go);
				currentDrawIndex++;
			}
		}
		void simulate_compression()
		{
			float Flow = 0;
			float remaining_mass;

			int w = mapwidth + (widhtBound * 2);
			int h = mapheight + (heightBound * 2);
			//Calculate and apply flow for each block
			for (int x = 1; x < w; x++)
			{
				for (int y = 1; y < h; y++)
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
						//Debug.Log("Flow " + Flow);
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
			for (int x = 0; x < w ; x++)
			{
				for (int y = 0; y < h; y++)
				{
					mass[x, y] = new_mass[x, y];
				}
			}

			for (int x = 1; x < w; x++)
			{
				for (int y = 1; y < h; y++)
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
			for (int x = 0; x < w ; x++)
			{
				mass[x, 0] = 0;
				mass[x, h -1] = 0;
			}
			for (int y = 1; y < h ; y++)
			{
				mass[0, y] = 0;
				mass[w -1, y] = 0;
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

		public void InitiateWater()
		{
			int w = mapwidth + (widhtBound * 2);
			int h = mapheight + (heightBound * 2);

			mass = new float[w, h];
			new_mass = new float[w, h];
			blocks = new int[w, h];

			Sprite newMat = null;
			for (int x = 0; x < w ; x++)
			{
				for (int y = 0; y < h ; y++)
				{
					int block = Random.Range(0, 3);
					if (Blocks[x, y] != null)
					{
						block = 1;

					}
					else if (WATER == block)
					{
						GameObject water = new GameObject();
						water.AddComponent<SpriteRenderer>();
						newMat = Resources.Load("Tiles/MiddleWater", typeof(Sprite)) as Sprite;
						water.GetComponent<SpriteRenderer>().sprite = newMat;
						//var water = Instantiate(Water);
						//water.transform.position = new Vector3(x, y, 0);
						objs.Add(water);
					}

					blocks[x, y] = block;
					mass[x, y] = blocks[x, y] == WATER ? MaxMass : 0.0f;
					new_mass[x, y] = blocks[x, y] == WATER ? MaxMass : 0.0f;
				}
			}
		}
		public void Initiate(GameManager game)
		{
			Blocks = new GameObject[mapwidth + (widhtBound * 2), mapheight + (heightBound * 2)];

			for (int x = 0; x < mapwidth + (widhtBound * 2); x++)
			{
				for (int y = 0; y < mapheight + (heightBound * 2) ; y++)
				{
					if ((x < widhtBound || x > (mapwidth + widhtBound)) || (y < heightBound || y > (mapheight + heightBound)))
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
						Tiles.Add(new Vector2(x, y), cube);
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
						Tiles.Add(new Vector2(posX, posY), cube);
						Blocks[posX, posY] = cube;
					}
					else
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						Tiles.Add(new Vector2(posX, posY), cube);
						Blocks[posX, posY] = cube;
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
			InitiateWater();
		}

		

		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}