using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.Actions;
using Game.GEntity;
using Game.Component;
using Game.Systems;

namespace Game.Movement
{
	public class RabbitPatrol : AnimalState
	{
		Vector3 goTo = Vector3.zero;
		public override void EnterState(GameManager game, Animal animal, int entityID, Entity entity)
		{
			var position = entity.gameObject.transform.position;
			NewPosition(game, position);
		}

		private void ReachedPosition(Game.GameManager game, Vector2 position, Animal animal, Entity entity, int rand)
		{
			if (rand == 0)
			{
				animal.TransitionState(game, animal.EntityID, entity, this.GetType(), typeof(RabbitChill));
				return;
			}
			NewPosition(game, position);
		}
		private void NewPosition(Game.GameManager game, Vector2 position)
		{
			int x = game.CurrentRandom.Next(1, 9);
			int sign = game.CurrentRandom.Next(0, 2);
			sign = sign == 0 ? -1 : 1;
			goTo.x = position.x + (sign * x);
			goTo.y = position.y;
			goTo.z = -0.2f;
		}
		public override void Update(GameManager game, Animal animal, int entityID, Entity entity, float delta)
		{
			var position = entity.gameObject.transform.position;
			if (goTo == Vector3.zero)
			{
				NewPosition(game, position);
			}
			var direction = goTo - position;
			animal.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
			animal.CurrentVelocity.y = Mathf.Max(animal.CurrentVelocity.y, -GameUnity.MaxGravity);
			animal.CurrentVelocity.x = Math.Sign(direction.x) * GameUnity.RabbitSpeed;
			float yMovement = animal.CurrentVelocity.y * delta;
			float xMovement = animal.CurrentVelocity.x * delta;
			
			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;
			
			bool vertGrounded = false;
			bool horGrounded = false;
			
			Vector3 tempPos = position;
			int layer = (int)Systems.Movement.LayerMaskEnum.Grounded;
			var mask = game.LayerMasks.MappedMasks[layer];
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, mask, out vertGrounded);
			tempPos.z = -0.2f;
			
			if (horGrounded)
			{
				int rand = game.CurrentRandom.Next(0, 4);
				ReachedPosition(game, position, animal, entity, rand);
			}
			if (vertGrounded)
			{
				animal.CurrentVelocity.y = 0;
				animal.Grounded = true;
			}
			else
			{
				tempPos.x -= xMovement;
			}
			entity.gameObject.transform.position = tempPos;
			float signDir = animal.CurrentVelocity.x;
			if (Mathf.Abs(signDir) > 0.3f)
			{
				int mult = (int)Mathf.Max((1 + Mathf.Sign(signDir)), 1);
				entity.Animator.transform.eulerAngles = new Vector3(entity.Animator.transform.eulerAngles.x, mult * 180, entity.Animator.transform.eulerAngles.z);
			}
			entity.Animator.SetBool("Walk", (!horGrounded && vertGrounded));
			entity.Animator.SetBool("Fall", !vertGrounded);
			Debug.DrawLine(position, goTo, Color.red);
			if (Math.Abs(position.x - goTo.x) < 1)
			{
				int rand = game.CurrentRandom.Next(0, 2);
				ReachedPosition(game, position, animal, entity, rand);
			}
		}

		public override void LeaveState(GameManager game, Animal animal, int entityID, Entity entity)
		{
			entity.Animator.SetBool("Fall", false);
			entity.Animator.SetBool("Walk", false);
		}
	}
}
