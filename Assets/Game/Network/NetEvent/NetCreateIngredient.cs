using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class NetCreateIngredient : NetEvent
{
	private static ObjectPool<NetCreateIngredient> _pool = new ObjectPool<NetCreateIngredient>(10);

	public TileMap.IngredientType IngredientType;
	public Vector2 Force;
	public Vector2 Position;
	public int Creator;
	public int Quantity;
	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		int itemNetID = (Creator * 200000) + NetEventID;
		VisibleItem visible = null;

		visible = Ingredient.MakeItem(game, Position, Force, IngredientType);
		visible.Item.Quantity = Quantity;
		visible.StartCoroutine(visible.TriggerTime());
		visible.Item.ItemNetID = itemNetID;
		visible.Item.CurrentGameObject = visible.gameObject;
		game.WorldItems.Add(visible);
	}

	public static NetCreateIngredient Make()
	{
		return _pool.GetNext();
	}

	public static NetCreateIngredient Make(int creator, int q, TileMap.IngredientType IngredientType, Vector3 position, Vector2 force)
	{
		var evt = _pool.GetNext();
		evt.IngredientType = IngredientType;
		evt.Force = force;
		evt.Position = position;
		evt.Creator = creator;
		evt.Quantity = q;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		Creator = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		float posX = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float posY = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float forceX = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		float forceY = BitConverter.ToSingle(byteData, index);
		index += sizeof(float);
		int ingredient = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);
		int quantity = BitConverter.ToInt32(byteData, index);
		index += sizeof(int);

		IngredientType = (TileMap.IngredientType)ingredient;
		Position = new Vector2(posX, posY);
		Force = new Vector2(forceX, forceY);
		Quantity = quantity;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetCreateIngredient));
		outgoing.AddRange(BitConverter.GetBytes(28));
		outgoing.AddRange(BitConverter.GetBytes(Creator));
		outgoing.AddRange(BitConverter.GetBytes(Position.x));
		outgoing.AddRange(BitConverter.GetBytes(Position.y));
		outgoing.AddRange(BitConverter.GetBytes(Force.x));
		outgoing.AddRange(BitConverter.GetBytes(Force.y));
		outgoing.AddRange(BitConverter.GetBytes((int)IngredientType));
		outgoing.AddRange(BitConverter.GetBytes(Quantity));
	}
}
