using Game;
using System.Collections;
using System.Collections.Generic;
using Gatherables;
using UnityEngine;

public partial class TileMap
{
	GameObject Water;
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
	public Transform[,] Waters;

	float[,] mass;
	float[,] new_mass;

	Sprite topWaterSprite;
	Sprite waterSprite;

	public void InitiateWater(GameManager game)
	{
		if (!GameUnity.CreateWater)
		{
			return;
		}
		topWaterSprite = Resources.Load("Tiles/TopWater", typeof(Sprite)) as Sprite;
		waterSprite = Resources.Load("Tiles/Middlewater", typeof(Sprite)) as Sprite;
		Water = GameObject.Instantiate(Resources.Load("Prefabs/Water", typeof(GameObject))) as GameObject;
		Water.transform.position = new Vector3(-1111, -1111, 0);

		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;

		blocks = new int[fullWidhth + 2, fullHeight + 2];
		mass = new float[fullWidhth + 2, fullHeight + 2];
		new_mass = new float[fullWidhth + 2, fullHeight + 2];
		Waters = new Transform[fullWidhth + 2, fullHeight + 2];
		for (int x = 0; x < fullWidhth + 2; x++)
		{
			for (int y = 0; y < fullHeight + 2; y++)
			{
				blocks[x, y] = game.CurrentRandom.Next(0, GameUnity.WaterAmountOneIn);
				if (blocks[x, y] == GROUND || blocks[x, y] > WATER)
				{
					blocks[x, y] = 0;
				}
				if ((y < fullHeight) && (x < fullWidhth) && Blocks[x, y] != null)
				{
					blocks[x, y] = 1;
					var gb = Blocks[x, y].GetComponent<GatherableBlock>();
					if (gb != null && gb.IngredientType == IngredientType.TreeChunk)
					{
						blocks[x, y] = 0;
					}
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
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;
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
		int fullWidhth = GameUnity.FullWidth;
		int fullHeight = GameUnity.FullHeight;

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
						if (Waters[x, y] != null)
						{
							GameObject.Destroy(Waters[x, y].gameObject);
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
					if (Waters[x, y] != null)
					{
						GameObject.Destroy(Waters[x, y].gameObject);
					}
				}
			}
		}
	}
	private void DrawBlock(int x, int y, float color, float waterMass)
	{
		GameObject go;
		if (Waters[x, y] == null)
		{
			go = GameObject.Instantiate(Water);
			Waters[x, y] = go.transform;
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

			Waters[x, y].GetComponent<SpriteRenderer>().sprite = topWaterSprite;
			topWaterOffset = 0.05f;
		}
		else
		{
			Waters[x, y].GetComponent<SpriteRenderer>().sprite = waterSprite;
			scaledMass = 1;
		}
		var pos = new Vector3(x + (0.28f * x), y + (0.28f * y), 0);
		Waters[x, y].position = new Vector3(pos.x, pos.y - (yOffset / 2) - topWaterOffset, Waters[x, y].position.z);
		Waters[x, y].localScale = new Vector3(Waters[x, y].localScale.x, scaledMass, 0.01f);
	}
}
