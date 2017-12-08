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

		public List<VisibleItem> WorldItems = new List<VisibleItem>();
		public void CreateEmptyPlayer(bool owner, string name, bool isHost, int team, Characters character, int reservedID = -1)
		{
			Entity ent = new Entity(reservedID);
			this.Entities.addEntity(ent);
			
			ent.AddComponent(Player.MakeFromLobby(ent.ID, owner, name, isHost, team, character));
			ent.AddComponent(ResourcesComponent.Make(ent.ID));
			
			Debug.Log("Create empty player ID " + ent.ID + " isowner " + owner + " name " + name + " ishost " + isHost + " TEAM " + team);
		}

		public void CreateFullPlayer(bool owner, string name, bool isHost, int team, Characters character, int reservedID = -1)
		{
			Entity ent = new Entity(reservedID);
			this.Entities.addEntity(ent);
			ent.AddComponent(Player.MakeFromLobby(ent.ID, owner, name, isHost, team, character));
			ent.AddComponent(ResourcesComponent.Make(ent.ID));
			ent.AddComponent(ActionQueue.Make(ent.ID));
			ent.AddComponent(MovementComponent.Make(ent.ID));
			ent.AddComponent(Stats.Make(ent.ID, 100, GameUnity.OxygenTime, GameUnity.OxygenTime));
			ent.AddComponent(InputComponent.Make(ent.ID));
			ent.AddComponent(ItemHolder.Make(ent.ID));
			ent.AddComponent(NetEventComponent.Make(ent.ID));
			var inventory = InventoryComponent.Make(ent.ID);
			ent.AddComponent(inventory);
			var player = Entities.GetComponentOf<Player>(ent.ID);
			var resources = Entities.GetComponentOf<ResourcesComponent>(ent.ID);

			var playerGameObject = GameObject.Instantiate(GetCharacterObject(player.Character), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			playerGameObject.tag = "Player";
			ent.gameObject = playerGameObject;
			ent.Animator = playerGameObject.GetComponentInChildren<Animator>();
			playerGameObject.AddComponent<IdHolder>().ID = ent.ID;
			playerGameObject.GetComponent<IdHolder>().Owner = owner;
			playerGameObject.transform.position = new Vector3((GameUnity.FullWidth / 2), (GameUnity.FullHeight / 2), 0);

			GameObject Ropes = new GameObject();
			Ropes.AddComponent<GraphicRope>().Owner = owner;
			Ropes.GetComponent<GraphicRope>().MakeRopes();
			resources.GraphicRope = Ropes.GetComponent<GraphicRope>();
			resources.FreeArm = playerGameObject.transform.Find("free_arm");
			resources.FreeArmAnimator = resources.FreeArm.Find("animator").GetComponent<Animator>();
			resources.Hand = resources.FreeArmAnimator.transform.Find("hand");
		}

		public void SetMainPlayer(GameObject player, InventoryComponent inventory)
		{
			_gameUnity.SetMainPlayer(player, inventory);

		}
		public GameManager()
		{
			
		}

        public void Update(float delta)
        {
			if (Client != null)
			{
				Client._byteDataBuffer.AddRange(Client._currentByteData);
				Client._currentByteData.Clear();
			}
			_systemManager.NormalUpdate(this, delta);
			if (Client != null)
			{
				Client._byteDataBuffer.Clear();
			}
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
			GameResources.AllItems = _gameUnity.AllItemsData;

			Debug.Log("Initiate GameManager");
			_systemManager.InitAll(this);
			
		}

		public GameObject GetCharacterObject(Characters character)
		{
			switch (character)
			{
				case Characters.Milton:
					return GameResources.Prefabs.Milton;

				case Characters.Peppermin:
					return GameResources.Prefabs.Peppermin;

				case Characters.Yolanda:
					return GameResources.Prefabs.Yolanda;

				case Characters.Schmillo:
					return GameResources.Prefabs.Schmillo;

			}
			return GameResources.Prefabs.Milton;
		}

	}
}
