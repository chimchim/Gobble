using Game;
using Game.Component;
using Game.GEntity;
using Game.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Rope : Item
{
	public struct NetworkRopeConnected
	{
		public Vector2 RayCastOrigin;
		public Vector2 Position;
		public Vector2 Origin;
		public float Length;
	}

	public bool SemiActive;
	private static ObjectPool<Rope> _pool = new ObjectPool<Rope>(10);
	public override void Recycle()
	{
		SemiActive = false;
		_pool.Recycle(this);
	}

	public Rope()
	{

	}
	public static Rope Make()
	{
		Rope item = _pool.GetNext();
		item.ID = ItemID.Rope;
		return item;
	}

	public static VisibleItem MakeItem(GameManager game, Vector3 position, Vector2 force)
	{
		var go = GameObject.Instantiate(game.GameResources.AllItems.Rope.Prefab);
		go.transform.position = position;

		var visible = go.AddComponent<VisibleItem>();
		var item = Make();
		visible.Item = item;
		visible.CallBack = (EntityID) =>
		{
			var player = game.Entities.GetComponentOf<Player>(EntityID);
			if (player.Owner)
			{
				HandleNetEventSystem.AddEventIgnoreOwner(game, EntityID, NetItemPickup.Make(EntityID, item.ItemNetID));
				item.OnPickup(game, EntityID, go);
			}
		};
		visible.Force = force;
		return visible;
	}
	public override void OnPickup(GameManager game, int entity, GameObject gameObject)
	{
		CheckMain(game, entity, game.GameResources.AllItems.Rope, gameObject);
	}
	public override void OwnerActivate(GameManager game, int entity)
	{
		SemiActive = false;
		base.OwnerActivate(game, entity);
	}
	public override void OwnerDeActivate(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		if (resources.GraphicRope.RopeItem != this)
		{
			var itemHolder = game.Entities.GetComponentOf<ItemHolder>(entity);
			itemHolder.ActiveItems.Remove(this);
		}
		else
		{
			SemiActive = true;
		}
		CurrentGameObject.SetActive(false);
	}
	public override void ClientDeActivate(GameManager game, int entity)
	{
		var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
		if (movement.CurrentState == MovementComponent.MoveState.Roped)
		{
			var input = game.Entities.GetComponentOf<InputComponent>(entity);
			var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
			input.RightClick = false;
			resources.GraphicRope.DeActivate();
			movement.RopeList.Clear();
			movement.CurrentState = MovementComponent.MoveState.Grounded;
		}
		base.ClientDeActivate(game, entity);
	}
	public override void ThrowItem(GameManager game, int entity)
	{
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		if (resources.GraphicRope.RopeItem == this)
		{
			return;
		}

		base.ThrowItem(game, entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);

		var ent = game.Entities.GetEntity(entity);
		var position = ent.gameObject.transform.position;
		var force = input.ScreenDirection * 5 + ent.PlayerSpeed;

		HandleNetEventSystem.AddEvent(game, entity, NetCreateItem.Make(entity, Item.ItemID.Rope, position, force));
	}

	public override void Input(GameManager game, int entity)
	{

		var player = game.Entities.GetComponentOf<Player>(entity);
		if (player.Owner)
		{
			var input = game.Entities.GetComponentOf<InputComponent>(entity);
			var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
			var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
			if (input.RightClick && movement.CurrentState != MovementComponent.MoveState.Roped)
			{
				HandleNetEventSystem.AddEventAndHandle(game, entity, NetEventRope.Make(entity, ItemNetID, true));
				resources.GraphicRope.RopeItem = this;
			}
			else if (input.RightClick && movement.CurrentState == MovementComponent.MoveState.Roped && !SemiActive)
			{
				if (resources.GraphicRope.RopeItem != null && resources.GraphicRope.RopeItem != this)
				{
					resources.GraphicRope.RopeItem.Remove = true;
				}
				HandleNetEventSystem.AddEventAndHandle(game, entity, NetEventRope.Make(entity, ItemNetID, false));
			}
		}
	}

	public override void Sync(GameManager game, Client.GameLogicPacket pack, byte[] byteData, ref int currentIndex)
	{

		int entity = pack.PlayerID;
		var player = game.Entities.GetComponentOf<Player>(entity);
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);

		var movestate = (MovementComponent.MoveState)pack.MovementState;
		var otherEntity = game.Entities.GetEntity(entity);
		var otherTransform = otherEntity.gameObject.transform;
		bool semiActive = BitConverter.ToBoolean(byteData, currentIndex); currentIndex += sizeof(bool);
		if (semiActive && !CurrentGameObject.activeSelf)
		{
			CurrentGameObject.SetActive(true);
		}
		else if (!semiActive && CurrentGameObject.activeSelf)
		{
			CurrentGameObject.SetActive(false);
		}
		bool ropeConnected = BitConverter.ToBoolean(byteData, currentIndex); currentIndex += sizeof(bool);
		if (ropeConnected)
		{
			var ropeConnect = GetRopeConnected(byteData, ref currentIndex);
			movement.CurrentState = MovementComponent.MoveState.Roped;
			otherTransform.position = pack.Position;
			movement.CurrentRoped = new MovementComponent.RopedData()
			{
				RayCastOrigin = ropeConnect.RayCastOrigin,
				origin = ropeConnect.Origin,
				Length = ropeConnect.Length,
				Damp = GameUnity.RopeDamping
			};
			movement.RopeList.Add(movement.CurrentRoped);
		}
		
		if (movement.CurrentState == MovementComponent.MoveState.Roped && pack.InputSpace)
		{
			
			float ropeAngle = BitConverter.ToSingle(byteData, currentIndex);
			var playerPosX = movement.CurrentRoped.origin.x + (-movement.CurrentRoped.Length * Mathf.Sin(ropeAngle));
			var playerPosY = movement.CurrentRoped.origin.y + (-movement.CurrentRoped.Length * Mathf.Cos(ropeAngle));
			otherTransform.position = pack.Position;
			Game.Movement.Roped.ReleaseRope(resources, movement, new Vector2(playerPosX, playerPosY), pack.Position);
			GetRopes(byteData, ref currentIndex, movement);

		}
		else if (movement.CurrentState == MovementComponent.MoveState.Roped && movestate != MovementComponent.MoveState.Roped)
		{
			resources.GraphicRope.DeActivate();
			movement.RopeList.Clear();
			movement.CurrentState = MovementComponent.MoveState.Grounded;
		}
	
		RopeSync(pack, otherEntity, movement, byteData, ref currentIndex);
		
	}
	private void RopeSync(Client.GameLogicPacket packet, Entity entity, MovementComponent movement, byte[] byteData, ref int currentIndex)
	{
		Vector2 diff = packet.Position - new Vector2(entity.gameObject.transform.position.x, entity.gameObject.transform.position.y);
		if (movement.CurrentState == MovementComponent.MoveState.Roped)
		{
			var ropeList = GetRopes(byteData, ref currentIndex, movement);
			if (movement.CurrentState == MovementComponent.MoveState.Roped && diff.magnitude > 2)
			{
				entity.gameObject.transform.position = packet.Position;

				movement.RopeList.Clear();
				for (int i = 0; i < ropeList.Length; i++)
				{
					var rope = ropeList[i];
					movement.CurrentRoped = rope;
					movement.RopeList.Add(movement.CurrentRoped);
				}
			}
		}
	}

	public MovementComponent.RopedData[] GetRopes (byte[] byteData, ref int currentByteIndex, MovementComponent movement)
	{
		float ropeAngle = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		float ropeVel = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		int ropeCount = BitConverter.ToInt32(byteData, currentByteIndex);
		currentByteIndex += sizeof(int);

		MovementComponent.RopedData[] roped = new MovementComponent.RopedData[ropeCount];
		for (int i = 0; i < ropeCount; i++)
		{
			float vel = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float angle = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float originx = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float originy = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float rayCastOriginx = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float rayCastOriginy = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float rayCastCollideOldPosx = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float rayCastCollideOldPosy = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float oldRopeCollidePosx = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float oldRopeCollidePosy = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			bool newRopeIsLeft = BitConverter.ToBoolean(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			float length = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			bool firstAngle = BitConverter.ToBoolean(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			float damp = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			roped[i] = new MovementComponent.RopedData()
			{
				Vel = vel,
				Angle = angle,
				origin = new Vector2(originx, originy),
				RayCastOrigin = new Vector2(rayCastOriginx, rayCastOriginy),
				RayCastCollideOldPos = new Vector2(rayCastCollideOldPosx, rayCastCollideOldPosy),
				OldRopeCollidePos = new Vector2(oldRopeCollidePosx, oldRopeCollidePosy),
				NewRopeIsLeft = newRopeIsLeft,
				Length = length,
				FirstAngle = firstAngle,
				Damp = damp
			};
		}
		return roped;
	}

	public NetworkRopeConnected GetRopeConnected(byte[] byteData, ref int currentByteIndex)
	{
		float rayCastOriginX = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		float rayCastOriginY = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		float originX = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		float originY = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		float positionX = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		float positionY = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		float l = BitConverter.ToSingle(byteData, currentByteIndex);
		currentByteIndex += sizeof(float);
		var ropeConnect = new NetworkRopeConnected
		{
			RayCastOrigin = new Vector2(rayCastOriginX, rayCastOriginY),
			Origin = new Vector2(originX, originY),
			Position = new Vector2(positionX, positionY),
			Length = l,
		};
		return ropeConnect;
	}
	public override void Serialize(GameManager game, int entity, List<byte> byteArray)
	{
		var input = game.Entities.GetComponentOf<InputComponent>(entity);
		var movement = game.Entities.GetComponentOf<MovementComponent>(entity);
		var resources = game.Entities.GetComponentOf<ResourcesComponent>(entity);
		var entityTransform = game.Entities.GetEntity(entity).gameObject.transform;

		byteArray.AddRange(BitConverter.GetBytes(ItemNetID));
		byteArray.AddRange(BitConverter.GetBytes(CurrentGameObject.activeSelf));
		bool ropeConnected = input.RopeConnected.Length > 0;
		byteArray.AddRange(BitConverter.GetBytes(ropeConnected));
		if (ropeConnected)
		{
			CreateRopeConnected(byteArray, input.RopeConnected);
			input.RopeConnected.Length = 0;
		}
		if (movement.CurrentState == MovementComponent.MoveState.Roped)
		{
			SerializeRope(byteArray, movement);
		}
	}

	private void CreateRopeConnected(List<byte> currentByteArray, InputComponent.NetworkRopeConnected ropeConnected)
	{
		currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.RayCastOrigin.x));
		currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.RayCastOrigin.y));
		currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Origin.x));
		currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Origin.y));
		currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Position.x));
		currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Position.y));
		currentByteArray.AddRange(BitConverter.GetBytes(ropeConnected.Length));
	}

	private void SerializeRope(List<byte> currentByteArray, MovementComponent movement)
	{
		currentByteArray.AddRange(BitConverter.GetBytes(movement.CurrentRoped.Angle));
		currentByteArray.AddRange(BitConverter.GetBytes(movement.CurrentRoped.Vel));
		currentByteArray.AddRange(BitConverter.GetBytes(movement.RopeList.Count));
		for (int i = 0; i < movement.RopeList.Count; i++)
		{
			var ropeData = movement.RopeList[i];
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Vel));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Angle));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.origin.x));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.origin.y));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastOrigin.x));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastOrigin.y));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastCollideOldPos.x));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.RayCastCollideOldPos.y));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.OldRopeCollidePos.x));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.OldRopeCollidePos.y));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.NewRopeIsLeft));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Length));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.FirstAngle));
			currentByteArray.AddRange(BitConverter.GetBytes(ropeData.Damp));
		}
	}
}

