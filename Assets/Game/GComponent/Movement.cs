using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class Movement : GComponent
    {
        private static ObjectPool<Movement> _pool = new ObjectPool<Movement>(100);
        public bool Falling;
        public bool Grounded;
        public Vector2 Input;
        public Vector3 Movedirection;
        public float jumpForce;
        public Movement()
        {
            
        }
        public static Movement Make(int entityID)
        {
            Movement comp = _pool.GetNext();
            comp.EntityID = entityID;
            return comp;
        }
    }
}
