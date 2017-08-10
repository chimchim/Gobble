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
using Game.AI;
using Game.AI.Behaviors;

public class GameUnity : MonoBehaviour 
{
    public GameObject Prefab;
    public GameObject AIPrefab;
    public GameObject Aim;
	GameManager game = new GameManager();
    public GameObject player;
    public GameObject camera;
    public Dictionary<int, Transform> entityDic = new Dictionary<int, Transform>();
    private int entity;
	void Start () 
	{
        SetFamilyID();
		//Entity ent = new Entity();
		//game.Entities.addEntity(ent);
        //ent.AddComponent(ActionQueue.Make(ent.ID));
        //ent.AddComponent(Movement.Make(ent.ID));
		//ent.AddComponent(Stats.Make(ent.ID));
        //ent.AddComponent(Player.Make(ent.ID));
        //player = Instantiate(Prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //player.tag = "Player";
        //ent.gameObject = player;
		//
        //entity = ent.ID;
        //entityDic.Add(ent.ID, player.transform);
        game.Systems.CreateSystems();
        game.Initiate();        
	}

	void Update () 
	{
        game.Update(Time.deltaTime);

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
