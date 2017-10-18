using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using UnityEngine;

namespace Game
{
	public class GameManager
	{
        private EntityManager _entityManager = new EntityManager();
		private SystemManager _systemManager = new SystemManager();

        public EntityManager Entities { get { return _entityManager; } }
		public SystemManager Systems { get { return _systemManager; } }

		public GameResources GameResources;
		public TileMap TileMap;
		private GameUnity _gameUnity;
		public Client Client;
		public System.Random CurrentRandom;
		public void CreateEmptyPlayer(bool owner, string name, bool isHost, int team, Characters character, int reservedID = -1)
		{
			Entity ent = new Entity(reservedID);
			this.Entities.addEntity(ent);
			
			ent.AddComponent(Player.MakeFromLobby(ent.ID, owner, name, isHost, team, character));
			ent.AddComponent(Game.Component.Resources.Make(ent.ID));
			Debug.Log("Create empty player ID " + ent.ID + " isowner " + owner + " name " + name + " ishost " + isHost + " TEAM " + team);
		}
		//public void CreatePlayerLocalPlayer(bool owner)
		//{
		//	Entity ent = new Entity();
		//	this.Entities.addEntity(ent);
		//	ent.AddComponent(ActionQueue.Make(ent.ID));
		//	ent.AddComponent(Game.Component.Movement.Make(ent.ID));
		//	ent.AddComponent(Stats.Make(ent.ID, 100, GameUnity.OxygenTime, GameUnity.OxygenTime));
		//	ent.AddComponent(Game.Component.Input.Make(ent.ID));
		//	ent.AddComponent(Game.Component.Resources.Make(ent.ID));
		//	ent.AddComponent(Player.MakeFromLobby(ent.ID, owner, "local", true, 0, Characters.Yolanda));
		//
		//	//var player = GameObject.Instantiate(_gameUnity.PrefabData.Peppermin, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		//	//player.tag = "Player";
		//	//ent.gameObject = player;
		//	//ent.Animator = player.GetComponentInChildren<Animator>();
		//	//if (ent.Animator)
		//	//	Debug.Log("animator exists");
		//	//if (owner)
		//	//{
		//	//	_gameUnity.SetMainPlayer(player);
		//	//}
		//}

		public void SetMainPlayer(GameObject player)
		{
			_gameUnity.SetMainPlayer(player);
		}
		public GameManager()
		{
			
		}

        public void Update(float delta)
        { 
			_systemManager.NormalUpdate(this, delta);

			if (GameUnity.CreateWater && TileMap != null)
			{
				TileMap.UpdateWater();
			}
		}
		public void FixedUpdate(float delta)
		{
			_systemManager.FixedUpdate(this, delta);
		}
		public void Initiate()
		{
			_gameUnity = GameObject.FindObjectOfType<GameUnity>();
			GameResources = new GameResources();
			GameResources.Prefabs = _gameUnity.PrefabData;
            Debug.Log("Initiate GameManager");
			_systemManager.InitAll(this);
		}
	}
}
