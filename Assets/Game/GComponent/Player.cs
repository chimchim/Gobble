using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class Player : GComponent
    {
        private static ObjectPool<Player> _pool = new ObjectPool<Player>(10);
		public bool Owner;
		public bool IsHost;
		public string PlayerName;

		public Player()
        {

        }
		public static Player Make(int entityID, bool owner, string name, bool isHost)
		{
			Player comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.Owner = owner;
			comp.PlayerName = name;
			comp.IsHost = isHost;
			return comp;
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
