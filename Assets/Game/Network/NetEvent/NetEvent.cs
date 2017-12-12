using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public abstract class NetEvent 
{
	public enum NetEventType
	{
		NetCreateItem,
		NetItemPickup,
		NetRopeEvent,
		NetDestroyItem,
		NetDestroyCube,
		NetCreateIngredient
	}

	public static Func<NetEvent>[] MakeEmpties = new Func<NetEvent>[]
	{
		NetCreateItem.Make,
		NetItemPickup.Make,
		NetEventRope.Make,
		NetDestroyItem.Make,
		NetDestroyCube.Make,
		NetCreateIngredient.Make
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

