using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using UnityEngine;

namespace Game
{
	public class GameManager
	{
        private EntityManager _entityManager = new EntityManager();
		private SystemManager _systemManager = new SystemManager();
        private ParticleManager _particleManager = new ParticleManager();

        public EntityManager Entities { get { return _entityManager; } }
		public SystemManager Systems { get { return _systemManager; } }
        public ParticleManager Particles { get { return _particleManager; } }

		public GameManager()
		{
            
		}

        public void Update(float delta)
        { 
			_systemManager.UpdateAll(this, delta);
            //_particleManager.Update();
        }

		public void Initiate()
		{
            //_particleManager.Initiate();
            _systemManager.InitAll(this);
		}
        public void SendMessage(int id, Message mess)
        {
            Entities.AddMessage(id, mess);
        }
	}
}
