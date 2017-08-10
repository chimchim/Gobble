using System.Collections.Generic;


namespace Game.Component
{
    public class PathMovement : GComponent
    {
        private static ObjectPool<PathMovement> _pool = new ObjectPool<PathMovement>(100);
        public Pathfinder Pathfinder;
        public UnityEngine.AI.NavMeshPath NavPath = new UnityEngine.AI.NavMeshPath();
        public bool HasPath;

        public PathMovement()
        {

        }
        public static PathMovement Make(int entityID, Pathfinder pf)
        {
            PathMovement comp = _pool.GetNext();
            comp.EntityID = entityID;
            comp.Pathfinder = pf;
            comp.HasPath = false;
            return comp;
        }
    }
}
