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
    public class ForwardSensor : AgentBehaviour
    {

        private Transform _eyes;
        private Vector2 _rayDirection;
        private int _angleCount;
        private int sensorY;
        private Transform head;
        public override void EnterState(GameManager game)
        {
            head = game.Entities.GetComponentOf<GameObjects>(EntityID).Head.transform;
			_eyes = head.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "Eyes");
            _rayDirection = new Vector2(1, 0);
            sensorY = 1;
        }

        public override void ExecuteState(GameManager game)
        {
            // ROTATE SENSOR
            
        }
        public override void ExitState(GameManager game)
        {

        }
    }
}
