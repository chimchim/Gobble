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
    public GameObject Prefab;
	GameManager game = new GameManager();
    public GameObject camera;
    public Dictionary<int, Transform> entityDic = new Dictionary<int, Transform>();

	[SerializeField]
	[Header("Gounded Variables")]
	public static float PlayerSpeed = 6;
	public static float JumpSpeed = 5.0F;
	public static float Gravity = 0.5F;
	public static float MaxGravity = 2.5F;

	[Header("Swim Variables")]
	public static float WaterGravity = 0.2F;
	public static float SwimUpExtraSpeed = 1.0F;
	public static float SwimDownMult = 0.77F;
	public static float MaxWaterSpeed = 1.0F;
	public static float SwimSpeed = 6;
	public static bool CreateWater = true;

	public bool CreateWaterPub = true;
	public float PlayerSpeedPub = 6;
	public float JumpSpeedPub = 5.0F;
	public float GravityPub = 0.5F;

	public float MaxGravityPub = 2.5F;
	public float WaterGravityPub = 0.2F;
	public float SwimUpExtraSpeedPub = 3.2F;
	public float SwimDownMultPub = 3.2F;
	public float SwimSpeedPub = 6f;
	public float MaxWaterSpeedPub = 6f;

	public static Vector3 StartingPosition;
	public Transform StartPos;
	private int entity;
	void Start () 
	{
		PlayerSpeed = PlayerSpeedPub;
		JumpSpeed = JumpSpeedPub;
		Gravity = GravityPub;

		WaterGravity = WaterGravityPub;
		SwimSpeed = SwimSpeedPub;
		SwimUpExtraSpeed = SwimUpExtraSpeedPub;
		SwimDownMult = SwimDownMultPub;
		MaxGravity = MaxGravityPub;

		CreateWater = CreateWaterPub;
		MaxWaterSpeed = MaxWaterSpeedPub;
		StartingPosition = StartPos.position;

		SetFamilyID();
		Entity ent = new Entity();
		game.Entities.addEntity(ent);
        ent.AddComponent(ActionQueue.Make(ent.ID));
        ent.AddComponent(Game.Component.Input.Make(ent.ID));
		//ent.AddComponent(Stats.Make(ent.ID));
		ent.AddComponent(Player.Make(ent.ID, true));
        var player = Instantiate(Prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        player.tag = "Player";
        ent.gameObject = player;

		GetComponent<Camera>().player = player;
		entity = ent.ID;
        entityDic.Add(ent.ID, player.transform);
        game.Systems.CreateSystems();
        game.Initiate();        
	}

	void Update () 
	{
        game.Update(Time.deltaTime);


		PlayerSpeed = PlayerSpeedPub;
		JumpSpeed = JumpSpeedPub;
		Gravity = GravityPub;

		WaterGravity = WaterGravityPub;
		SwimSpeed = SwimSpeedPub;
		SwimUpExtraSpeed = SwimUpExtraSpeedPub;
		SwimDownMult = SwimDownMultPub;
		MaxGravity = MaxGravityPub;
		MaxWaterSpeed = MaxWaterSpeedPub;

		CreateWater = CreateWaterPub;
	}
    void LateUpdate()
    {

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
