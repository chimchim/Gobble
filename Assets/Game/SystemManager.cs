using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game
{
    public class SystemManager
    {
		public enum GameState
		{
			None,
			Menu,
			Game,
			QuickJoin
		}
		public GameState GoToState;
		public GameState CurrentGameState;
        private Dictionary<GameState,List<ISystem>> _update = new Dictionary<GameState, List<ISystem>>();
		private Dictionary<GameState, List<ISystem>> _fixedUpdate = new Dictionary<GameState, List<ISystem>>();

		public void NormalUpdate(GameManager game, float delta)
		{
			if (CurrentGameState != GoToState && (GoToState != GameState.None))
			{
				CurrentGameState = GoToState;
				InitAll(game);
			}
			foreach(ISystem system in _update[CurrentGameState])
			{
                system.Update(game);
			}
		}

		public void FixedUpdate(GameManager game, float delta)
		{
			foreach (ISystem system in _fixedUpdate[CurrentGameState])
			{
				system.Update(game);
			}
		}

		public void ChangeState(GameManager game, GameState state)
		{
			GoToState = state;
		}

		public void CreateSystems()
        {
			_update.Add(GameState.Menu, new List<ISystem>());
			_fixedUpdate.Add(GameState.Menu, new List<ISystem>());
			_update.Add(GameState.Game, new List<ISystem>());
			_fixedUpdate.Add(GameState.Game, new List<ISystem>());
			_update.Add(GameState.QuickJoin, new List<ISystem>());
			_fixedUpdate.Add(GameState.QuickJoin, new List<ISystem>());

			var quickJoin = new QuickJoin();
            _update[GameState.QuickJoin].Add(quickJoin);
			_update[GameState.Game].Add(quickJoin);

			#region Using Menu
			_update[GameState.Menu].Add(new MenuSystem());
			_update[GameState.Game].Add(new Map());
			_update[GameState.Game].Add(new InitResources());
			_update[GameState.Game].Add(new InputSystem());
			_fixedUpdate[GameState.Game].Add(new Game.Systems.Movement());
			_fixedUpdate[GameState.Game].Add(new ResetInput()); 
			#endregion
		}
		
        public void InitAll(GameManager game)
        {
			UnityEngine.Debug.Log("Initate " + CurrentGameState);
            foreach (ISystem system in _update[CurrentGameState])
            {
                system.Initiate(game);
            }
			foreach (ISystem system in _fixedUpdate[CurrentGameState])
			{
				UnityEngine.Debug.Log("Initate system " + system);
				system.Initiate(game);
			}
		}
    }
}