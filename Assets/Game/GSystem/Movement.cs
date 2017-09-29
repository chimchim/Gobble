using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using Game.Actions;
using UnityEditor;

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
				var movement = game.Entities.GetComponentOf<Game.Component.Movement>(e);
				movement.CurrentVelocity += movement.ForceVelocity;
				#region Debug
				if (movement.State == Component.Movement.MoveState.FlyingDebug)
				{
					movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed * 3;
					movement.CurrentVelocity.y = input.Axis.y * GameUnity.PlayerSpeed * 3;
					float yMovement = movement.CurrentVelocity.y * Time.deltaTime;
					float xMovement = movement.CurrentVelocity.x * Time.deltaTime;

					entityGameObject.transform.position += new Vector3(xMovement, yMovement, 0);
				} 
				#endregion
				if (movement.State == Component.Movement.MoveState.Roped)
				{
					Vector2 playerPos = entityGameObject.transform.position;
					Vector2 playerPosFirstMove = playerPos + (movement.CurrentVelocity * Time.deltaTime);
					Vector2 origin = movement.CurrentRoped.origin;

					float len = movement.CurrentRoped.Length;
					float diff = (playerPosFirstMove - origin).magnitude;
					float vel = movement.CurrentRoped.Vel;
					float angle = movement.CurrentRoped.Angle;
					float aAcc = 0;
					Debug.DrawLine(playerPos, origin, Color.red);
					float yMovement = 0;
					float xMovement = 0;
					float lastAngle = 0;
					if (diff > len || movement.CurrentRoped.FirstAngle || (movement.CurrentVelocity.y < 0 && playerPos.y < origin.y))
					{

						#region First Angle
						if (!movement.CurrentRoped.FirstAngle)
						{
							movement.CurrentRoped.Length = (playerPos - origin).magnitude;
							len = movement.CurrentRoped.Length;
							float angle2 = Vector2.Angle((origin - playerPos).normalized, (-Vector2.up));
							if (origin.x > playerPos.x)
							{
								angle = (Mathf.PI / 180f) * (180 - angle2);
							}
							else
							{
								angle = (Mathf.PI / 180f) * -(180 - angle2);
							}
							movement.CurrentRoped.Angle = angle;
							movement.CurrentRoped.FirstAngle = true;
							movement.CurrentRoped.Vel = 1;

							float sinned = Mathf.Abs(Mathf.Sin(angle));
							float cosed = Mathf.Abs(Mathf.Cos(angle));
							float newXVel = movement.CurrentVelocity.x * cosed;
							float newYVel = movement.CurrentVelocity.y * sinned;

							float velXDir = newXVel * -Mathf.Sign(Mathf.Cos(angle));
							float velYDir = newYVel * Mathf.Sign(Mathf.Sin(angle));

							float newSpeed = velXDir + velYDir;
							float ropeDirection = Mathf.Sign(newSpeed);
							float deltaTimeMult = 1 / Time.deltaTime;

							//Debug.Log("newSpeed " + newSpeed);

							float tempAngle = movement.CurrentRoped.Vel + angle;
							playerPos.x = origin.x + (-len * Mathf.Sin(tempAngle));
							playerPos.y = origin.y + (-len * Mathf.Cos(tempAngle));

							Vector2 ropeMovePos = playerPos - new Vector2(entityGameObject.transform.position.x, entityGameObject.transform.position.y);
							float ropeSpeed = (ropeMovePos.magnitude) * deltaTimeMult;
							float velDivider = movement.CurrentRoped.Vel / ropeSpeed;
							float newVel = Mathf.Abs(velDivider) * Mathf.Abs(newSpeed) * ropeDirection; // 6 = New ropeSpeed
							movement.CurrentRoped.Vel = newVel;

							movement.CurrentVelocity = Vector2.zero;
						} 
						#endregion
						playerPos.x = origin.x + (-len * Mathf.Sin(angle));
						playerPos.y = origin.y + (-len * Mathf.Cos(angle));
						lastAngle = angle - movement.CurrentRoped.Vel;

						xMovement = playerPos.x - entityGameObject.transform.position.x;
						yMovement = playerPos.y - entityGameObject.transform.position.y;
						
						float gravity = GameUnity.RopeGravity;
						#region Rope Input
						if (playerPos.x < origin.x)
						{
							if (input.Axis.x > 0)
							{
								gravity += gravity * GameUnity.RopeSpeedMult;
							}
						}
						if (playerPos.x > origin.x)
						{
							if (input.Axis.x < 0)
							{
								gravity += gravity * GameUnity.RopeSpeedMult;
							}
						} 
						#endregion
						aAcc = (-1 * gravity / len) * Mathf.Sin(angle) * Time.deltaTime;
						movement.CurrentRoped.Vel += aAcc;
						movement.CurrentRoped.Vel *= movement.CurrentRoped.Damp;
						movement.CurrentRoped.Angle += movement.CurrentRoped.Vel;

					}
					else
					{
						
						movement.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
						movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
						movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;
						yMovement = movement.CurrentVelocity.y * Time.deltaTime;
						xMovement = movement.CurrentVelocity.x * Time.deltaTime;
					}
					stats.OxygenSeconds += Time.deltaTime;
					stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);
					
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
						if (yMovement > 0)
						{
							movement.CurrentRoped.Angle = lastAngle;
							movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * 0.3f;
						}
						else
						{
							movement.State = Component.Movement.MoveState.Grounded;
						}
					}
					if (horGrounded)
					{

						movement.CurrentRoped.Angle = lastAngle;
						movement.CurrentRoped.Vel = -movement.CurrentRoped.Vel * 0.3f;
					}

					movement.Grounded = vertGrounded;
					
					//var layerMask = 1 << LayerMask.NameToLayer("Water");
					//var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					//RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					//if (hit.collider != null)
					//{
					//	movement.FallingTime = 0;
					//	movement.State = Component.Movement.MoveState.Swimming;
					//	Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					//}
				} 
				#region Grounded
				if (movement.State == Component.Movement.MoveState.Grounded)
				{
					movement.CurrentVelocity.y += -GameUnity.Gravity * GameUnity.Weight;
					movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
					movement.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed;

					stats.OxygenSeconds += Time.deltaTime;
					stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

					if (input.Space && movement.Grounded)
					{
						movement.CurrentVelocity.y = GameUnity.JumpSpeed;
					}

					float yMovement = movement.CurrentVelocity.y * Time.deltaTime;
					float xMovement = movement.CurrentVelocity.x * Time.deltaTime;

					float xOffset = 0.35f;
					float yOffset = 0.65f;

					bool vertGrounded = false;
					bool horGrounded = false;

					Vector3 tempPos = entityGameObject.transform.position;
					tempPos = VerticalMovement(tempPos, yMovement, xOffset, yOffset, out vertGrounded);
					tempPos = HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);
					entityGameObject.transform.position = tempPos;
					movement.Grounded = vertGrounded;
					if (vertGrounded)
					{
						if (movement.FallingTime > GameUnity.ExtraFallSpeedAfter)
						{
							float fallMulti = movement.FallingTime - GameUnity.ExtraFallSpeedAfter;
							AffectHP fallDamage = AffectHP.Make(-GameUnity.FallDamage * fallMulti);
							fallDamage.Apply(game, e);
							fallDamage.Recycle();
						}
						movement.FallingTime = 0;
						movement.CurrentVelocity.y = 0;
					}
					else
					{
						if (movement.CurrentVelocity.y < 0)
						{
							movement.FallingTime += Time.deltaTime;
						}
					}
					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider != null)
					{
						movement.FallingTime = 0;
						movement.State = Component.Movement.MoveState.Swimming;
						Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					}
				} 
				#endregion
				#region Floating
				if (movement.State == Component.Movement.MoveState.Floating)
				{
					movement.CurrentVelocity.y = movement.CurrentVelocity.y - GameUnity.Gravity;
					movement.CurrentVelocity.y = Mathf.Max(movement.CurrentVelocity.y, -GameUnity.MaxGravity);
					movement.CurrentVelocity.x += input.Axis.x * GameUnity.SwimSpeed;
					movement.CurrentVelocity.x = Mathf.Clamp(movement.CurrentVelocity.x, -GameUnity.MaxWaterSpeed, GameUnity.MaxWaterSpeed);

					stats.OxygenSeconds += Time.deltaTime;
					stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);

					float yMovement = movement.CurrentVelocity.y * Time.deltaTime;
					float xMovement = movement.CurrentVelocity.x * Time.deltaTime;

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
						if (movement.CurrentVelocity.y < 0)
						{
							movement.State = Component.Movement.MoveState.Grounded;
						}
						movement.CurrentVelocity.y = 0;
					}

					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider != null)
					{
						movement.FloatJump = true;
						movement.State = Component.Movement.MoveState.Swimming;
						Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					}

				} 
				#endregion
				#region Swimming
				if (movement.State == Component.Movement.MoveState.Swimming)
				{
					movement.CurrentVelocity.y += GameUnity.WaterGravity + (input.Axis.y * GameUnity.SwimSpeed);
					movement.CurrentVelocity.y = Mathf.Clamp(movement.CurrentVelocity.y, -GameUnity.MaxWaterSpeed, GameUnity.SwimUpExtraSpeed + GameUnity.MaxWaterSpeed);
					movement.CurrentVelocity.x += input.Axis.x * GameUnity.SwimSpeed;
					movement.CurrentVelocity.x = Mathf.Clamp(movement.CurrentVelocity.x, -GameUnity.MaxWaterSpeed, GameUnity.MaxWaterSpeed);


					movement.SwimTime += Time.deltaTime;
					if (movement.SwimTime > GameUnity.LoseOxygenAfter)
					{
						float oxygenDepletionTime = movement.SwimTime - GameUnity.OxygenTime;
						if (((int)oxygenDepletionTime) == movement.OxygenDeplationTick)
						{
							movement.OxygenDeplationTick++;
							AffectHP damage = AffectHP.Make(-GameUnity.OxygenDPS);
							damage.Apply(game, e);
							damage.Recycle();
						}
						stats.OxygenSeconds -= Time.deltaTime;
						stats.OxygenSeconds = Mathf.Max(0, stats.OxygenSeconds);
					}
					else
					{
						stats.OxygenSeconds += Time.deltaTime;
						stats.OxygenSeconds = Mathf.Min(stats.OxygenSeconds, stats.MaxOxygenSeconds);
					}
					float yMovement = movement.CurrentVelocity.y * Time.deltaTime;
					float xMovement = movement.CurrentVelocity.x * Time.deltaTime;

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
						movement.CurrentVelocity.y = 0;
					}
					if (horGrounded)
					{
						movement.CurrentVelocity.x = 0;
					}

					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider == null)
					{
						if (yMovement > 0)
						{
							movement.FloatingCounter++;
							movement.CurrentVelocity.y = GameUnity.WaterJumpSpeed / 4f;
							if (input.Axis.y > 0 && movement.FloatingCounter >= GameUnity.FloatJumpEvery)
							{
								movement.FloatingCounter = 0;
								movement.CurrentVelocity.y = GameUnity.WaterJumpSpeed;
							}
						}
						movement.SwimTime = 0;
						movement.OxygenDeplationTick = 0;
						movement.State = Component.Movement.MoveState.Floating;
						Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					}
				} 
				#endregion
				entityGameObject.transform.position = new Vector3(entityGameObject.transform.position.x, entityGameObject.transform.position.y, -0.2f);
			}
		}

		public Vector3 VerticalMovement(Vector3 pos, float y, float Xoffset, float yoffset, out bool grounded)
		{
			float fullRayDistance = yoffset + Mathf.Abs(y);
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");
			float sign = Mathf.Sign(y);
			Vector3 firstStartY = new Vector3(-Xoffset + 0.05f, 0, 0) + pos;
			Vector3 secondStartY = new Vector3(Xoffset - 0.05f, 0, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			hitsY[0] = Physics2D.Raycast(firstStartY, Vector3.up * sign, fullRayDistance, layerMask);
			hitsY[1] = Physics2D.Raycast(secondStartY, Vector3.up * sign, fullRayDistance, layerMask);
			Debug.DrawLine(firstStartY, firstStartY + (Vector3.up * fullRayDistance * sign), Color.red);
			Debug.DrawLine(secondStartY, secondStartY + (Vector3.up * fullRayDistance * sign), Color.red);
			grounded = false;
			Vector3 movement = new Vector3(pos.x, pos.y + y, 0);
			for (int i = 0; i < hitsY.Length; i++)
			{
				if (hitsY[i].collider != null)
				{
					float distance = Mathf.Abs(hitsY[i].point.y - pos.y);
					float moveAmount = (fullRayDistance - distance);
					moveAmount = (distance * sign) + (yoffset * -sign);
					movement = new Vector3(pos.x, pos.y + (moveAmount), 0);
					grounded = true;
					break;
				}
			}
			
			return movement;
		}
		public Vector3 HorizontalMovement(Vector3 pos, float x, float xoffset, float yoffset, out bool grounded)
		{
			float fullRayDistance = xoffset + Mathf.Abs(x);
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");
			float sign = Mathf.Sign(x);
			Vector3 firstStartX = new Vector3(0, -yoffset + 0.05f, 0) + pos;
			Vector3 secondStartX = new Vector3(0, yoffset - 0.05f, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			hitsY[0] = Physics2D.Raycast(firstStartX, Vector2.right * sign, fullRayDistance, layerMask);
			hitsY[1] = Physics2D.Raycast(secondStartX, Vector2.right * sign, fullRayDistance, layerMask);
			Debug.DrawLine(firstStartX, firstStartX + (Vector3.right * fullRayDistance * sign), Color.red);
			Debug.DrawLine(secondStartX, secondStartX + (Vector3.right * fullRayDistance * sign), Color.red);
			grounded = false;

			Vector3 movement = new Vector3(pos.x + x, pos.y, 0);
			for (int i = 0; i < hitsY.Length; i++)
			{
				if (hitsY[i].collider != null)
				{
					float distance = Mathf.Abs(hitsY[i].point.x - pos.x);
					float moveAmount = (fullRayDistance - distance);
					moveAmount = (distance * sign) + (xoffset * -sign);
					movement = new Vector3(pos.x + (moveAmount), pos.y, 0);
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
				var input = game.Entities.GetComponentOf<Game.Component.Input>(e);
				var movement = game.Entities.GetComponentOf<Game.Component.Movement>(e);
				if (player.Owner)
				{
					if (GameUnity.DebugMode)
					{
						movement.State = Component.Movement.MoveState.FlyingDebug;
					}
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
//Debug.DrawLine(firstStartX, firstStartX + (Vector3.right * xoffset * sign), Color.red);
//Debug.DrawLine(secondStartX, secondStartX + (Vector3.right * xoffset * sign), Color.red);