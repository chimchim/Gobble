using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game.Component;

namespace Game
{

    public class Shot
    {

        public Vector2 Direction;
        public Vector3 Origin;
        public int Updates;

        public Shot(Vector2 direction, Vector3 origin,  int updates)
        {
            Direction = direction;
            Origin = origin;
            Updates = updates;
        }

    }
}
