using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
    public class AILogic : ISystem
    {
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Agent, PathMovement>();
        public void Update(GameManager game)
        {
            var agents = game.Entities.GetEntitiesWithComponents(_bitmask);
            foreach (int e in agents)
            {
                var agent = game.Entities.GetComponentOf<Agent>(e);
                agent.StateMachine.Update(game);

            }
        }

        public void Initiate(GameManager game)
        {
            var agents = game.Entities.GetEntitiesWithComponents(_bitmask);
            foreach (int e in agents)
            {
                var agent = game.Entities.GetComponentOf<Agent>(e);
                agent.StateMachine.Init(game, e);

            }
        }
        public void SendMessage(GameManager game, int reciever, Message message)
        {
            var agent = game.Entities.GetComponentOf<Agent>(reciever);
            agent.StateMachine.ReceiveMessage(game, message);
        }
    }
}
