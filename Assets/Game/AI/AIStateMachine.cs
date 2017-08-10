using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game;
namespace Game.AI
{
	public class AIStateMachine
	{
		public string AgentName { get; private set; }

		private string _currentState;
        private string _nextState;
        private int EntityID;
		private readonly Dictionary<string, BehaviourState> _states = new Dictionary<string, BehaviourState>();

		public void Update(GameManager game)
		{
            //string nextState = null;
			BehaviourState behaviourState = _states[_currentState];
            //foreach(Message mess in game.Entities.GetEntity(EntityID).Messages)
            //{
            //    Debug.Log("Change state to " + mess.GetType().ToString());
            //    nextState = behaviourState.TryTransition(mess.GetType());
            //}

            if (_nextState != null)
			{
				try
				{
					_states[_currentState].ExitBehaviours(game);
				}
				catch (Exception e)
				{
					Debug.Log("Exception in Exitstate " + _currentState);
				}
                _currentState = _nextState;
				try
				{
					_states[_currentState].EnterBehaviours(game);
				}
				catch (Exception e)
				{
					Debug.Log("Exception in Exitstate " + _currentState);
				}
			}
			_states[_currentState].ExecuteBehaviours(game);
            _nextState = null;
		}

        public void AddState(string s, BehaviourState behaviour)
        {
            _currentState = s;
            _states.Add(s, behaviour);
        }
        public void Init(GameManager game, int id)
        {
            foreach(string state in _states.Keys)
            {
                for (int i = 0; i < _states[state].Behaviours.Count; i++)
                {
                    _states[state].Behaviours[i].EntityID = id;
                    EntityID = id;
                }
            }

            foreach (string state in _states.Keys)
            {
                for (int i = 0; i < _states[state].Behaviours.Count; i++)
                {
                    _states[state].Behaviours[i].EnterState(game);
                }
            }
            _currentState = "FirstState";
        }

        public void ReceiveMessage(GameManager game, Message msg)
        {
            BehaviourState behaviourState = _states[_currentState];
            _nextState = behaviourState.TryTransition(game, msg);
            if (_nextState != null)
            {
                _states[_nextState].SendMessage(game, msg);
            }
        }
	}
}
