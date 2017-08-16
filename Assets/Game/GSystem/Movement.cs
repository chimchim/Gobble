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
				entityGameObject.GetComponent<Animator>().SetBool("Run", false);
				input.CurrentGravity += -GameUnity.Gravity;

				float xMovement = 0;
				
				if (input.Axis != 0)
				{
					xMovement = input.Axis * GameUnity.PlayerSpeed * Time.deltaTime;
					entityGameObject.GetComponent<Animator>().SetBool("Run", true);
				}
				else
				{
					xMovement = 0;
				}

				if (input.Space && input.Grounded)
				{
					Debug.Log("jump");
					input.CurrentGravity = GameUnity.JumpSpeed;
				}
				float yMovement = input.CurrentGravity * Time.deltaTime;

				float xOffset = 0.35f;
				float yOffset = 0.65f;
				Vector3 tempPos = entityGameObject.transform.position;
				bool isGrounded = false;
				tempPos = VerticalMovement(tempPos, yMovement, xOffset, yOffset, out isGrounded);
				tempPos = HorizontalMovement(tempPos, xMovement, xOffset, yOffset);

				input.Grounded = isGrounded;
				if (isGrounded)
				{
					input.CurrentGravity = 0;
				}
				entityGameObject.transform.position = tempPos;
			}
		}

		public Vector3 VerticalMovement(Vector3 pos, float y, float Xoffset, float yoffset, out bool grounded)
		{
			float sign = Mathf.Sign(y);

			Vector3 firstStartY = new Vector3(-Xoffset + 0.05f, y, 0) + pos;
			Vector3 secondStartY = new Vector3(Xoffset - 0.05f, y, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			hitsY[0] = Physics2D.Raycast(firstStartY, Vector3.up * sign, yoffset);
			hitsY[1] = Physics2D.Raycast(secondStartY, Vector3.up * sign, yoffset);

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
		public Vector3 HorizontalMovement(Vector3 pos, float x, float xoffset, float yoffset)
		{
			float sign = Mathf.Sign(x);
			Vector3 firstStartX = new Vector3(x, -yoffset + 0.05f, 0) + pos;
			Vector3 secondStartX = new Vector3(x, yoffset - 0.05f, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			hitsY[0] = Physics2D.Raycast(firstStartX, Vector2.right * sign, xoffset);
			hitsY[1] = Physics2D.Raycast(secondStartX, Vector2.right * sign, xoffset);
			Debug.DrawLine(firstStartX, firstStartX + (Vector3.right * xoffset * sign), Color.red);
			Debug.DrawLine(secondStartX, secondStartX + (Vector3.right * xoffset * sign), Color.red);

			Vector3 movement = new Vector3(pos.x + x, pos.y, 0);
			for (int i = 0; i < hitsY.Length; i++)
			{
				if (hitsY[i].collider != null)
				{
					float distance = Mathf.Abs(hitsY[i].point.x - pos.x);
					float moveAmount = (xoffset) - distance;
					movement = new Vector3(pos.x + (moveAmount * -sign), pos.y, 0);
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