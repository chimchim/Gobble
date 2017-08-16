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
	public static float PlayerSpeed = 6;
	public static float JumpSpeed = 5.0F;
	public static float Gravity = 0.5F;

	public float PlayerSpeedPub = 6;
	public float JumpSpeedPub = 5.0F;
	public float GravityPub = 0.5F;

	public static Vector3 StartingPosition;
	public Transform StartPos;
	private int entity;
	void Start () 
	{
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
