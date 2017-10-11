using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class MenuComponent : GComponent
	{
		private static ObjectPool<MenuComponent> _pool = new ObjectPool<MenuComponent>(10);
		public MenuGUI Menu;
		public int PlayerAmount;

		public MenuComponent()
		{

		}
		public static MenuComponent Make(int entityID)
		{
			MenuComponent comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
