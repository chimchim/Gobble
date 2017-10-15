using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Component
{
	// lägg till entiterna i de representerade system när komponenterna skapas
	/* smygar spel där man försöker lista ut varandras position i mörker ( inte så snabb kanske) lysen där man kan vakta. low precision när man träffar från
	 * lång håll,
     * olika hastigheter när man har vissa vapen

	 Turnaround time på vapen?*/
	public abstract class GComponent
	{
		protected GComponent()
		{
            Disabled = false;
        }

		public abstract void Recycle();

		public int EntityID;
        public bool Disabled;
        private static readonly Dictionary<Type, int> _ids = new Dictionary<Type, int>();

        public static int GetID<T>() where T : GComponent
        {
            Type t = typeof(T);

                int id;
                if (_ids.TryGetValue(t, out id))
                {
                    return id;
                }
            
            return -1;
        }
        public static void AddID(int id, Type t)
        {
            _ids.Add(t, id);
        }
	}
}
