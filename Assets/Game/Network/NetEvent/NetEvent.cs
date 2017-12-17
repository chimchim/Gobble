using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Gatherables;

public abstract class NetEvent 
{
	public enum NetEventType
	{
		NetCreateItem,
		NetItemPickup,
		NetRopeEvent,
		NetDestroyItem,
		NetDestroyCube,
		NetCreateIngredient,
		NetDestroyWorldItem,
		NetJump,
		NetDestroyCustom,
		NetIngredientFromGatherable,
		NetCreateLadder
	}

	public static NetEvent GetGatherableEvent(Gatherable gatherable)
	{
		if (gatherable.GetType() == typeof(GatherableBlock))
		{
			GatherableBlock block = gatherable as GatherableBlock;
			return NetDestroyCube.Make(block.X, block.Y);
		}
		if (gatherable.GetType() == typeof(GatherableCustom))
		{
			GatherableCustom custom = gatherable as GatherableCustom;
			return NetDestroyCustom.Make(custom.CustomIndex);
		}
		return null;
	}

	public static Func<NetEvent>[] MakeEmpties = new Func<NetEvent>[]
	{
		NetCreateItem.Make,
		NetItemPickup.Make,
		NetEventRope.Make,
		NetDestroyItem.Make,
		NetDestroyCube.Make,
		NetCreateIngredient.Make,
		NetDestroyWorldItem.Make,
		NetJump.Make,
		NetDestroyCustom.Make,
		NetIngredientFromGatherable.Make,
		NetCreateLadder.Make
	};

	// Default Accessability is Aspect
	public int Iterations;
	public int NetEventID;

	public abstract void Recycle();
	public abstract void Handle(Game.GameManager game);


	public void NetSerialize(object gameState, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes(NetEventID));
		InnerNetSerialize((Game.GameManager)gameState, outgoing);
	}

	public void NetDeserialize(object gameState, byte[] byteData, int index)
	{
		InnerNetDeserialize((Game.GameManager)gameState, byteData, index);
	}

	protected abstract void InnerNetDeserialize(Game.GameManager game, byte[] byteData, int index);

	protected abstract void InnerNetSerialize(Game.GameManager game, List<byte> outgoing);


}

