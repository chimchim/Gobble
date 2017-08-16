using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class Player : GComponent
    {
        private static ObjectPool<Player> _pool = new ObjectPool<Player>(100);
		public bool Owner;
        public Player()
        {

        }
        public static Player Make(int entityID, bool owner)
        {
            Player comp = _pool.GetNext();
            comp.EntityID = entityID;
			comp.Owner = owner;

			return comp;
        }
    }
}
