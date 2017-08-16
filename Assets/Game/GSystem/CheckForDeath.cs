using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class CheckForDeath : ISystem
	{
		private Dictionary<int, float> deathTimer = new Dictionary<int, float>();

		private float deathTime = 5;
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<GameObjects, Stats>();
		public void Update(GameManager game)
		{
		}

        public void Initiate(GameManager game)
		{
		}
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
	}
}
