using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script : MonoBehaviour
{
	Vector2 CurrentVelocity;

	void FixedUpdate ()
	{

		float x = UnityEngine.Input.GetAxis("Horizontal");
		float y = UnityEngine.Input.GetAxis("Vertical");
		CurrentVelocity.y += -0.5f;
		CurrentVelocity.x = x * 6;
		if (Input.GetKeyDown(KeyCode.Space))
			CurrentVelocity.y = 10;
		float yMovement = CurrentVelocity.y * Time.fixedDeltaTime;
		float xMovement = CurrentVelocity.x * Time.fixedDeltaTime;
		var yDir = new Vector2(0, CurrentVelocity.y).normalized;
		var xDir = new Vector2(0, CurrentVelocity.x).normalized;
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");

		Vector2 fromPos = transform.position;
		Vector3 tempPos = fromPos;
		tempPos = Vertical2(tempPos, yMovement);
		tempPos = Horizontal2(tempPos, xMovement);
		transform.position = tempPos;

	}
	public Vector2 Horizontal2(Vector2 fromPos, float xMove)
	{
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");
		var hit = Physics2D.CircleCast(fromPos, 0.5f, new Vector2(xMove, 0), Mathf.Abs(xMove), layerMask);
		var translate = new Vector2(xMove, 0);
		Vector2 toPos = fromPos + translate;

		Vector2 currentPos = fromPos;

		if (hit.transform != null)
		{
			Debug.Log("Hit x ");
			Debug.DrawLine(fromPos, hit.centroid, Color.blue);
			Debug.DrawLine(hit.point, hit.centroid, Color.green);
			Debug.DrawLine(hit.point, fromPos, Color.red);
			var diff = toPos - hit.point;
			float dot = Vector2.Dot(-diff.normalized, translate.normalized);
			//Debug.DrawLine(fromPos, fromPos + (translate * 112), Color.white);
			if (dot > 0)
				translate = new Vector2(0, diff.y) * 0.05f;
			Debug.DrawLine(fromPos, fromPos + (translate * 112), Color.white);
			return hit.centroid + new Vector2(0.01f * -Mathf.Sign(xMove), 0) + translate;

		}
		return currentPos + translate;
	}

	public Vector2 Vertical2(Vector2 fromPos, float yMove)
	{
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");
		var hit = Physics2D.CircleCast(fromPos, 0.5f, new Vector2(0, yMove), Mathf.Abs(yMove), layerMask);
		var translate = new Vector2(0, yMove);
		Vector2 toPos = fromPos + translate;

		Vector2 currentPos = fromPos;
		Vector2 colTranslate = Vector2.zero;

		if (hit.transform != null)
		{
			
			//Debug.DrawLine(fromPos, hit.centroid, Color.blue);
			//Debug.DrawLine(hit.point, hit.centroid, Color.green);
			//Debug.DrawLine(hit.point, fromPos, Color.red);
			var diff = toPos - hit.point;
			float dot = Vector2.Dot(-diff.normalized, translate.normalized);

			if (dot > 0)
				CurrentVelocity.y *= (1 - dot);
			if (dot > 0)
				translate = new Vector2(diff.x, 0) * 0.01f;
			return hit.centroid + new Vector2(0, 0.01f * -Mathf.Sign(yMove)) + translate;
		}
		
		return currentPos + translate;
	}
	public Vector2 Both(Vector2 fromPos, float xMove, float yMove)
	{
		var firstMove = new Vector2(xMove, yMove);
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");
		var hits = Physics2D.CircleCastAll(fromPos, 0.5f, firstMove.normalized, firstMove.magnitude, layerMask);
		var translate = firstMove;
		Vector2 toPos = fromPos + translate;
		Debug.Log("firstMove " + firstMove);
		Vector2 currentPos = fromPos;
		Vector2 colTranslate = Vector2.zero;
		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].transform != null)
			{
				Debug.DrawLine(fromPos, hits[i].centroid, Color.blue);
				Debug.DrawLine(hits[i].point, hits[i].centroid, Color.green);
				Debug.DrawLine(hits[i].point, fromPos, Color.red);
				var diff = toPos - hits[i].point;
				var diffL = 0.5f - diff.magnitude;
				var modTranslate = diffL * diff.normalized;
				float dot = Vector2.Dot(-diff.normalized, translate.normalized);
				Debug.Log("Dot " + dot);

				translate = modTranslate;
				CurrentVelocity.x = 0;
				//UnityEditor.EditorApplication.isPaused = true;
				//}
			}
			colTranslate += translate;
		}
		if (colTranslate.magnitude > 0)
			translate = colTranslate;
		return currentPos + translate;
	}
	public Vector2 Horizontal(Vector2 fromPos, float xMove)
	{
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");
		var hits = Physics2D.CircleCastAll(fromPos, 0.5f, new Vector2(xMove, 0), Mathf.Abs(xMove), layerMask);
		var translate = new Vector2(xMove, 0);
		Vector2 toPos = fromPos + translate;

		Vector2 currentPos = fromPos;
		Vector2 colTranslate = Vector2.zero;
		if (hits.Length > 1)
			return fromPos;
		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].transform != null)
			{
				Debug.DrawLine(fromPos, hits[i].centroid, Color.blue);
				Debug.DrawLine(hits[i].point, hits[i].centroid, Color.green);
				Debug.DrawLine(hits[i].point, fromPos, Color.red);
				var diff = toPos - hits[i].point;
				var diffL = 0.5f - diff.magnitude;
				var modTranslate = diffL * diff.normalized;
				modTranslate.x += xMove;
				translate = modTranslate;

			}
			colTranslate += translate;
		}
		if (colTranslate.magnitude > 0)
			translate = colTranslate;
		return currentPos + translate;
	}

	public Vector2 Vertical(Vector2 fromPos, float yMove)
	{
		var layerMask = 1 << LayerMask.NameToLayer("Collideable");
		var hits = Physics2D.CircleCastAll(fromPos, 0.5f, new Vector2(0, yMove), Mathf.Abs(yMove), layerMask);
		var translate = new Vector2(0, yMove);
		Vector2 toPos = fromPos + translate;

		Vector2 currentPos = fromPos;
		Vector2 colTranslate = Vector2.zero;

		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].transform != null)
			{
				//Debug.DrawLine(fromPos, hits[i].centroid, Color.blue);
				//Debug.DrawLine(hits[i].point, hits[i].centroid, Color.green);
				//Debug.DrawLine(hits[i].point, fromPos, Color.red);
				var diff = toPos - hits[i].point;
				var diffL = 0.5f - diff.magnitude;
				var modTranslate = diffL * diff.normalized;	
				float dot = Vector2.Dot(-diff.normalized, translate.normalized);
				modTranslate.y += yMove;
				translate = modTranslate;
				if(dot > 0)
					CurrentVelocity.y *= (1- dot);


			}
			colTranslate += translate;
		}
		if (colTranslate.magnitude > 0)
			translate = colTranslate;
		return currentPos + translate;
	}
}
