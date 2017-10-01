using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicRope : MonoBehaviour {

	// Use this for initialization
	List<Transform> Ropes;
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	public void DeActivate()
	{
		for (int i = 0; i < Ropes.Count; i++)
		{
			Ropes[i].position = Vector3.zero;
			Ropes[i].localScale = new Vector3(1, 1, 1);
		}
	}
	public void MakeRopes()
	{
		List<Transform> ropes = new List<Transform>();
		var rope = GameObject.Instantiate(UnityEngine.Resources.Load("Prefabs/Rope", typeof(GameObject))) as GameObject;
		ropes.Add(rope.transform);

		for (int i = 0; i < 60; i++)
		{
			var newRope = GameObject.Instantiate(rope);
			//newRope.transform.parent = transform;
			newRope.SetActive(true);
			ropes.Add(newRope.transform);
		}
		Ropes = ropes;
	}
	public int drawIndex = 0;
	public void DrawRope(Vector2[] drawPositions, Vector2 playerPos, int ropeIndex)
	{
		drawIndex = 0;
		for (int i = 0; i < drawPositions.Length - 1; i++)
		{
			Vector2 first = drawPositions[i];
			Vector2 second = drawPositions[i + 1];
			//Debug.DrawLine(first, second, Color.blue);
			Vector2 direction = (second - first).normalized;
			float len = (second - first).magnitude;
			int ropeAmount = (int)(len / 0.51f);
			float extra = len - (0.51f * ropeAmount);
			Vector2 nextPos = first;

			for (int j = 0; j < ropeAmount; j++)
			{
				Ropes[drawIndex].name = second.ToString();
				Ropes[drawIndex].position = first + (direction * 0.51f * j) + (direction * 0.255f);
				nextPos = first + (direction * 0.51f * j);
				Ropes[drawIndex].localScale = new Vector3(1, 1, 1);
				//Ropes[drawIndex].LookAt(second);
				Ropes[drawIndex].right = direction;
				//Ropes[drawIndex].localEulerAngles = new Vector3(Ropes[drawIndex].eulerAngles.x, -90.1f, Ropes[drawIndex].eulerAngles.z);
				drawIndex++;
			}
			Ropes[drawIndex].position = first + (direction * 0.51f * (ropeAmount -1)) + (direction * 0.255f) + (direction * 0.255f) + (direction * extra/2);
			Ropes[drawIndex].name = "jerry";
			Ropes[drawIndex].localScale = new Vector3(extra / 0.51f, Ropes[drawIndex].localScale.y, Ropes[drawIndex].localScale.z);
			Ropes[drawIndex].right = direction;
			//Ropes[drawIndex].LookAt(second);
			//Ropes[drawIndex].localEulerAngles = new Vector3(Ropes[drawIndex].eulerAngles.x, -90.1f, Ropes[drawIndex].eulerAngles.z);
			drawIndex++;
		}
		for (int j = drawIndex; j < Ropes.Count; j++)
		{ 
			Ropes[j].position = Vector3.zero;
		
		}
	}
}
