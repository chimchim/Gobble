using System.Collections;
using Game.Misc;
using UnityEngine;

namespace Game.Component
{
	public class Holster : GComponent
	{
		private static ObjectPool<Holster> _pool = new ObjectPool<Holster>(10);

		public Holster()
		{

		}
		public static Holster Make(int entityID)
		{
			Holster comp = _pool.GetNext();

			return comp;
		}
	}

	//public class Gun : GComponent
	//{
	//
	//	public Gun()
	//	{
	//
	//	}
	//	public static Gun Make(int entityID)
	//	{
	//		Gun comp = _pool.GetNext();
	//
	//		return comp;
	//	}
	//}

}
