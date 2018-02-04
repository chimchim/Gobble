using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using UnityEngine;
using Game.Movement;
using MoveState = Game.Component.MovementComponent.MoveState;
namespace Game
{
	public class E
	{
		public enum Inventory
		{
			Main,
			BackPack,
			Equipped
		}
		public enum Effects
		{
			Slice2,
			Blood3,
			Ricochet,
			Death,
			BloodDeath
		}
	}
	public class GameManager
	{
	
        private EntityManager _entityManager = new EntityManager();
		private SystemManager _systemManager = new SystemManager();

        public EntityManager Entities { get { return _entityManager; } }
		public SystemManager Systems { get { return _systemManager; } }
		public List<DelayedAction> CallBacks = new List<DelayedAction>();
		public struct DelayedAction
		{
			public Action action;
			public float delay;
		}
		public RealTimeVariables RealVariables;
		public LayerMasksVariables LayerMasks;
		public GameResources GameResources;
		public AnimalVariables Animals;
		public TileMap TileMap;
		private GameUnity _gameUnity;
		public Client Client;
		public System.Random CurrentRandom;
		public System.Random PrivateRandom;
		public List<VisibleItem> WorldItems = new List<VisibleItem>();

		public void AddAction(Action a, float d = 0)
		{
			//Debug.Log("Time " + Time.time + " d " + d)
			var action = new DelayedAction
			{
				action = a,
				delay = d + Time.time
			};
			CallBacks.Add(action);
		}
		public void CreateEmptyPlayer(bool owner, string name, bool isHost, int team, Characters character, int reservedID = -1)
		{
			Entity ent = new Entity(reservedID);
			this.Entities.addEntity(ent);
			
			ent.AddComponent(Player.MakeFromLobby(ent.ID, owner, name, isHost, team, character));
			ent.AddComponent(ResourcesComponent.Make(ent.ID));
			
			Debug.Log("Create empty player ID " + ent.ID + " isowner " + owner + " name " + name + " ishost " + isHost + " TEAM " + team);
		}
		public void CreateRabbit(Vector2 position)
		{
			Entity ent = new Entity();
			this.Entities.addEntity(ent);
			var list = new List<Movement.AnimalState>();
			var patrol = new RabbitPatrol(0);
			list.Add(patrol);
			list.Add(new RabbitChill(1));
			list.Add(new JumpFlee(2));
			//list.Add(new RabbitDig(3));
			var animal = Animal.Make(ent.ID, list);
			animal.CurrentState = patrol;
			ent.AddComponent(animal);

			var playerGameObject = GameObject.Instantiate(this.GameResources.Prefabs.Rabbit, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			playerGameObject.AddComponent<IdHolder>().ID = ent.ID;
			ent.gameObject = playerGameObject;
			ent.gameObject.transform.position = position;
			ent.Animator = playerGameObject.GetComponentInChildren<Animator>();
		}
		public void CreateFullPlayer(bool owner, string name, bool isHost, int team, int ownerTeam, Characters character, int reservedID = -1)
		{
			Entity ent = new Entity(reservedID);
			this.Entities.addEntity(ent);
			ent.AddComponent(Player.MakeFromLobby(ent.ID, owner, name, isHost, team, character));
			ent.AddComponent(ResourcesComponent.Make(ent.ID));
			ent.AddComponent(ActionQueue.Make(ent.ID));
			ent.AddComponent(Stats.Make(ent.ID, 100, GameUnity.OxygenTime, GameUnity.OxygenTime));
			ent.AddComponent(InputComponent.Make(ent.ID));
			ent.AddComponent(NetEventComponent.Make(ent.ID));
			var inventory = InventoryComponent.Make(ent.ID);
			ent.AddComponent(inventory);
			var player = Entities.GetComponentOf<Player>(ent.ID);
			var resources = Entities.GetComponentOf<ResourcesComponent>(ent.ID);
			player.Enemy = (player.Team != ownerTeam);
			var playerGameObject = GameObject.Instantiate(GetCharacterByTeam(team), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

			#region SetEnemy
			var moveComp = MovementComponent.Make(ent.ID);
			ent.AddComponent(moveComp);
			((Grounded)moveComp.States[(int)MoveState.Grounded]).PlayerLayer = !player.Enemy ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("PlayerEnemy");
			((Grounded)moveComp.States[(int)MoveState.Grounded]).PlayerPlatformLayer = !player.Enemy ? LayerMask.NameToLayer("PlayerPlatform") : LayerMask.NameToLayer("PlayerEnemyPlatform");
			playerGameObject.tag = "Player";
			playerGameObject.layer = !player.Enemy ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("PlayerEnemy"); 
			#endregion

			ent.gameObject = playerGameObject;
			ent.Animator = playerGameObject.GetComponentInChildren<Animator>();
			moveComp.Body = playerGameObject.GetComponent<Rigidbody2D>();
			playerGameObject.AddComponent<IdHolder>().ID = ent.ID;
			playerGameObject.GetComponent<IdHolder>().Owner = owner;
			playerGameObject.transform.position = new Vector3((GameUnity.FullWidth / 2), (GameUnity.FullHeight / 2), 0);
			Entities.GetComponentOf<InputComponent>(ent.ID).NetworkPosition = new Vector3((GameUnity.FullWidth / 2), (GameUnity.FullHeight / 2), 0);

			GameObject Ropes = new GameObject();
			Ropes.AddComponent<GraphicRope>().Owner = owner;
			Ropes.GetComponent<GraphicRope>().MakeRopes();
			resources.GraphicRope = Ropes.GetComponent<GraphicRope>();
			resources.LerpCharacter = playerGameObject.transform.Find("graphics").GetComponent<LerpCharacter>();
			resources.FreeArm = playerGameObject.transform.Find("graphics/free_arm");
			resources.FreeArmAnimator = resources.FreeArm.Find("animator").GetComponent<Animator>();
			resources.ArmEvents = resources.FreeArmAnimator.GetComponent<AnimationEvents>();
			resources.Hand = resources.FreeArmAnimator.transform.Find("hand");

			var itemHolder = ItemHolder.Make(ent.ID);
			ent.AddComponent(itemHolder);
			var hands = EmptyHands.Make();
			hands.ItemNetID = -1;
			itemHolder.Items.Add(hands.ItemNetID, hands);
			itemHolder.Hands = hands;
			if (player.Owner)
				hands.OwnerActivate(this, itemHolder.EntityID);
			else
				hands.ClientActivate(this, itemHolder.EntityID);

		}

		public void SetMainPlayer(GameObject player, InventoryComponent inventory)
		{
			_gameUnity.SetMainPlayer(player, inventory);

		}
		public GameManager()
		{
			
		}

		public void CreateEffect(E.Effects effect, Vector2 pos, Quaternion rotation, float delay)
		{
			var go = GameObject.Instantiate(GameResources.Prefabs.Effects[(int)effect]);
			go.transform.position = new Vector3(pos.x, pos.y, -0.4f);
			go.transform.rotation = rotation;
			GameObject.Destroy(go, delay);
		}
		public void CreateEffect(E.Effects effect, Vector2 pos, Vector2 direction, float delay)
		{
			var go = GameObject.Instantiate(GameResources.Prefabs.Effects[(int)effect]);
			go.transform.position = new Vector3(pos.x, pos.y, -0.4f);
			go.transform.right = direction;
			GameObject.Destroy(go, delay);
		}
		public GameObject CreateEffect(E.Effects effect, Vector2 pos, float delay)
		{
			var go = GameObject.Instantiate(GameResources.Prefabs.Effects[(int)effect]);
			go.transform.position = new Vector3(pos.x, pos.y, -0.4f);
			GameObject.Destroy(go, delay);
			return go;
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
			for (int i = CallBacks.Count - 1; i > -1; i--)
			{
				var a = CallBacks[i];
				if (Time.time >= a.delay)
				{
					CallBacks.RemoveAt(i);
					a.action.Invoke();
				}
				a.delay += delta;
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
			GameResources.ScriptResources = _gameUnity.ResourceData;
			Animals = _gameUnity.Animals;
			LayerMasks = _gameUnity.LayerMasks;
			RealVariables = _gameUnity.RealTimeVariables;
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
		public GameObject GetCharacterByTeam(int team)
		{
			switch (team)
			{
				case 0:
					return GameResources.Prefabs.Yolanda;

				case 1:
					return GameResources.Prefabs.Schmillo;

			}
			return GameResources.Prefabs.Milton;
		}

	}
}
