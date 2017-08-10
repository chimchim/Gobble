using System;
using System.Collections.Generic;
using System.Text;
using Game.Component;

namespace Game.Actions
{
    public abstract class Action
    {
        public abstract string Type();
        public abstract void Apply(GameManager game, int entity);
        public abstract void Recycle();
    }
}



