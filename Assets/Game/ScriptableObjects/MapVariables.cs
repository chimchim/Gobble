using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Map", order = 1)]
public class MapVariables : ScriptableObject
{
	public bool CreateWater = true;
	public int WaterAmountOneIn = 5;
	public int WaterSimulations = 9000;
	public int WaterSimulationsPerUpdate = 3;
	public int MapHeight = 50;
	public int MapWidth = 90;
	public int HeightBound = 8;
	public int WidhtBound = 8;
}