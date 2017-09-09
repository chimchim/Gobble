using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
    public class Movement : ISystem
    {
        private float _gravity = 20;

        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Game.Component.Input>();

        public void Update(GameManager game)
        {
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int e in entities)
			{
				var entityGameObject = game.Entities.GetEntity(e).gameObject;
				var input = game.Entities.GetComponentOf<Game.Component.Input>(e);
				if (!input.Swimming)
				{
					entityGameObject.GetComponent<Animator>().SetBool("Run", false);
					input.CurrentVelocity.y += -GameUnity.Gravity;
					input.CurrentVelocity.y = Mathf.Max(input.CurrentVelocity.y, -GameUnity.MaxGravity);
					float xMovement = 0;

					if (input.Axis.x != 0)
					{
						input.CurrentVelocity.x = input.Axis.x * GameUnity.PlayerSpeed * Time.deltaTime;
						entityGameObject.GetComponent<Animator>().SetBool("Run", true);
					}
					else
					{
						input.CurrentVelocity.x = 0;
					}
					xMovement = input.CurrentVelocity.x;
					if (input.Space && input.Grounded)
					{
						Debug.Log("jump");
						input.CurrentVelocity.y = GameUnity.JumpSpeed;
					}
					float yMovement = input.CurrentVelocity.y * Time.deltaTime;

					float xOffset = 0.35f;
					float yOffset = 0.65f;
					Vector3 tempPos = entityGameObject.transform.position;
					bool isGrounded = false;
					bool horGrounded = false;
					tempPos = VerticalMovement(tempPos, yMovement, xOffset, yOffset, out isGrounded);
					tempPos = HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horGrounded);

					input.Grounded = isGrounded;
					if (isGrounded)
					{
						input.CurrentVelocity.y = 0;
					}
					entityGameObject.transform.position = tempPos;

					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider != null)
					{
						input.Swimming = true;
						input.EnterWater = 0.2f;
						Debug.DrawLine(topRayPos, topRayPos + (-Vector2.up * (yOffset)), Color.magenta);
					}
				}
				else
				{
					if (input.EnterWater <= 0)
					{
						input.SwimGravity += GameUnity.WaterGravity;
						input.SwimGravity = Mathf.Min(input.SwimGravity, GameUnity.MaxWaterGravity);
					}
					input.CurrentVelocity += new Vector2(input.Axis.x, input.Axis.y) * GameUnity.SwimSpeed;
					input.CurrentVelocity += new Vector2(0, GameUnity.WaterGravity);
					if (input.EnterWater <= 0)
					{
						input.CurrentVelocity = input.CurrentVelocity.normalized * Mathf.Min(input.CurrentVelocity.magnitude, GameUnity.MaxWaterSpeed);
					}
					else
					{
						input.CurrentVelocity = input.CurrentVelocity.normalized * Mathf.Min(input.CurrentVelocity.magnitude, GameUnity.PlayerSpeed);
					}
					input.EnterWater -= Time.deltaTime;
					float xMovement = input.CurrentVelocity.x * Time.deltaTime;
					float yMovement = input.CurrentVelocity.y * Time.deltaTime;// + (input.SwimGravity * Time.deltaTime);
					

					float xOffset = 0.35f;
					float yOffset = 0.65f;
					Vector3 tempPos = entityGameObject.transform.position;
					bool horizontalGrounded = false;
					bool verticalGrounded = false;
					tempPos = VerticalMovement(tempPos, yMovement, xOffset, yOffset, out verticalGrounded);
					tempPos = HorizontalMovement(tempPos, xMovement, xOffset, yOffset, out horizontalGrounded);

					if (horizontalGrounded)
					{
						input.CurrentVelocity.x = 0;
					}
					if (verticalGrounded)
					{
						input.CurrentVelocity.y = 0;
					}
					entityGameObject.transform.position = tempPos;

					var layerMask = 1 << LayerMask.NameToLayer("Water");
					var topRayPos = new Vector2(tempPos.x, tempPos.y + 0.65f);
					RaycastHit2D hit = Physics2D.Raycast(topRayPos, -Vector3.up, yOffset, layerMask);
					if (hit.collider == null)
					{
						input.Swimming = false;
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

        }
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
    }
}