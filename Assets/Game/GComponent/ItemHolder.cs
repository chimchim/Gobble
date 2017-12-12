using System.Collections.Generic;
using UnityEngine;
namespace Game.Component
{
	public class ItemHolder : GComponent
	{
		public static int ActiveItemsCount = 1;
		private static ObjectPool<ItemHolder> _pool = new ObjectPool<ItemHolder>(10);
		public Dictionary<int,Item> Items = new Dictionary<int, Item>();
		public List<Item> ActiveItems = new List<Item>();
		public EmptyHands Hands;
		public override void Recycle()
		{
			_pool.Recycle(this);
		}

		public ItemHolder()
		{

		}
		public static ItemHolder Make(int entityID)
		{
			ItemHolder comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
