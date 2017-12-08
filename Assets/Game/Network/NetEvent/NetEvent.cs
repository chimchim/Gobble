using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public abstract class BaseNetEvent<T> : NetEvent where T : BaseNetEvent<T>, new()
{
	private static readonly IObjectPool<T> _pool = new ObjectPool<T>(60);

	protected static T GetNext()
	{
		var ret = _pool.GetNext();
		return ret;
	}

	public override sealed void Recycle()
	{
		Reset();
		_pool.Recycle(this as T);
	}

	protected virtual void Reset()
	{
		NetEventID = 0;
		Iterations = 0;
	}

	protected override sealed void CloneFrom(NetEvent other)
	{
		InnerCloneFrom(other as T);
	}

	protected virtual void InnerCloneFrom(T other)
	{
		throw new NotImplementedException("We're not supposed to clone from a prototype with events in its buffer");
	}
}

public abstract class NetEvent 
{

	// Default Accessability is Aspect
	public int Iterations;
	public int NetEventID;
	public int ID { get { return GetID(GetType()); } }
	public abstract void Recycle();

	delegate NetEvent Maker();

	private static readonly object _lock = new object();

	private static Dictionary<Type, int> _ids;
	private static Dictionary<int, Maker> _makers;

	public bool Is<T>()
	{
		return this is T;
	}
	private static void BuildMetaData()
	{
		lock (_lock)
		{
			if (_ids == null)
			{
				_ids = new Dictionary<Type, int>();
				_makers = new Dictionary<int, Maker>();
				List<Type> types = new List<Type>(
					Assembly.GetAssembly(typeof(NetEvent)).GetTypes().Where
						(t => t.IsSubclassOf(typeof(NetEvent)) && !t.IsAbstract).OrderBy
						(t => t.Name));
				for (int i = 0; i < types.Count; ++i)
				{
					_ids.Add(types[i], i);
					var methodInfo = types[i].GetMethod("GetNext", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
					_makers.Add(i, (Maker)Delegate.CreateDelegate(typeof(Maker), methodInfo));
				}
			}
		}
	}

	private static int GetID(Type type)
	{
		BuildMetaData();

		return _ids[type];
	}

	public static NetEvent MakeFromIncoming(Game.GameManager game, int id, byte[] incoming, int index)
	{
		BuildMetaData();

		try
		{
			var ret = _makers[id]();
			ret.NetDeserialize(game, incoming, index);
			return ret;
		}
		catch (KeyNotFoundException e)
		{
			Debug.Log("No such AbilityAspectEvent ID: ");
			throw;
		}
	}

	public NetEvent Clone()
	{
		BuildMetaData();

		var ret = _makers[ID]();
		ret.CloneFrom(this);

		return ret;
	}

	public abstract void Handle(Game.GameManager game);
	protected abstract void CloneFrom(NetEvent other);


	public void NetSerialize(object gameState, List<byte> outgoing)
	{
		outgoing.AddRange(BitConverter.GetBytes(ID));
		InnerNetSerialize((Game.GameManager)gameState, outgoing);
	}

	public void NetDeserialize(object gameState, byte[] byteData, int index)
	{
		InnerNetDeserialize((Game.GameManager)gameState, byteData, index);
	}

	protected abstract void InnerNetDeserialize(Game.GameManager game, byte[] byteData, int index);

	protected abstract void InnerNetSerialize(Game.GameManager game, List<byte> outgoing);


}

