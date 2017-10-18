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
