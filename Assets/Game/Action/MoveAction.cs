using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game.Component;
namespace Game.Actions
{
    public class MoveAction : Action
    {
        private static ObjectPool<MoveAction> _pool = new ObjectPool<MoveAction>(100);
        private Vector2 _translate;
        private float maxMove = 2.0f;
        public MoveAction()
        {

        }

        public static MoveAction Make(Vector2 trans)
        {
            MoveAction move = _pool.GetNext();
            move._translate = trans;
            return move;
        }

        public override void Apply(GameManager game, int owner)
        {
            
            Movement movement = game.Entities.GetComponentOf<Movement>(owner);
            GameObject go = game.Entities.GetEntity(owner).gameObject;
            movement.Input += new Vector2(_translate.x, _translate.y);
            //go.GetComponentsInChildren<Animator>().SetBool("run", true);
			foreach (Animator a in go.GetComponentsInChildren<Animator>())
			{
				a.SetBool("run", true);
			}
        }

        public override void Recycle()
        {
            _pool.Recycle(this);
        }

        public override string Type()
        {
            return "MoveAction";
        }
    }
}