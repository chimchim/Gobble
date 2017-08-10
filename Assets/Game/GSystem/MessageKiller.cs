using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
    public class MessageKiller : ISystem
    {
        private readonly Bitmask _bitmask = Bitmask.Zero;
        public void Update(GameManager game)
        {
            var entites = game.Entities.GetEntites();
            foreach (int e in entites)
            {
                var messList = game.Entities.GetEntity(e).Messages;
                for (int i = 0; i < messList.Count; i++)
                {
                    messList[i].Recycle();
                }
                messList.Clear();

                var newMess = game.Entities.GetEntity(e).NewMessages;
                for (int i = 0; i < newMess.Count; i++)
                {
                    messList.Add(newMess[i]);
                }
                newMess.Clear();
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
