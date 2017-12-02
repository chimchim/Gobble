using System.Collections.Generic;
using UnityEngine;
namespace Game.Component
{
	public class ItemHolder : GComponent
	{
		public static int ActiveItemsCount = 1;
		private static ObjectPool<ItemHolder> _pool = new ObjectPool<ItemHolder>(10);
		public List<Item> Items = new List<Item>();
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
			comp.Items.Add(ItemCreator.Make());
			comp.Items.Add(Rope.Make());
			comp.Items.Add(PickAxe.Make());
			return comp;
		}
	}
}
