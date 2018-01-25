using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;
using Game.Actions;
using System;

namespace Game.Systems
{
    public class Movement : ISystem
    {
        private float _gravity = 20;
		public enum LayerMaskEnum
		{
			Grounded,
			Roped,
			Ladder,
			Attack
		}
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<InputComponent>();
		
		public void Update(GameManager game, float delta)
        {
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int e in entities)
			{
				var entity = game.Entities.GetEntity(e);
				
				var entityGameObject = entity.gameObject;
				entity.LastPosition = entityGameObject.transform.position;
				var input = game.Entities.GetComponentOf<InputComponent>(e);
				var stats = game.Entities.GetComponentOf<Game.Component.Stats>(e);
				var movement = game.Entities.GetComponentOf<MovementComponent>(e);
				var resource = game.Entities.GetComponentOf<ResourcesComponent>(e);
				resource.GraphicRope.UpdateRope();
				if (entity.Animator == null)
					continue;

				float signDir = movement.CurrentVelocity.x + movement.ForceVelocity.x;

				if (Mathf.Abs(signDir) > 0.3f)
				{
					int mult = (int)Mathf.Max((1 + Mathf.Sign(signDir)), 1);
					entity.Animator.transform.eulerAngles = new Vector3(entity.Animator.transform.eulerAngles.x, mult * 180, entity.Animator.transform.eulerAngles.z);
					resource.FacingDirection = (int)Mathf.Sign(signDir);
				}

				int currentStateIndex = (int)movement.CurrentState;
				movement.States[currentStateIndex].Update(game, movement, e, entity, delta);
				entityGameObject.transform.position = new Vector3(entityGameObject.transform.position.x, entityGameObject.transform.position.y, -0.2f);
				entity.PlayerSpeed = (entityGameObject.transform.position - entity.LastPosition) * (1/Time.fixedDeltaTime);
			}
		}
		public static void DoJump(GameManager game, int id)
		{
			var animator = game.Entities.GetEntity(id).Animator;
			var movement = game.Entities.GetComponentOf<MovementComponent>(id);

			movement.CurrentVelocity.y = GameUnity.JumpSpeed;
			animator.SetBool("Jump", true);

		}

		public static bool CheckGrounded(Vector2 tempPos, float y, float yOffset, MappedMasks masks, out int layer)
		{
			layer = 0;
			LayerMask layerMask = 0;// = masks.DownLayers[0];
			#region Layers
			if (y < 0)
			{
				layerMask = masks.DownLayers;
			}
			else
			{
				layerMask = masks.UpLayers;
			}
			#endregion
			Vector2 yDirection = new Vector2(0, y).normalized;
			float ySize = Mathf.Abs(y);

			for (int i = -1; i < 2; i++)
			{
				var pos = tempPos + new Vector2((i * 0.1f), yOffset * Math.Sign(y));
				var hit2 = Physics2D.Raycast(pos, yDirection, ySize, layerMask);
				Debug.DrawLine(pos, pos + (yDirection * ySize), Color.red);
				if (hit2.transform != null)
				{
					layer = hit2.transform.gameObject.layer;
					return true;
				}
			}
			return false;
		}

		public static void NetSync(GameManager game, Player player, MovementComponent movement, InputComponent input, int e, float delta)
		{
			var entity = game.Entities.GetEntity(e);
			var otherTransform = entity.gameObject.transform;
			Vector2 otherPosition = otherTransform.position;
			Vector2 networkPosition = input.NetworkPosition;
			//Debug.DrawLine(otherPosition, networkPosition, Color.green);
			
			Vector2 diff = networkPosition - otherPosition;
			Debug.DrawLine(otherPosition, otherPosition +(diff.normalized * 10), Color.green);
			if (!player.Owner && movement.CurrentState != MovementComponent.MoveState.Roped)
			{

				float speed = GameUnity.NetworkLerpSpeed + (GameUnity.NetworkLerpSpeed * (diff.magnitude / GameUnity.NetworkLerpSpeed));
				Vector2 translate = diff.normalized * GameUnity.NetworkLerpSpeed * delta;
				translate = translate.magnitude > diff.magnitude ? diff : translate;
				Vector2 translatePos = otherPosition + translate;
				movement.Body.MovePosition(translatePos);
				Vector2 newPos = otherTransform.position;
				Vector2 newPosDiff = newPos - otherPosition;
				float dot = Vector2.Dot(newPosDiff.normalized, diff.normalized);

				if (dot < 0)
				{
					Debug.Log("SNAP DOT ");
					entity.gameObject.transform.position = networkPosition;
					movement.Body.MovePosition(networkPosition);
				}
				if (diff.magnitude > 1.2f)
				{
					var resources = game.Entities.GetComponentOf<ResourcesComponent>(e);
					resources.LerpCharacter.SetLerp(entity.gameObject.transform.position, networkPosition);
					Debug.Log("SNAP diff.magnitude > 3");
					entity.gameObject.transform.position = networkPosition;
					movement.Body.MovePosition(networkPosition);
				}
			}
		}

		public static Vector3 VerticalMovement(Vector3 pos, float y, float Xoffset, float yoffset, MappedMasks masks, out bool grounded)
		{
			float half = yoffset - (yoffset/10);
			float fullRayDistance = yoffset + Mathf.Abs(y) - half;
			LayerMask layerMask = 0;// = masks.DownLayers[0];
			#region Layers
			if (y < 0)
			{
				layerMask = masks.DownLayers;
			}
			else
			{
				layerMask = masks.UpLayers;
			} 
			#endregion

			float sign = Mathf.Sign(y);
			Vector3 firstStartY = new Vector3(-Xoffset + 0.05f, half * sign, 0) + pos;
			Vector3 secondStartY = new Vector3(Xoffset - 0.05f, half * sign, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			hitsY[0] = Physics2D.Raycast(firstStartY, Vector3.up * sign, fullRayDistance, layerMask);
			hitsY[1] = Physics2D.Raycast(secondStartY, Vector3.up * sign, fullRayDistance, layerMask);
			Debug.DrawLine(firstStartY, firstStartY + (Vector3.up * fullRayDistance * sign), Color.red);
			Debug.DrawLine(secondStartY, secondStartY + (Vector3.up * fullRayDistance * sign), Color.red);
			grounded = false;
			Vector3 movement = new Vector3(pos.x, pos.y + y, 0);
			for (int i = 0; i < hitsY.Length; i++)
			{
				if (hitsY[i].collider != null && hitsY[i].distance > 0)
				{
					float distance = Mathf.Abs(hitsY[i].point.y - pos.y);
					float moveAmount = (distance * sign) + ((yoffset) * -sign);
					movement = new Vector3(pos.x, pos.y + (moveAmount), 0);
					grounded = true;
					break;
				}
			}
			
			return movement;
		}
		public static Vector3 HorizontalMovement(Vector3 pos, float x, float xoffset, float yoffset, out bool grounded)
		{
			grounded = false;
			if (Mathf.Abs(x) <= 0)
				return pos;
			float half = xoffset - (xoffset / 10);
			float fullRayDistance = xoffset + Mathf.Abs(x) - half;
			var layerMask = 1 << LayerMask.NameToLayer("Collideable");
			float sign = Mathf.Sign(x);
			Vector3 firstStartX = new Vector3(half * sign, -yoffset + 0.05f, 0) + pos;
			Vector3 secondStartX = new Vector3(half * sign, yoffset - 0.05f, 0) + pos;
			RaycastHit2D[] hitsY = new RaycastHit2D[2];
			hitsY[0] = Physics2D.Raycast(firstStartX, Vector2.right * sign, fullRayDistance, layerMask);
			hitsY[1] = Physics2D.Raycast(secondStartX, Vector2.right * sign, fullRayDistance, layerMask);
			Debug.DrawLine(firstStartX, firstStartX + (Vector3.right * fullRayDistance * sign), Color.red);
			Debug.DrawLine(secondStartX, secondStartX + (Vector3.right * fullRayDistance * sign), Color.red);

			Vector3 movement = new Vector3(pos.x + x, pos.y, 0);
			for (int i = 0; i < hitsY.Length; i++)
			{
				if (hitsY[i].collider != null && hitsY[i].distance > 0)
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
				movement.Body = game.Entities.GetEntity(e).gameObject.GetComponent<Rigidbody2D>();
				Debug.Log("initiate aniamtor");
				if (player.Owner)
				{
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