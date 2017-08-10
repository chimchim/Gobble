using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game.Component;
namespace Game.Actions
{
	public class WalkAction : Action
	{
		private static ObjectPool<WalkAction> _pool = new ObjectPool<WalkAction>(100);
		private Vector2 _translate;
		private float maxMove = 2.0f;
		public WalkAction()
		{

		}

		public static WalkAction Make(Vector2 trans)
		{
			WalkAction move = _pool.GetNext();
			move._translate = trans;
			return move;
		}

		public override void Apply(GameManager game, int owner)
		{
			Movement movement = game.Entities.GetComponentOf<Movement>(owner);
			GameObject go = game.Entities.GetEntity(owner).gameObject;
			movement.Input += new Vector2(_translate.x, _translate.y);
			foreach (Animator a in go.GetComponentsInChildren<Animator>())
			{
				a.SetBool("walk", true);
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