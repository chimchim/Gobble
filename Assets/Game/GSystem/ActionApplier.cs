using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;
using Game;

namespace Game.Systems
{
   
    public class ActionApplier : ISystem
    {
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<ActionQueue>();
        public void Update(GameManager game, float delta)
        {
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
            foreach (int e in entities)
            {
                var aq = game.Entities.GetComponentOf<ActionQueue>(e);
                foreach(Actions.Action action in aq.Actions)
                {
                    action.Apply(game, e);
                    action.Recycle();
                }
                aq.Actions.Clear();
            }
        }

        public void Initiate(GameManager game)
        {

        }
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
    }
}
