using System;
using System.Collections.Generic;
using System.Collections;
using Game;
using Game.Component;
using UnityEngine;
using System.Linq;
using Game.Actions;
using Game.Misc;
namespace Game.AI.Behaviors
{
    public class RotateShooter : AgentBehaviour
    {
        private bool _shoot;
        private Vector3 _shootPoint;
        public override void EnterState(GameManager game)
        {

        }

        public override void ExecuteState(GameManager game)
        {
            if (_shoot)
            {
                var gameObjects = game.Entities.GetComponentOf<GameObjects>(EntityID);

                Vector2 GunForward = new Vector2(gameObjects.Aim.transform.forward.x, gameObjects.Aim.transform.forward.y);
                Vector3 Target = new Vector2(
                    (_shootPoint - gameObjects.Shooter.transform.position).x,
                    (_shootPoint.y - gameObjects.Shooter.transform.position.y - 0.09f) + ((_shootPoint.z - gameObjects.Shooter.transform.position.z) * (19.2f / 90f)));

                float angle = (180 - Vector2.Angle(GunForward, Target));
                Vector3 cross = Vector3.Cross(new Vector3(GunForward.x, GunForward.y, 0), new Vector3(Target.x, Target.y, 0));

                if (cross.z > 0)
                    angle = -angle;
                gameObjects.Shooter.transform.Rotate(Vector3.forward, angle, Space.World);
                var aq = game.Entities.GetComponentOf<ActionQueue>(EntityID);
                //aq.Actions.Add(ShootAction.Make());
                _shoot = false;
            }
        }
        public override void ReceiveMessage(GameManager game, Message msg)
        {
            if (msg is ShootPoint)
            {
                
                var shootPoint = (ShootPoint)msg;
                _shoot = true;
                _shootPoint = shootPoint.HitPoint;
            }
        }
    }
}
