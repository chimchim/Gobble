using System.Collections.Generic;
using Game.Actions;
using Game.AI;
namespace Game.Component
{
    public class Agent : GComponent
    {
        private static ObjectPool<Agent> _pool = new ObjectPool<Agent>(100);
        public AIStateMachine StateMachine;
        public Agent()
        {

        }
        public static Agent Make(int entityID, AIStateMachine ai)
        {
            Agent comp = _pool.GetNext();
            comp.StateMachine = ai;
            comp.EntityID = entityID;
            return comp;
        }
    }
}