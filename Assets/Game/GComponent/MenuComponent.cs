﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class MenuComponent : GComponent
	{
		private static ObjectPool<MenuComponent> _pool = new ObjectPool<MenuComponent>(10);
		public MenuGUI Menu;
		public bool IsHost;
		public int PlayerAmount;

		public Action<GameManager, MenuComponent, byte[]>[] ActionArray;
		public override void Recycle()
		{
			Menu = null;
			IsHost = false;
			PlayerAmount = 0;
			_pool.Recycle(this);
		}
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
