using System.Collections;
using Game.Misc;
using UnityEngine;

namespace Game.Component
{
    public class AIGameObjects : GComponent
    {
        private static ObjectPool<AIGameObjects> _pool = new ObjectPool<AIGameObjects>(100);

        public GameObject Head;
        public GameObject Shooter;
        public GameObject Collider;


        public AIGameObjects()
        {

        }
        public static AIGameObjects Make(int entityID, GameObject head, GameObject shooter, GameObject collider)
        {
            AIGameObjects comp = _pool.GetNext();
            comp.Head = head;
            comp.Shooter = shooter;
            comp.Collider = collider;
            return comp;
        }
    }
}
