﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class Input : GComponent
	{
		private static ObjectPool<Input> _pool = new ObjectPool<Input>(10);
		public Vector2 Axis;
		public bool Space;
		public bool RightClick;


		public Input()
		{

		}
		public static Input Make(int entityID)
		{
			Input comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}
