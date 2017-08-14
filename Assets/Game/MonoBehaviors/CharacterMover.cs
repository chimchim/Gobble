using UnityEngine;
using System.Collections;

public class CharacterMover : MonoBehaviour
{
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 velocity;
	private bool Grounded;

	void Update()
	{
		float axis = Input.GetAxis("Horizontal");

		GetComponent<Animator>().SetBool("Run", false);
		if (axis != 0)
		{
			moveDirection = new Vector3(axis, 0, 0) * speed;
			GetComponent<Animator>().SetBool("Run", true);
		}
		velocity.y += -gravity;
		if (Input.GetKeyDown(KeyCode.Space) && Grounded)
		{
			velocity.y = jumpSpeed;
		}
		transform.position += velocity * Time.deltaTime;
		transform.position += moveDirection * Time.deltaTime;

		var sprite = GetComponent<SpriteRenderer>();
		float x = sprite.sprite.bounds.size.x;
		float y = sprite.sprite.bounds.size.y;

		float yRayDistance = (y / 2);
		Vector3 firstStartY = new Vector3(-(x / 2), 0, 0) + transform.position;
		Vector3 secondStartY = new Vector3((x / 2), 0, 0) + transform.position;

		Vector3 firstStartX = new Vector3(0, (y / 2), 0) + transform.position;
		Vector3 secondStartX = new Vector3(0, -(y / 2), 0) + transform.position;
		Vector3 firstEnd = new Vector3(Mathf.Sign(axis) * (x / 2), (y / 2), 0) + transform.position;
		Vector3 secondEnd = new Vector3(Mathf.Sign(axis) * (x / 2), -(y / 2), 0) + transform.position;

		Debug.DrawLine(firstStartX, firstEnd, Color.red);
		Debug.DrawLine(secondStartX, secondEnd, Color.red);

		RaycastHit2D[] hitsX = new RaycastHit2D[2];
		hitsX[0] = Physics2D.Raycast(firstStartX, Mathf.Sign(axis) * Vector2.right, x / 2);
		hitsX[1] = Physics2D.Raycast(secondStartX, Mathf.Sign(axis) * Vector2.right, x / 2);

		for (int i = 0; i < hitsX.Length; i++)
		{
			if (hitsX[i].collider != null)
			{
				float distance = Mathf.Abs(hitsX[i].point.x - transform.position.x);
				float moveAmount = (x / 2) - distance;
				Vector3 newPosition = new Vector3(-Mathf.Sign(axis) * moveAmount, 0, 0) + transform.position;
				transform.position = newPosition;
				Grounded = true;
				break;
			}
			Grounded = false;
		}

		RaycastHit2D[] hitsY = new RaycastHit2D[2];
		hitsY[0] = Physics2D.Raycast(firstStartY, -Vector2.up, y / 2);
		hitsY[1] = Physics2D.Raycast(secondStartY, -Vector2.up, y / 2);

		for (int i = 0; i < hitsY.Length; i++)
		{
			if (hitsY[i].collider != null)
			{
				float distance = Mathf.Abs(hitsY[i].point.y - transform.position.y);
				float moveAmount = (y / 2) - distance;
				Vector3 newPosition = new Vector3(0, moveAmount, 0) + transform.position;
				transform.position = newPosition;
				velocity.y = 0;
				Grounded = true;
				break;
			}
			Grounded = false;
		}
	}
}