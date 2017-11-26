﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
	public class ResourcesComponent : GComponent
	{
		private static ObjectPool<ResourcesComponent> _pool = new ObjectPool<ResourcesComponent>(10);
		public GraphicRope GraphicRope;
		public Transform FreeArm;
		public string Character;

		public override void Recycle()
		{
			GraphicRope = null;
			Character = "";
			_pool.Recycle(this);
		}
		public ResourcesComponent()
		{

		}
		public static ResourcesComponent Make(int entityID)
		{
			ResourcesComponent comp = _pool.GetNext();
			comp.EntityID = entityID;
			return comp;
		}
	}
}