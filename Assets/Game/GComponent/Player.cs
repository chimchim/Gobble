using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class Player : GComponent
    {
        private static ObjectPool<Player> _pool = new ObjectPool<Player>(100);

        public Player()
        {

        }
        public static Player Make(int entityID)
        {
            Player comp = _pool.GetNext();
            comp.EntityID = entityID;
            return comp;
        }
    }
}
