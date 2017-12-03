using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class InventoryComponent : GComponent
	{
		private static ObjectPool<InventoryComponent> _pool = new ObjectPool<InventoryComponent>(10);
		public InventoryBackpack InventoryBackpack;
		public InventoryMain MainInventory;
		public override void Recycle()
		{

			_pool.Recycle(this);
		}
		public InventoryComponent()
		{

		}
		public static InventoryComponent Make(int entityID)
		{
			InventoryComponent comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
