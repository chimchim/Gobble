using System.Collections;
using Game.Misc;
using UnityEngine;

namespace Game.Component
{
    public class GameObjects : GComponent
    {
		private static ObjectPool<GameObjects> _pool = new ObjectPool<GameObjects>(10);

        public GameObject Head;
        public GameObject Shooter;
		public GameObject Barrel;
        public GameObject Aim;
        public GameObject Collider;
		public GameObject Target;
        public string Enemy;
        public GameObjects()
        {

        }
		public static GameObjects Make(int entityID, GameObject head, GameObject shooter, GameObject barrel, GameObject aim, GameObject collider, GameObject target, string enemy)
        {
            GameObjects comp = _pool.GetNext();
            comp.Head = head;
            comp.Aim = aim;
			comp.Target = target;
            comp.Shooter = shooter;
			comp.Barrel = barrel;
            comp.Collider = collider;
            comp.Enemy = enemy;
            return comp;
        }
    }
}
