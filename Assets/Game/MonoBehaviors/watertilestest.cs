using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class watertilestest : MonoBehaviour
{

	public GameObject Air;
	public GameObject Water;
	public GameObject Ground;

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

	void Start()
	{
		Waters[0] = Resources.Load("Material/Water/Blue9", typeof(Material)) as Material;
		Waters[1] = Resources.Load("Material/Water/Blue8", typeof(Material)) as Material;
		Waters[2] = Resources.Load("Material/Water/Blue7", typeof(Material)) as Material;
		Waters[3] = Resources.Load("Material/Water/Blue6", typeof(Material)) as Material;
		Waters[4] = Resources.Load("Material/Water/Blue5", typeof(Material)) as Material;
		Waters[5] = Resources.Load("Material/Water/Blue4", typeof(Material)) as Material;
		Waters[6] = Resources.Load("Material/Water/Blue3", typeof(Material)) as Material;
		Waters[7] = Resources.Load("Material/Water/Blue2", typeof(Material)) as Material;
		Waters[8] = Resources.Load("Material/Water/Blue1", typeof(Material)) as Material;
		Waters[9] = Resources.Load("Material/Water/Blue", typeof(Material)) as Material;
		blocks = new int[map_width + 2, map_height + 2];//[map_height + 2];
		mass = new float[map_width + 2, map_height + 2];
		new_mass = new float[map_width + 2, map_height + 2];


		initmap();
	}

	void Update()
	{
		currentDrawIndex = 0;
		simulate_compression();
		for (int x = 0; x < drawed.Count; x++)
		{
			drawed[x].transform.position = new Vector3(-1111, -1111, 0);
		}
		for (int x = 1; x < map_height + 2; x++)
		{
			for (int y = 1; y < map_height + 2; y++)
			{
				if (blocks[x, y] == WATER)
				{
					if (mass[x, y] < MinDraw)
					{

					}
					else
					{
						if (mass[x, y] < MaxMass)
						{
							//Draw a half-full block. Block size is dependent on the amount of water in it.
							if (mass[x, y + 1] >= MinDraw)
							{
								draw_block(x, y, mass[x, y + 1], 1);
							}
							draw_block(x, y, mass[x, y], mass[x, y]);
						}
						else
						{

							draw_block(x, y, mass[x, y], 1);
						}
					}
				}
			}
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
			go = Instantiate(Water);
			objs.Add(go);
			drawed.Add(go);
			currentDrawIndex++;
		}
		else
		{

			go = objs[currentDrawIndex];
			if(!drawed.Contains(go))
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

	void initmap()
	{
		for (int x = 0; x < map_height + 2; x++)
		{
			for (int y = 0; y < map_height + 2; y++)
			{
				blocks[x, y] = Random.Range(0, 3);// int(random(0, 3));

				if (GROUND == blocks[x, y])
				{
					var water = Instantiate(Ground);
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
	// Update is called once per frame



}
