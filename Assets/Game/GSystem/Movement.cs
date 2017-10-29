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

        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent>();
		
		public void Update(GameManager game)
        {
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int e in entities)
			{
				var entity = game.Entities.GetEntity(e);
				var entityGameObject = entity.gameObject;
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var stats = game.Entities.GetComponentOf<Game.Component.Stats>(e);
				var movement = game.Entities.GetComponentOf<MovementComponent>(e);
				var rope = game.Entities.GetComponentOf<ResourcesComponent>(e).GraphicRope;
				rope.UpdateRope();
				if (entity.Animator == null)
					continue;

				float signDir = movement.CurrentVelocity.x + movement.ForceVelocity.x;
				if (Mathf.Abs(signDir) > 0.1f)
				{
					int mult = (int)Mathf.Max((1 + Mathf.Sign(signDir)), 1);
					entity.Animator.transform.eulerAngles = new Vector3(entity.Animator.transform.eulerAngles.x, mult * 180, entity.Animator.transform.eulerAngles.z);

				}

				int currentStateIndex = (int)movement.CurrentState;
				movement.States[currentStateIndex].Update(game, movement, e, entity);
				entityGameObject.transform.position = new Vector3(entityGameObject.transform.position.x, entityGameObject.transform.position.y, -0.2f);
			}
		}
		public static void DoJump(GameManager game, int id)
		{
			var animator = game.Entities.GetEntity(id).Animator;
			var movement = game.Entities.GetComponentOf<MovementComponent>(id);

			movement.CurrentVelocity.y = GameUnity.JumpSpeed;
			animator.SetBool("Jump", true);

		}
		public static Vector3 VerticalMovement(Vector3 pos, float y, float Xoffset, float yoffset, out bool grounded)
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
		public static Vector3 HorizontalMovement(Vector3 pos, float x, float xoffset, float yoffset, out bool grounded)
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
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var movement = game.Entities.GetComponentOf<MovementComponent>(e);
				var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);

				Debug.Log("initiate aniamtor");
				if (player.Owner)
				{
					if (GameUnity.DebugMode)
					{
						movement.CurrentState = MovementComponent.MoveState.FlyingDebug;
					}
					
					var oxygenMeter = GameObject.FindObjectOfType<OxygenMeter>();
					var hpbar = GameObject.FindObjectOfType<HpBar>();
					stats.HpBar = hpbar;
					stats.HpBar.Bar.gameObject.SetActive(true);
					oxygenMeter.gameObject.SetActive(true);
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