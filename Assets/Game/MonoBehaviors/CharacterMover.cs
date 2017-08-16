using UnityEngine;
using System.Collections;

public class CharacterMover : MonoBehaviour
{
	public Vector2 XRayOffset;
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 velocity;
	private bool Grounded;

	void LateUpdate()
	{
		float axis = Input.GetAxis("Horizontal");
		Vector3 thisMovement = Vector3.zero;
		GetComponent<Animator>().SetBool("Run", false);
		if (axis != 0)
		{
			moveDirection = new Vector3(axis, 0, 0) * speed;
			GetComponent<Animator>().SetBool("Run", true);
		}
		else
		{
			moveDirection = Vector3.zero;
		}
		velocity.y += -gravity;
		if (Input.GetKeyDown(KeyCode.Space))
		{
			velocity.y = jumpSpeed;
		}
		thisMovement += velocity * Time.deltaTime;
		thisMovement += moveDirection * Time.deltaTime;

		var sprite = GetComponent<SpriteRenderer>();
		float x = sprite.sprite.bounds.size.x;
		float y = sprite.sprite.bounds.size.y;


		float xOffset = 0.35f;
		float yOffset = 0.65f;
		Vector3 tempPos = transform.position;

		tempPos = VerticalMovement(tempPos, thisMovement.y, xOffset, yOffset);
		tempPos = HorizontalMovement(tempPos, thisMovement.x, xOffset, yOffset);
		

		transform.position = tempPos;

	}
	public Vector3 VerticalMovement(Vector3 pos, float y, float Xoffset, float yoffset)
	{
		float sign = Mathf.Sign(y);

		Vector3 firstStartY = new Vector3(-Xoffset + 0.05f, y, 0) + pos;
		Vector3 secondStartY = new Vector3(Xoffset - 0.05f, y, 0) + pos;
		RaycastHit2D[] hitsY = new RaycastHit2D[2];
		hitsY[0] = Physics2D.Raycast(firstStartY, Vector3.up * sign, yoffset);
		hitsY[1] = Physics2D.Raycast(secondStartY, Vector3.up * sign, yoffset);

		Debug.DrawLine(firstStartY, firstStartY + (Vector3.up * yoffset * sign), Color.red);
		Debug.DrawLine(secondStartY, secondStartY + (Vector3.up * yoffset * sign), Color.red);
		
		Vector3 movement = new Vector3(pos.x, pos.y + y, 0);
		for (int i = 0; i < hitsY.Length; i++)
		{
			if (hitsY[i].collider != null)
			{
				float distance = Mathf.Abs(hitsY[i].point.y - pos.y);

				float moveAmount = yoffset - distance;
				movement = new Vector3(pos.x, pos.y + (moveAmount * -sign), 0);
				velocity.y = 0;
				Grounded = true;
				break;
			}
			Grounded = false;
		}
		return movement;
	}
	public Vector3 HorizontalMovement(Vector3 pos, float x, float xoffset, float yoffset)
	{
		float sign = Mathf.Sign(x);
		Vector3 firstStartX = new Vector3(x, -yoffset + 0.05f, 0) + pos;
		Vector3 secondStartX = new Vector3(x, yoffset - 0.05f, 0) + pos;
		RaycastHit2D[] hitsY = new RaycastHit2D[2];
		hitsY[0] = Physics2D.Raycast(firstStartX, Vector2.right * sign, xoffset );
		hitsY[1] = Physics2D.Raycast(secondStartX, Vector2.right * sign, xoffset );
		Debug.DrawLine(firstStartX, firstStartX + (Vector3.right * xoffset * sign), Color.red);
		Debug.DrawLine(secondStartX, secondStartX + (Vector3.right * xoffset * sign), Color.red);

		Vector3 movement = new Vector3(pos.x + x, pos.y, 0);
		for (int i = 0; i < hitsY.Length; i++)
		{
			if (hitsY[i].collider != null)
			{
				float distance = Mathf.Abs(hitsY[i].point.x - pos.x);
				if (distance == 0)
					continue;
				float moveAmount = (xoffset) - distance;
				movement = new Vector3(pos.x + (moveAmount*-sign), pos.y, 0);
				break;
			}
			Grounded = false;
		}
		return movement;
	}
}