using System;
using System.Collections.Generic;

namespace Game.Misc
{
	public class IDGiver
	{
		private static List<int> freeIDs = new List<int>();
		private static int nextID = 0;

		public static int GetNewID()
		{
			if (freeIDs.Count != 0)
			{
				int ret = freeIDs[0];
				freeIDs.RemoveAt(0);
				return ret;
			}
			else
			{
				return nextID++;
			}
		}

		public static void FreeID(int id)
		{
			freeIDs.Add(id);
		}
	}
}

