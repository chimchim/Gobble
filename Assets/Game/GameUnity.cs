using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.GEntity;
using Game.Component;
using Game.Systems;
using System.Linq;
using System.Linq;
using System.Text;
using System.Reflection;
using System;

public class GameUnity : MonoBehaviour 
{
	GameManager game = new GameManager();

	public AllScriptableItems AllItemsData;
	public SwimVariables SwimData;
	public GroundVariables GroundData;
	public MapVariables MapData;
	public StatsVariables StatsData;
	public MineralsGenVariables MineralsGen;
	public RopeDataVariables RopeData;
	public PrefabsVariables PrefabData;
	public ScriptableResources ResourceData;
	public LayerMasksVariables LayerMasks;
	[SerializeField]
	[Header("Gounded Variables")]
	public static float Weight;
	public static float PlayerSpeed;
	public static float JumpSpeed;
	public static float Gravity;
	public static float MaxGravity;
	public static float ExtraFallSpeedAfter;
	public static float ForceDamper;
	public static Vector2 GroundHitBox;
	public static float NetworkLerpSpeed;

	[Header("Rope Variables")]
	public static float RopeGravity;
	public static float RopeSpeedMult;
	public static float RopeDamping;
	public static float RopeLength;
	public static float RopeThrowStartSpeed;
	public static float RopeBouncy;
	public static Vector2 RopeHitBox;

	[Header("Swim Variables")]
	public static float WaterJumpSpeed;
	public static float WaterGravity;
	public static float SwimUpExtraSpeed;
	public static float SwimDownMult;
	public static float MaxWaterSpeed;
	public static float SwimSpeed;
	public static float OxygenTime;
	public static float LoseOxygenAfter;
	public static int FloatJumpEvery;

	[Header("Stat Variables")]
	public static float OxygenDPS;
	public static float FallDamage;
	public static float MaxHP;

	[Header("Map Variables")]
	public static bool RotateArm;
	public static bool DebugMode;
	public static bool CreateWater;
	public static bool GenerateIslands;
	public static bool GenerateSmallIsland;
	public static bool QuickJoin;
	public static int WaterAmountOneIn;
	public static int WaterSimulations;
	public static int WaterSimulationsPerUpdate;
	public static int MapHeight;
	public static int MapWidth;
	public static int HeightBound;
	public static int WidhtBound;
	public static int BottomBoundOffset;
	public static int TopBoundOffset;
	public static int FullWidth;
	public static int FullHeight;
	public static bool ShowMiniMap;
	public static int MiniMapBoundryX;
	public static int MiniMapBoundryY;
	public static float PerlinZoom;

	public static int MainInventorySize = 3;
	public static int BackpackInventorySize = 9;
	public InventoryBackpack InventoryBackpack;
	public InventoryMain MainInventory;
	public GameObject BuildConsole;
	public GameObject MenuObject;
	public GameObject MiniMapCanvas;
	public MiniMap MiniMap;
	public GameObject Canvas;
	public static Vector3 StartingPosition;
	public Transform StartPos;
	private bool _miniMapActive = true;
	void Start () 
	{
		//Application.targetFrameRate = 80;
		SetVariables();
		Application.runInBackground = true;
		StartingPosition = StartPos.position;

		SetFamilyID();
	
		game.Systems.CreateSystems();
		game.Initiate();
		if (!MapData.UseMenu)
		{
			game.CreateEmptyPlayer(true, "local", true, 0, Characters.Yolanda);
			game.Systems.ChangeState(game, SystemManager.GameState.Game);
			MenuObject.SetActive(false);
			game.CurrentRandom = new System.Random();
		}
		else
		{
			if (MapData.QuickJoin)
			{
				game.Systems.ChangeState(game, SystemManager.GameState.QuickJoin);
				MenuObject.SetActive(false);
			}
			else
			{
				MenuObject.SetActive(true);
				game.Systems.ChangeState(game, SystemManager.GameState.Menu);
			}
		}
	}

	public void SetMainPlayer(GameObject player, InventoryComponent inventory)
	{
		GetComponent<FollowCamera>().player = player;
		MiniMap.player = player;
		InventoryBackpack.gameObject.SetActive(true);
		MainInventory.gameObject.SetActive(true);
		inventory.InventoryBackpack = InventoryBackpack;
		inventory.MainInventory = MainInventory;
	}

	bool buildConsoleActive;
	int normal = 0;
	void Update () 
	{
        game.Update(Time.deltaTime);

		SetVariables();

		StartingPosition = StartPos.position;

		if (UnityEngine.Input.GetKeyDown(KeyCode.B))
		{
			buildConsoleActive = !buildConsoleActive;
			BuildConsole.SetActive(buildConsoleActive);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.C))
		{
			Canvas.SetActive(false);
		}
	}
	void FixedUpdate()
	{
		normal = 0;
		game.FixedUpdate(Time.fixedDeltaTime);
	}

	private void SetVariables()
	{
		PlayerSpeed = GroundData.PlayerSpeed;
		JumpSpeed = GroundData.JumpSpeed;
		Gravity = GroundData.Gravity;
		ExtraFallSpeedAfter = GroundData.ExtraFallSpeedAfter;
		MaxGravity = GroundData.MaxGravity;
		Weight = GroundData.Weight;
		ForceDamper = GroundData.ForceDamper;
		GroundHitBox = GroundData.GroundHitBox;
		NetworkLerpSpeed = GroundData.NetworkLerpSpeed;

		RopeGravity = RopeData.RopeGravity;
		RopeSpeedMult = RopeData.RopeSpeedMult;
		RopeDamping = RopeData.RopeDamping;
		RopeLength = RopeData.RopeLength;
		RopeThrowStartSpeed = RopeData.RopeThrowStartSpeed;
		RopeBouncy =  RopeData.RopeBouncy;
		RopeHitBox = RopeData.RopeHitBox;
		//Water
		WaterJumpSpeed = SwimData.WaterJumpSpeed;
		WaterGravity = SwimData.WaterGravity;
		SwimSpeed = SwimData.SwimSpeed;
		SwimUpExtraSpeed = SwimData.SwimUpExtraSpeed;
		SwimDownMult = SwimData.SwimDownMult;
		OxygenTime = SwimData.OxygenTime;
		LoseOxygenAfter = SwimData.LoseOxygenAfter;
		MaxWaterSpeed = SwimData.MaxWaterSpeed;
		FloatJumpEvery = SwimData.FloatJumpEvery;

		//Stats 
		OxygenDPS = StatsData.OxygenDPS;
		FallDamage = StatsData.FallDamage;
		MaxHP = StatsData.MaxHP;

		//Map
		RotateArm = MapData.RotateArm;
		DebugMode = MapData.DebugMode;
		CreateWater = MapData.CreateWater;
		GenerateIslands = MapData.GenerateIslands;
		GenerateSmallIsland = MapData.GenerateSmallIsland;
		QuickJoin = MapData.QuickJoin;
		WaterAmountOneIn = MapData.WaterAmountOneIn;
		WaterSimulations = MapData.WaterSimulations;
		WaterSimulationsPerUpdate = MapData.WaterSimulationsPerUpdate;
		MapHeight = MapData.MapHeight;
		MapWidth = MapData.MapWidth;
		HeightBound = MapData.HeightBound;
		WidhtBound = MapData.WidhtBound;
		BottomBoundOffset = MapData.BottomBoundOffset;
		TopBoundOffset = MapData.TopBoundOffset;
		ShowMiniMap = MapData.ShowMiniMap;
		MiniMapBoundryX = MapData.MiniMapBoundryX;
		MiniMapBoundryY = MapData.MiniMapBoundryY;
		FullWidth = MapWidth + (WidhtBound * 2);
		FullHeight = MapHeight + (HeightBound * 2) + BottomBoundOffset + TopBoundOffset;
		PerlinZoom = MapData.PerlinZoom;

	}
    void LateUpdate()
    {
		
		if (ShowMiniMap && !_miniMapActive)
		{
			_miniMapActive = true;
			MiniMapCanvas.SetActive(true);
			MiniMap.gameObject.SetActive(true);
		}
		else if(!ShowMiniMap && _miniMapActive)
		{
			_miniMapActive = false;
			MiniMapCanvas.SetActive(false);
			MiniMap.gameObject.SetActive(false);
		}
    }

    private void SetFamilyID()
    {
        UnityEngine.Debug.Log("SETID");
        var assembly = Assembly.GetExecutingAssembly();
        var types = FindSubClassesOf<GComponent>();
        int i = 0;
        foreach (var type in types)
        {
            GComponent.AddID(i, type);
            i++;
        }
    }
    public static IEnumerable<Type> FindSubClassesOf<TBaseType>()
    {
        var baseType = typeof(TBaseType);
        var assembly = baseType.Assembly;

        return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
    }
}
