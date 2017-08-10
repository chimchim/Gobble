using System;
using System.Collections.Generic;
using System.Collections;
using Game;
using Game.Component;
using UnityEngine;
using Game.Actions;
namespace Game.AI.Behaviors
{
    public class FollowWayPoints : AgentBehaviour
    {
        public float Speed = 1f;

        //private Vector3 targetPosition;
        private int currentIndex = 0;
        public override void EnterState(GameManager game)
        {
            currentIndex = 1;
        }

        public override void ExecuteState(GameManager game)
        {
            var pathMovement = game.Entities.GetComponentOf<PathMovement>(EntityID);
            var aq = game.Entities.GetComponentOf<ActionQueue>(EntityID);
            var gameObjects = game.Entities.GetComponentOf<GameObjects>(EntityID);
            var head = gameObjects.Head.transform;
            var move = game.Entities.GetComponentOf<Movement>(EntityID);

            if (pathMovement.HasPath)
            {
                Vector2 currentPosition = ToVector2(head.position);
                var wayPoints = pathMovement.NavPath;
                float moveAmount = Speed * Time.deltaTime;
                while (true)
                {
                    
                    Vector2 targetPosition = ToVector2(wayPoints.corners[currentIndex]);
                    float currentLength = (targetPosition - currentPosition).magnitude;
                    moveAmount = Math.Min(moveAmount, currentLength);
                    Vector2 translatePosition = (targetPosition - currentPosition).normalized * moveAmount;
                    Vector3 shooterEuler = gameObjects.Shooter.transform.eulerAngles;
                    //Debug.Log("gameObjects.Shooter.transform.position " + gameObjects.Shooter.transform.position + " " + translatePosition.x);
                    if (translatePosition.x < 0)
                    {
                        
                        gameObjects.Target.transform.position = gameObjects.Shooter.transform.position + new Vector3(translatePosition.x * 500, 0, 0);
                        head.eulerAngles = new Vector3(0, 270, 0);
                    }
                    else
                    {
                        gameObjects.Target.transform.position = gameObjects.Shooter.transform.position + new Vector3(translatePosition.x * 500, 0, 0);
                        //gameObjects.Shooter.transform.eulerAngles = new Vector3(0, shooterEuler.y, 0);
                        head.eulerAngles = new Vector3(0, 90, 0);
                    }

                    if (currentLength > moveAmount)
                    {
                        aq.Actions.Add(MoveAction.Make(translatePosition));
                        break;
                    }
            
                    moveAmount = (Speed * Time.deltaTime) - currentLength;
                    aq.Actions.Add(MoveAction.Make(translatePosition));
                    currentPosition = ToVector2(wayPoints.corners[currentIndex]);
                    currentIndex++;
                    if (currentIndex == wayPoints.corners.Length)
                    {
                        currentIndex = 1;
                        pathMovement.HasPath = false;
                        break;
                    }
                }
                Vector2 GunForward = new Vector2(gameObjects.Aim.transform.forward.x, gameObjects.Aim.transform.forward.y);
                Vector3 targetTemp = gameObjects.Target.transform.position;
                Vector3 Target = new Vector2(
                    (targetTemp - gameObjects.Aim.transform.position).x,
                    (targetTemp - gameObjects.Aim.transform.position).y);

                float angle = (180 - Vector2.Angle(GunForward, Target));
                Vector3 cross = Vector3.Cross(new Vector3(GunForward.x, GunForward.y, 0), new Vector3(Target.x, Target.y, 0));
                if (cross.z > 0)
                {
                    angle = -angle;
                }
                gameObjects.Shooter.transform.Rotate(Vector3.forward, angle, Space.World);
            }
           for (int i = 0; i < pathMovement.NavPath.corners.Length - 1; i++)
               Debug.DrawLine(pathMovement.NavPath.corners[i], pathMovement.NavPath.corners[i + 1], Color.red);

        }

       
        private Vector2 ToVector2(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
    }
}
