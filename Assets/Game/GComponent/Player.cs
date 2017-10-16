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
		public int Team;
		public int LobbySlot;
		public Characters Character;
		public override void Recycle()
		{
			LobbySlot = -1;
			Team = 0;
			Owner = false;
			IsHost = false;
			PlayerName = "";
			Character = 0;
			_pool.Recycle(this);
		}
		public Player()
        {

        }
		public static Player MakeFromLobby(int entityID, bool owner, string name, bool isHost, int team, Characters character)
		{
			Player comp = _pool.GetNext();
			comp.EntityID = entityID;
			comp.Owner = owner;
			comp.PlayerName = name;
			comp.IsHost = isHost;
			comp.Team = team;
			comp.LobbySlot = -1;
			comp.Character = character;
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
