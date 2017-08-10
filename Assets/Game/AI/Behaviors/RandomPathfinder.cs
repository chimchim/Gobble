using System;
using System.Collections.Generic;
using System.Collections;
using Game;
using Game.Component;
using UnityEngine;

namespace Game.AI.Behaviors
{
    public class RandomPathfinder : AgentBehaviour 
    {
        public float Range =  20;

        public override void EnterState(GameManager game)
        {

        }

        public override void ExecuteState(GameManager game)
        {
            var pathMovement = game.Entities.GetComponentOf<PathMovement>(EntityID);
            var transform = game.Entities.GetComponentOf<GameObjects>(EntityID).Head.transform;

            Vector3 sampledTarget = Vector3.zero;
            if (RandomPoint(transform.position, Range, out sampledTarget) && !pathMovement.HasPath)
            {
                UnityEngine.AI.NavMeshPath toGoal = new UnityEngine.AI.NavMeshPath();
                UnityEngine.AI.NavMesh.CalculatePath(transform.position, sampledTarget, UnityEngine.AI.NavMesh.AllAreas, toGoal);
                pathMovement.NavPath = toGoal;
                pathMovement.HasPath = true;
            }
        }

        public override void ExitState(GameManager game)
        {

        }

        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 40; i++)
            {

                Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
                UnityEngine.AI.NavMeshHit hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}
