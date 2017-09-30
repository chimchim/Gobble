using System.Collections;
using System.Collections.Generic;
using Game.Systems;

namespace Game
{
    public class SystemManager
    {
        private List<ISystem> _update = new List<ISystem>();
		private List<ISystem> _fixedUpdate = new List<ISystem>();
		public void UpdateAll(GameManager game, float delta)
		{
            //var entities = game.Entities.GetEntites();
			foreach(ISystem system in _update)
			{

                system.Update(game);
			}
		}
		public void FixedUpdate(GameManager game, float delta)
		{
			//var entities = game.Entities.GetEntites();
			foreach (ISystem system in _fixedUpdate)
			{
				//foreach (int e in entities)
				//{
				//	var messages = game.Entities.GetEntity(e).Messages;
				//	for (int i = 0; i < messages.Count; i++)
				//	{
				//		system.SendMessage(game, e, messages[i]);
				//
				//	}
				//}
				system.Update(game);
			}
		}
		public void CreateSystems()
        {
			_update.Add(new Map());
			_update.Add(new InputSystem());
			_update.Add(new InitResources());
			_fixedUpdate.Add(new Game.Systems.Movement());
			_fixedUpdate.Add(new ResetInput());
		}
        public void InitAll(GameManager game)
        {
            foreach (ISystem system in _update)
            {
                system.Initiate(game);
            }
			foreach (ISystem system in _fixedUpdate)
			{
				system.Initiate(game);
			}
		}
    }
}