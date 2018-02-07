using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;
using Game.Movement;
using MoveState = Game.Component.MovementComponent.MoveState;

namespace Game.Systems
{
	public class InitResources : ISystem
	{
		// gör en input translator?

		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player>();

		public void Update(GameManager game, float delta)
		{
		}


		public void Initiate(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			int players = 0;
			int ownerTeam = 0;
			foreach (int entity in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
					ownerTeam = player.Team;
			}
			foreach (int entity in entities)
			{
				var ent = game.Entities.GetEntity(entity);
				ent.AddComponent(ActionQueue.Make(ent.ID));
				ent.AddComponent(Stats.Make(ent.ID, 100, GameUnity.OxygenTime, GameUnity.OxygenTime));
				ent.AddComponent(InputComponent.Make(ent.ID));
				ent.AddComponent(NetEventComponent.Make(ent.ID));
				var inventory = InventoryComponent.Make(ent.ID);
				ent.AddComponent(inventory);
				var player = game.Entities.GetComponentOf<Player>(entity);
				var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
				var movecomp = MovementComponent.Make(ent.ID);
				ent.AddComponent(movecomp);
				#region SetEnemy
				player.Enemy = (player.Team != ownerTeam);
				((Grounded)movecomp.States[(int)MoveState.Grounded]).PlayerLayer = !player.Enemy ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("PlayerEnemy");
				((Grounded)movecomp.States[(int)MoveState.Grounded]).PlayerPlatformLayer = !player.Enemy ? LayerMask.NameToLayer("PlayerPlatform") : LayerMask.NameToLayer("PlayerEnemyPlatform");
				var playerGameObject = GameObject.Instantiate(game.GetCharacterByTeam(player.Team), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
				playerGameObject.tag = "Player";
				playerGameObject.layer = !player.Enemy ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("PlayerEnemy"); 
				#endregion

				ent.gameObject = playerGameObject;

				ent.Animator = playerGameObject.GetComponentInChildren<Animator>();
				playerGameObject.AddComponent<IdHolder>().ID = ent.ID;
				playerGameObject.GetComponent<IdHolder>().Owner = player.Owner;
				playerGameObject.transform.position =  new Vector3((GameUnity.FullWidth / 2), (GameUnity.FullHeight / 2), 0);
				game.Entities.GetComponentOf<InputComponent>(ent.ID).NetworkPosition = new Vector3((GameUnity.FullWidth / 2), (GameUnity.FullHeight / 2), 0);
				if (player.Owner)
				{
					game.SetMainPlayer(playerGameObject, inventory);
				}

				GameObject Ropes = new GameObject();
				Ropes.AddComponent<GraphicRope>().Owner = player.Owner;
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
	}
}
