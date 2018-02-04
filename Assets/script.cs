using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script : MonoBehaviour
{
	Vector2 CurrentVelocity;

	private void Start()
	{
		int[] ints = new int[]
			{
				3, 5, 9, 5, 3, 9, 7, 9, 9
				//0, 0, 0, -1, -1, 1, 1, 1, -1, -1, -1
			};
		Debug.Log(solution(ints));
	}
	public int solution(int[] A)
	{
		int ret = 0;

		for (int i = 0; i < A.Length; ++i)
		{
			ret = Mathf.Abs(A[i] - ret);
		}

		return ret;
	}

	int GetSlice(int[] A, int index, ref int newIndex)
	{
		int iterations = 1;
		int sum = A[index];
		index++;
		while (true)
		{
			if (index < A.Length && (sum + A[index]) >= 0)
			{
				iterations++;
				sum += A[index];
				index++;
			}
			else
			{
				index++;
				newIndex = index;
				break;
			}
		}
		return iterations;
	}
	int recursive(int[] A, int leftIndex, int rightIndex, int current, int leftSlice, int rightSlice)
	{
		//int leftSlice = current;
		//int rightSlice = current;
		int tempcurrent = current;
		if (leftIndex > -1)
			leftSlice += A[leftIndex];
		else
			leftSlice += -100;

		if (rightIndex < A.Length)
			rightSlice += A[rightIndex];
		else
			rightSlice += -100;

		if (leftSlice == rightSlice && leftSlice >= 0)
			tempcurrent += recursive(A, leftIndex - 1, rightIndex + 1, leftSlice, leftSlice, rightSlice);
		if(leftSlice > rightSlice && leftSlice >= 0)
			tempcurrent += recursive(A, leftIndex - 1, rightIndex, leftSlice, leftSlice, rightSlice);
		if (rightSlice > leftSlice && rightSlice >= 0)
			tempcurrent += recursive(A, leftIndex, rightIndex + 1, rightSlice, leftSlice, rightSlice);

		return tempcurrent;
	}
	public void StartTraverse(int[] A, int index)
	{
		int leftSlice = 1;
		int rightSlice = 1;
		int leftIndex = index - 1;
		int rightIndex = index + 1;
		while (true)
		{
			if (leftIndex > -1)
				leftSlice += A[leftIndex];
			else
				leftSlice += -100;

			if (rightIndex < A.Length)
				leftSlice += A[rightIndex];
			else
				rightSlice += -100;


		}
	}
	void FixedUpdate ()
	{
		return;
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
