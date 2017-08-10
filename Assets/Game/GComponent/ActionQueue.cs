using System.Collections.Generic;
using Game.Actions;

namespace Game.Component
{
    public class ActionQueue : GComponent
    {
		private static ObjectPool<ActionQueue> _pool = new ObjectPool<ActionQueue>(100);

        public List<Action> Actions = new List<Action>();
        public ActionQueue()
        {

        }
        public static ActionQueue Make(int entityID)
        {
			ActionQueue comp = _pool.GetNext();
            comp.EntityID = entityID;
            return comp;
        }
    }
}