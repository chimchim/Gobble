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

		int mapheight = 50;
		int mapwidth = 90;
		int heightBound = 8;
		int widhtBound = 8;

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
		public void Update(GameManager game)
		{
			if (GameUnity.CreateWater)
			{
				UpdateWater();
			}
		}
		public void UpdateWater()
		{
			int fullWidhth = mapwidth + (widhtBound * 2);
			int fullHeight = mapheight + (heightBound * 2);

			for (int i = 0; i < GameUnity.WaterSimulationsPerUpdate; i++)
			{
				simulate_compression();
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
								draw_block(x, y, mass[x, y], scaledSize);
							}
							else
							{

								draw_block(x, y, mass[x, y], 1);
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
		private void draw_block(int x, int y, float color, float waterMass)
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
			//waters[x, y].GetComponent<Renderer>().material = Waters[watertransparency];
			if ((blocks[x, y + 1] != WATER || mass[x, y + 1] < MinDraw) && waterMass < MaxMass)
			{

				waters[x, y].GetComponent<SpriteRenderer>().sprite = topWaterSprite;
				topWaterOffset = 0.05f;
			}
			else
			{
				waters[x, y].GetComponent<SpriteRenderer>().sprite = waterSprite;
				scaledMass = 1;
				//waters[x, y].GetComponent<Renderer>().material = Waters[watertransparency];
			}
			var pos = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
			waters[x, y].position = new Vector3(pos.x, pos.y - (yOffset / 2) -topWaterOffset, 0.1f);
			waters[x, y].localScale = new Vector3(waters[x, y].localScale.x, scaledMass, 0.01f);
		}
		public void Initiate(GameManager game)
		{
			int fullWidhth = mapwidth + (widhtBound * 2);
			int fullHeight = mapheight + (heightBound * 2);
			topWaterSprite = Resources.Load("Tiles/TopWater", typeof(Sprite)) as Sprite;
			waterSprite = Resources.Load("Tiles/Middlewater", typeof(Sprite)) as Sprite;
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
						cube.layer = LayerMask.NameToLayer("Collideable");
						cube.transform.position = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
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
						cube.layer = LayerMask.NameToLayer("Collideable");
						Blocks[posX, posY] = cube;
					}
					else
					{
						GameObject cube = new GameObject();
						cube.AddComponent<SpriteRenderer>();
						cube.AddComponent<BoxCollider2D>();
						cube.transform.position = new Vector3(posX + (0.28f * posX), posY + (0.28f * posY), 0);
						cube.GetComponent<BoxCollider2D>().size = new Vector2(1.28f, 1.28f);
						cube.layer = LayerMask.NameToLayer("Collideable");
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
					var bot = ((y - 1) >=  0) && (Blocks[x, y - 1] != null);
					var right = ((x + 1) < fullWidhth) && (Blocks[x + 1, y ] != null);
					var left = ((x - 1) >= 0) && (Blocks[x - 1, y] != null);
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

			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var go = game.Entities.GetEntity(entity);
				go.gameObject.transform.position = GameUnity.StartingPosition;
			}
			if (GameUnity.CreateWater)
			{
				InitiateWater();
				for (int i = 0; i < GameUnity.WaterSimulations; i++)
				{
					simulate_compression();
				}
			}
		}

		

		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
		void InitiateWater()
		{
			Water = GameObject.Instantiate(Resources.Load("Prefabs/Water", typeof(GameObject))) as GameObject;
			Water.transform.position = new Vector3(-1111, -1111, 0);
			Waters[0] = Resources.Load("Material/Water/Blue9", typeof(Material)) as Material;
			Waters[1] = Resources.Load("Material/Water/Blue8", typeof(Material)) as Material;
			Waters[2] = Resources.Load("Material/Water/Blue7", typeof(Material)) as Material;
			Waters[3] = Resources.Load("Material/Water/Blue6", typeof(Material)) as Material;
			Waters[4] = Resources.Load("Material/Water/Blue5", typeof(Material)) as Material;
			Waters[5] = Resources.Load("Material/Water/Blue4", typeof(Material)) as Material;
			Waters[6] = Resources.Load("Material/Water/Blue3", typeof(Material)) as Material;
			Waters[7] = Resources.Load("Material/Water/Blue3", typeof(Material)) as Material;
			Waters[8] = Resources.Load("Material/Water/Blue3", typeof(Material)) as Material;
			Waters[9] = Resources.Load("Material/Water/Blue3", typeof(Material)) as Material;

			int fullWidhth = mapwidth + (widhtBound * 2);
			int fullHeight = mapheight + (heightBound * 2);

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


		void simulate_compression()
		{
			float Flow = 0;
			float remaining_mass;
			int fullWidhth = mapwidth + (widhtBound * 2);
			int fullHeight = mapheight + (heightBound * 2);
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
	}
}