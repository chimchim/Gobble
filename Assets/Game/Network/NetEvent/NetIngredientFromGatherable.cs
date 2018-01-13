using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class NetIngredientFromGatherable : NetEvent
{
	private static ObjectPool<NetIngredientFromGatherable> _pool = new ObjectPool<NetIngredientFromGatherable>(10);

	public TileMap.IngredientType IngredientType;
	public Vector2 Force;
	public int Creator;
	public int Quantity;
	public int CustomIndex;
	public override void Recycle()
	{
		Iterations = 0;
		_pool.Recycle(this);
	}
	public override void Handle(GameManager game)
	{
		if (!game.TileMap.CustomGatherables.ContainsKey(CustomIndex))
		{
			Debug.LogError("CustomIndex DE NULL " + CustomIndex);
			return;
		}
		var custom = game.TileMap.CustomGatherables[CustomIndex];
		game.TileMap.CustomGatherables.Remove(CustomIndex);
		int itemNetID = (Creator * 200000) + NetEventID;
		VisibleItem visible = null;

		visible = Ingredient.MakeFromGatherable(game, custom, Force, IngredientType, Creator);
		visible.Item.Quantity = Quantity;
		visible.StartCoroutine(visible.TriggerTime());
		visible.Item.ItemNetID = itemNetID;
		visible.Item.CurrentGameObject = visible.gameObject;
		game.WorldItems.Add(visible);
	}

	public static NetIngredientFromGatherable Make()
	{
		return _pool.GetNext();
	}

	public static NetIngredientFromGatherable Make(int creator, int q, int customIndex, TileMap.IngredientType IngredientType, Vector2 force)
	{
		var evt = _pool.GetNext();
		evt.IngredientType = IngredientType;
		evt.Force = force;
		evt.Creator = creator;
		evt.Quantity = q;
		evt.CustomIndex = customIndex;
		return evt;
	}


	protected override void InnerNetDeserialize(GameManager game, byte[] byteData, int index)
	{
		Creator = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		float forceX = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		float forceY = BitConverter.ToSingle(byteData, index); index += sizeof(float);
		int ingredient = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		int quantity = BitConverter.ToInt32(byteData, index); index += sizeof(int);
		int customIndex = BitConverter.ToInt32(byteData, index); index += sizeof(int);

		IngredientType = (TileMap.IngredientType)ingredient;
		Force = new Vector2(forceX, forceY);
		Quantity = quantity;
		CustomIndex = customIndex;
	}

	protected override void InnerNetSerialize(GameManager game, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes((int)NetEventType.NetIngredientFromGatherable));
		outgoing.AddRange(BitConverter.GetBytes(24));
		outgoing.AddRange(BitConverter.GetBytes(Creator));
		outgoing.AddRange(BitConverter.GetBytes(Force.x));
		outgoing.AddRange(BitConverter.GetBytes(Force.y));
		outgoing.AddRange(BitConverter.GetBytes((int)IngredientType));
		outgoing.AddRange(BitConverter.GetBytes(Quantity));
		outgoing.AddRange(BitConverter.GetBytes(CustomIndex));
	}
}
