using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;

namespace Game.AI
{
	public abstract class AgentBehaviour
	{
        public int EntityID;
        public AIStateMachine MyMachine;
        public virtual void EnterState(GameManager game){}
        public virtual void ExecuteState(GameManager game){}
        public virtual void ExitState(GameManager game){}

        public virtual void ReceiveMessage(GameManager game, Message msg)
        {

        }
	}
}
