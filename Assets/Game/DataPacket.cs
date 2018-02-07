using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Game
{
	public abstract class DataPacket
	{
		public abstract void Recycle();
	}

	public class Logout : DataPacket
	{
		private static ObjectPool<Logout> _pool = new ObjectPool<Logout>(10);
		public int LeftID;
		public int CurrentHostID;

		public static Logout Make(int leftID, int currentHostID)
		{
			Logout pack = _pool.GetNext();
			pack.LeftID = leftID;
			pack.CurrentHostID = currentHostID;
			return pack;
		}
		public override void Recycle()
		{
			LeftID = -1;
			CurrentHostID = -1;
			_pool.Recycle(this);
		}
	}

	public class ChangeTeam : DataPacket
	{
		private static ObjectPool<ChangeTeam> _pool = new ObjectPool<ChangeTeam>(10);
		public int PlayerID;
		public int Team;

		public static ChangeTeam Make(int playerID, int team)
		{
			ChangeTeam pack = _pool.GetNext();
			pack.PlayerID = playerID;
			pack.Team = team;
			return pack;
		}
		public override void Recycle()
		{
			PlayerID = -1;
			Team = -1;
			_pool.Recycle(this);
		}
	}

	public class ChangeChar : DataPacket
	{
		private static ObjectPool<ChangeChar> _pool = new ObjectPool<ChangeChar>(10);
		public int PlayerID;
		public Characters Character;

		public static ChangeChar Make(int playerID, Characters character)
		{
			ChangeChar pack = _pool.GetNext();
			pack.PlayerID = playerID;
			pack.Character = character;
			return pack;
		}
		public override void Recycle()
		{
			PlayerID = -1;
			Character = Characters.Milton;
			_pool.Recycle(this);
		}
	}

}
