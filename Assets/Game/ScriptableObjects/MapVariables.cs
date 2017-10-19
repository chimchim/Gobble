using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Map", order = 1)]
public class MapVariables : ScriptableObject
{
	public bool GenerateIslands = true;
	public bool ShowMiniMap = true;
	public bool DebugMode = true;
	public bool CreateWater = true;
	public bool GenerateSmallIsland = false;
	public bool UseMenu = false;
	public bool QuickJoin = false;
	public int WaterAmountOneIn = 5;
	public int WaterSimulations = 9000;
	public int WaterSimulationsPerUpdate = 3;
	public int MapHeight = 50;
	public int MapWidth = 90;
	public int HeightBound = 8;
	public int WidhtBound = 8;
	public int BottomBoundOffset = 4;
	public int TopBoundOffset = 4;
	public int MiniMapBoundryX = 6;
	public int MiniMapBoundryY = 6;
	public float PerlinZoom = 0.1f;



}