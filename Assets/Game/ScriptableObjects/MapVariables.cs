using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Map", order = 1)]
public class MapVariables : ScriptableObject
{
	public bool CreateWater = true;
	public int WaterAmountOneIn = 5;
	public int WaterSimulations = 9000;
	public int WaterSimulationsPerUpdate = 3;
}