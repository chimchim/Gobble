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

		public RabbitPatrol(int index) : base(index) { }
		public override void EnterState(GameManager game, Animal animal, Entity entity, Player host)
		{
			var position = entity.gameObject.transform.position;
			NewPosition(game, position);
		}

		private bool ReachedPosition(Game.GameManager game, Vector2 position, Animal animal, Entity entity, int rand, Player host)
		{
			if (rand == 0)
			{
				animal.TransitionState(game, entity, this.GetType(), typeof(RabbitChill), host);
				return false;
			}
			NewPosition(game, position);
			return true;
		}
		private void NewPosition(Game.GameManager game, Vector2 position)
		{
			int x = game.PrivateRandom.Next(1, 9);
			int sign = game.PrivateRandom.Next(0, 2);
			sign = sign == 0 ? -1 : 1;
			goTo.x = position.x + (sign * x);
			goTo.y = position.y;
			goTo.z = -0.2f;
		}
		public override void Update(GameManager game, Animal animal, Entity entity, Player host, float delta)
		{
			var position = entity.gameObject.transform.position;
			if (host.IsHost)
			{
				var closest = ClosestPlayer(game, position);
				var fromPlayer = position - closest;
				if (fromPlayer.magnitude < game.Animals.RabbitAggro)
				{
					animal.TransitionState(game, entity, this.GetType(), typeof(JumpFlee), host);
					return;
				}
				if (goTo == Vector3.zero)
				{
					NewPosition(game, position);
				}
			}

			var direction = goTo - position;
			animal.CurrentVelocity.y += -GameUnity.Gravity * animal.Weight;
			animal.CurrentVelocity.y = Mathf.Max(animal.CurrentVelocity.y, -GameUnity.MaxGravity);
			animal.CurrentVelocity.x = Math.Sign(direction.x) * game.Animals.RabbitSpeed;
			float yMovement = animal.CurrentVelocity.y * delta;
			float xMovement = animal.CurrentVelocity.x * delta;
			
			float xOffset = GameUnity.GroundHitBox.x;
			float yOffset = GameUnity.GroundHitBox.y;
			
			bool vertGrounded = false;
			bool horGrounded = false;
			if (!host.IsHost && goTo == Vector3.zero)
				xMovement = 0;
			Vector3 tempPos = position;
			int layer = (int)Systems.Movement.LayerMaskEnum.Grounded;
			var mask = game.LayerMasks.MappedMasks[layer];
			tempPos = Game.Systems.Movement.HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
			tempPos = Game.Systems.Movement.VerticalMovement(tempPos, yMovement, xOffset, yOffset, mask, out vertGrounded);
			tempPos.z = -0.2f;
			
			if (horGrounded && host.IsHost)
			{
				int rand = game.PrivateRandom.Next(0, 4);
				if (!ReachedPosition(game, position, animal, entity, rand, host))
					return;
			}
			if (vertGrounded)
			{
				animal.CurrentVelocity.y = 0;
				animal.Grounded = true;
			}

			entity.gameObject.transform.position = tempPos;
			entity.Animator.SetBool("Walk", (!horGrounded && vertGrounded));
			entity.Animator.SetBool("Fall", !vertGrounded);

			Debug.DrawLine(position, goTo, Color.red);
			if (Math.Abs(position.x - goTo.x) < 1)
			{
				goTo = Vector3.zero;
				int rand = game.PrivateRandom.Next(0, 2);
				if(host.IsHost)
					ReachedPosition(game, position, animal, entity, rand, host);
			}
		}

		public override void InnerSerialize(GameManager game, Animal animal, List<byte> byteArray)
		{
			byteArray.AddRange(BitConverter.GetBytes(goTo.x));
			byteArray.AddRange(BitConverter.GetBytes(goTo.y));
		}

		public override void InnerDeSerialize(GameManager game, Animal animal, byte[] byteData, ref int index)
		{
			float x = BitConverter.ToSingle(byteData, index);
			index += sizeof(float);
			float y = BitConverter.ToSingle(byteData, index);
			index += sizeof(float);
			goTo = new Vector3(x, y, -0.2f);
		}

		public override void LeaveState(GameManager game, Animal animal, Entity entity, Player host)
		{
			entity.Animator.SetBool("Fall", false);
			entity.Animator.SetBool("Walk", false);
		}
	}
}
