using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game.Component;
namespace Game.Actions
{
    public class JumpAction : Action
    {
        private static ObjectPool<JumpAction> _pool = new ObjectPool<JumpAction>(100);
        private float _jumpForce;
        public JumpAction()
        {

        }

        public static JumpAction Make(float force)
        {
            var ret = _pool.GetNext();
            ret._jumpForce = force;
            return ret;
        }

        public override void Apply(GameManager game, int owner)
        {
            Movement movement = game.Entities.GetComponentOf<Movement>(owner);
            if (movement.Grounded)
            {
                movement.jumpForce = _jumpForce;
				GameObject go = game.Entities.GetEntity(owner).gameObject;
				//go.GetComponent<Animator>().SetTrigger("jump");
				foreach (Animator a in go.GetComponentsInChildren<Animator>())
				{
					a.SetTrigger("jump");
				}
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