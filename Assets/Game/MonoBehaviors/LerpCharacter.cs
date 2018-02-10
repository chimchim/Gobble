using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCharacter : MonoBehaviour
{
	float speed = 20;
	void Update ()
	{
		Vector2 diff = transform.root.position - transform.position;
		Vector2 translate = diff.normalized * speed * Time.deltaTime;
		translate = diff.magnitude > translate.magnitude ? translate : diff;
		transform.localPosition += new Vector3(translate.x, translate.y, 0);
		Debug.DrawLine(transform.root.position, transform.position, Color.cyan);
	}

	public void SetLerp(Vector2 from, Vector2 to)
	{
		Vector2 diff = from - to;
		transform.localPosition = diff;
	}
}
