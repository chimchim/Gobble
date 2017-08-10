using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Misc
{
    public static class MathGame
    {
        public static Vector2 Rotate(Vector2 v, float degrees)
        {
            float DegToRad = (float)Math.PI / 180;
            return RotateRadians(v, degrees * DegToRad);
        }

        public static Vector2 RotateRadians(Vector2 v, float radians)
        {
            var ca = (float)Math.Cos(radians);
            var sa = (float)Math.Sin(radians);
            return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
        }
    }
}
