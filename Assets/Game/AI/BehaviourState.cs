using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;

namespace Game.AI
{
	public class BehaviourState
	{
		public List<AgentBehaviour> Behaviours = new List<AgentBehaviour>();
		public Dictionary<Type, string> Transitions = new Dictionary<Type, string>();
		public string StateName;

        
		public void EnterBehaviours(GameManager game)
		{
            for(int i = 0; i < Behaviours.Count; i++)
            {
                Behaviours[i].EnterState(game);
            }
		}

        public void ExecuteBehaviours(GameManager game)
		{
            for (int i = 0; i < Behaviours.Count; i++)
            {
                Behaviours[i].ExecuteState(game);
            }
		}

		public void ExitBehaviours(GameManager game)
		{
            for (int i = 0; i < Behaviours.Count; i++)
            {
                Behaviours[i].ExitState(game);
            }
		}

		public string TryTransition(GameManager game, Message msg)
		{
            SendMessage(game, msg);
            if (Transitions.ContainsKey(msg.GetType()))
			{
                return Transitions[msg.GetType()];
			}
			else
			{
				return null;
			}
		}
        public void SendMessage(GameManager game, Message msg)
        {
            for (int i = 0; i < Behaviours.Count; i++)
            {
                Behaviours[i].ReceiveMessage(game, msg);
            }
        }
	}
}
