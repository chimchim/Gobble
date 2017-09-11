using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
    public class Movement : ISystem
    {
        private float _gravity = 20;

        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Game.Component.Input>();
		//				entityGameObject.GetComponent<Animator>().SetBool("Run", false);
		public void Update(GameManager game)
        {
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int e in entities)
			{
				var entityGameObject = game.Entities.GetEntity(e).gameObject;
				var input = game.Entities.GetComponentOf<Game.Component.Input>(e);
				var stats = game.Entities.GetComponentOf<Game.Component.Stats>(e);

				if (input.State == Component.Input.MoveState.Grounded)
				{
					input.CurrentVelocity.y += -GameUnity.Gravity;
					input.CurrentVelocity.y = Mathf.Max(input.CurrentVelocity.y, -GameUnity.MaxGravity);
					input.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;

					stats.OxygenSeconds += Time.deltaTime;
					stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

					if (input.Space && input.Grounded)
					{
						input.CurrentVelocity.y = GameUnity.JumpSpeed;
					}
					float yMovement = input.CurrentVelocity.y * Time.deltaTime;
					float xMovement = input.CurrentVelocity.x * Time.deltaTime;

					float xOffset = 0.35f;
					float yOffset = 0.65f;
					
					bool vertGrounded = false;
					bool horGrounded = false;

					Vector3 tempPos = entityGameObject.transform.position;
					tempPos = VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
					tempPos = HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
					entityGameObject.transform.position = tempPos;

					input.Grounded = vertGrounded;
					if (vertGrounded)
					{
						if (input.FallingTime > GameUnity.ExtraFallSpeedAfter)
						{
							float fallMulti = input.FallingTime - GameUnity.ExtraFallSpeedAfter;
							AffectHP fallDamage = AffectHP.Make(-GameUnity.FallDamage * fallMulti);
							fallDamage.Apply(game, e);
							fallDamage.Recycle();
						}
						input.FallingTime = 0;
						input.CurrentVelocity.y = 0;
					}
					else
					{
						if (input.CurrentVelocity.y < 0)
						{
							input.FallingTime += Time.deltaTime;
						}
					}
					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider != null)
					{
						input.FallingTime = 0;
						input.State = Component.Input.MoveState.Swimming;
						Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					}
				}
				if (input.State == Component.Input.MoveState.Floating)
				{
					input.CurrentVelocity.y = input.CurrentVelocity.y - GameUnity.Gravity;
					input.CurrentVelocity.y = Mathf.Max(input.CurrentVelocity.y, -GameUnity.MaxGravity);
					input.CurrentVelocity.x += input.Axis.x * GameUnity.SwimSpeed;
					input.CurrentVelocity.x = Mathf.Clamp(input.CurrentVelocity.x, -GameUnity.MaxWaterSpeed, GameUnity.MaxWaterSpeed);

					stats.OxygenSeconds += Time.deltaTime;
					stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

					//if (input.Space && input.FloatJump)
					//{
					//	input.CurrentVelocity.y = GameUnity.JumpSpeed / 2f;
					//	input.FloatJump = false;
					//}

					float yMovement = input.CurrentVelocity.y * Time.deltaTime;
					float xMovement = input.CurrentVelocity.x * Time.deltaTime;

					float xOffset = 0.35f;
					float yOffset = 0.65f;

					bool vertGrounded = false;
					bool horGrounded = false;

					Vector3 tempPos = entityGameObject.transform.position;
					tempPos = VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
					tempPos = HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
					entityGameObject.transform.position = tempPos;

					if (vertGrounded)
					{
						if (input.CurrentVelocity.y < 0)
						{
							input.State = Component.Input.MoveState.Grounded;
						}
						input.CurrentVelocity.y = 0;
					}

					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider != null)
					{
						input.FloatJump = true;
						input.State = Component.Input.MoveState.Swimming;
						Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					}

				}
				if (input.State == Component.Input.MoveState.Swimming)
				{
					input.CurrentVelocity.y += GameUnity.WaterGravity + (input.Axis.y * GameUnity.SwimSpeed);
					input.CurrentVelocity.y = Mathf.Clamp(input.CurrentVelocity.y, -GameUnity.MaxWaterSpeed, GameUnity.SwimUpExtraSpeed + GameUnity.MaxWaterSpeed);
					input.CurrentVelocity.x += input.Axis.x * GameUnity.SwimSpeed;
					input.CurrentVelocity.x = Mathf.Clamp(input.CurrentVelocity.x, -GameUnity.MaxWaterSpeed, GameUnity.MaxWaterSpeed);

					
					input.LoseOxygenAfter += Time.deltaTime;
					if (input.LoseOxygenAfter > GameUnity.LoseOxygenAfter)
					{
						stats.OxygenSeconds -= Time.deltaTime;
						stats.OxygenSeconds = Mathf.Max(0, stats.OxygenSeconds);
					}
					else
					{
						stats.OxygenSeconds += Time.deltaTime;
						stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);
					}
					float yMovement = input.CurrentVelocity.y * Time.deltaTime;
					float xMovement = input.CurrentVelocity.x * Time.deltaTime;

					Vector2 fullmovement = new Vector2(xMovement, yMovement);
					if (yMovement < 0)
					{
						fullmovement = fullmovement.normalized * fullmovement.magnitude * GameUnity.SwimDownMult;
					}
					float xOffset = 0.35f;
					float yOffset = 0.65f;

					bool vertGrounded = false;
					bool horGrounded = false;

					Vector3 tempPos = entityGameObject.transform.position;
					tempPos = VerticalMovement(tempPos, fullmovement.y, xOffset, yOffset, out vertGrounded);
					tempPos = HorizontalMovement(tempPos, fullmovement.x, xOffset, yOffset, out horGrounded);
					entityGameObject.transform.position = tempPos;

					if (vertGrounded)
					{
						input.CurrentVelocity.y = 0;
					}
					if (horGrounded)
					{
						input.CurrentVelocity.x = 0;
					}
					
					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider == null)
					{
						if (yMovement > 0)
						{
							input.CurrentVelocity.y = GameUnity.WaterJumpSpeed / 2f;
							if (input.Axis.y > 0)
							{
								input.CurrentVelocity.y = GameUnity.WaterJumpSpeed;
							}
						}
						input.LoseOxygenAfter = 0;
						input.State = Component.Input.MoveState.Floating;
						Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					}
				}
			}
		}

		public Vector3 VerticalMovement(Vector3 pos, float y, float Xoffset, float yoffset, out bool grounded)
		{
			float sign = Mathf.Sign(y);

			Vector3 firstStartY = new Vector3(-Xoffset + 0.05f, y, 0) + pos;
			Vector3 secondStartY = new Vector3(Xoffset - 0.05f, y, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");
			hitsY[0] = Physics2D.Raycast(firstStartY, Vector3.up * sign, yoffset, layerMask);
			hitsY[1] = Physics2D.Raycast(secondStartY, Vector3.up * sign, yoffset, layerMask);

			Debug.DrawLine(firstStartY, firstStartY + (Vector3.up * yoffset * sign), Color.red);
			Debug.DrawLine(secondStartY, secondStartY + (Vector3.up * yoffset * sign), Color.red);
			grounded = false;
			Vector3 movement = new Vector3(pos.x, pos.y + y, 0);
			for (int i = 0; i < hitsY.Length; i++)
			{
				if (hitsY[i].collider != null)
				{
					float distance = Mathf.Abs(hitsY[i].point.y - pos.y);

					float moveAmount = yoffset - distance;
					movement = new Vector3(pos.x, pos.y + (moveAmount * -sign), 0);
					grounded = true;
					break;
				}
			}
			return movement;
		}
		public Vector3 HorizontalMovement(Vector3 pos, float x, float xoffset, float yoffset, out bool grounded)
		{
			float sign = Mathf.Sign(x);
			Vector3 firstStartX = new Vector3(x, -yoffset + 0.05f, 0) + pos;
			Vector3 secondStartX = new Vector3(x, yoffset - 0.05f, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");
			hitsY[0] = Physics2D.Raycast(firstStartX, Vector2.right * sign, xoffset, layerMask);
			hitsY[1] = Physics2D.Raycast(secondStartX, Vector2.right * sign, xoffset, layerMask);
			Debug.DrawLine(firstStartX, firstStartX + (Vector3.right * xoffset * sign), Color.red);
			Debug.DrawLine(secondStartX, secondStartX + (Vector3.right * xoffset * sign), Color.red);
			grounded = false;
			Vector3 movement = new Vector3(pos.x + x, pos.y, 0);
			for (int i = 0; i < hitsY.Length; i++)
			{
				if (hitsY[i].collider != null)
				{
					float distance = Mathf.Abs(hitsY[i].point.x - pos.x);
					float moveAmount = (xoffset) - distance;
					movement = new Vector3(pos.x + (moveAmount * -sign), pos.y, 0);
					grounded = true;
					break;
				}
			}
			return movement;

		}
		public void Initiate(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int e in entities)
			{
				var stats = game.Entities.GetComponentOf<Game.Component.Stats>(e);
				var player = game.Entities.GetComponentOf<Game.Component.Player>(e);
				if (player.Owner)
				{
					var oxygenMeter = GameObject.FindObjectOfType<OxygenMeter>();
					oxygenMeter.PlayerStats = stats;
				}
			}
		}
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
    }
}