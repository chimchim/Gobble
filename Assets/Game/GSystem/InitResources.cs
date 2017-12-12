using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class InitResources : ISystem
	{
		// gör en input translator?

		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player>();

		public void Update(GameManager game, float delta)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
				{
					
				}
			}
		}


		public void Initiate(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			int players = 0;
			foreach (int entity in entities)
			{
				var ent = game.Entities.GetEntity(entity);
				ent.AddComponent(ActionQueue.Make(ent.ID));
				ent.AddComponent(MovementComponent.Make(ent.ID));
				ent.AddComponent(Stats.Make(ent.ID, 100, GameUnity.OxygenTime, GameUnity.OxygenTime));
				ent.AddComponent(InputComponent.Make(ent.ID));
				ent.AddComponent(NetEventComponent.Make(ent.ID));
				var inventory = InventoryComponent.Make(ent.ID);
				ent.AddComponent(inventory);
				var player = game.Entities.GetComponentOf<Player>(entity);
				var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);

				var playerGameObject = GameObject.Instantiate(game.GetCharacterObject(player.Character), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
				playerGameObject.tag = "Player";
				ent.gameObject = playerGameObject;
				ent.Animator = playerGameObject.GetComponentInChildren<Animator>();
				playerGameObject.AddComponent<IdHolder>().ID = ent.ID;
				playerGameObject.GetComponent<IdHolder>().Owner = player.Owner;
				playerGameObject.transform.position = new Vector3((GameUnity.FullWidth / 2), (GameUnity.FullHeight / 2), 0);
				if (player.Owner)
				{
					game.SetMainPlayer(playerGameObject, inventory);
				}

				GameObject Ropes = new GameObject();
				Ropes.AddComponent<GraphicRope>().Owner = player.Owner;
				Ropes.GetComponent<GraphicRope>().MakeRopes();
				resources.GraphicRope = Ropes.GetComponent<GraphicRope>();
				resources.FreeArm = playerGameObject.transform.Find("free_arm");
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
					hands.OwnerActivate(game, itemHolder.EntityID);
				else
					hands.ClientActivate(game, itemHolder.EntityID);


			}
		}

		private GameObject GetCharacterObject(GameManager game, Characters character)
		{
			switch (character)
			{
				case Characters.Milton:
					return game.GameResources.Prefabs.Milton;

				case Characters.Peppermin:
					return game.GameResources.Prefabs.Peppermin;

				case Characters.Yolanda:
					return game.GameResources.Prefabs.Yolanda;

				case Characters.Schmillo:
					return game.GameResources.Prefabs.Schmillo;

			}
			return game.GameResources.Prefabs.Milton;
        }
		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}
	}
}
