using System;
using System.Collections.Generic;
using Game.Actions;

namespace Game.Component
{
    public class ActionQueue : GComponent
    {
		private static ObjectPool<ActionQueue> _pool = new ObjectPool<ActionQueue>(10);

        public List<Game.Actions.Action> Actions = new List<Game.Actions.Action>();
		public override void Recycle()
		{
			Actions.Clear();
			_pool.Recycle(this);
		}
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