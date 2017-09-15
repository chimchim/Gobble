using System.Collections;
using System.Collections.Generic;
using Game.Systems;

namespace Game
{
    public class SystemManager
    {
        private List<ISystem> _systems = new List<ISystem>();
		
		public void UpdateAll(GameManager game, float delta)
		{
            var entities = game.Entities.GetEntites();
			foreach(ISystem system in _systems)
			{
                foreach (int e in entities)
                {
                    var messages = game.Entities.GetEntity(e).Messages;
                    for (int i = 0; i < messages.Count; i++)
                    {
                        system.SendMessage(game, e, messages[i]);

                    }
                }
                system.Update(game);
			}
		}

        public void CreateSystems()
        {
			_systems.Add(new Map());
			_systems.Add(new InputSystem());
			_systems.Add(new Movement());
		}
        public void InitAll(GameManager game)
        {
            foreach (ISystem system in _systems)
            {
                system.Initiate(game);
            }
        }
    }
}