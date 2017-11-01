using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeArmPositioning : MonoBehaviour
{
	public Transform Character;
	private SpriteRenderer Renderer;
	public Dictionary<string, Vector2> ArmPositions = new Dictionary<string, Vector2>();
	[Serializable]
	public struct ArmPos
	{
		public string name;
		public Vector2 pos;
	}
	public ArmPos[] pos;

	void Start ()
	{
		for (int i = 0; i < pos.Length; i++)
		{
			ArmPositions.Add(pos[i].name, pos[i].pos);
		}

		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.1f);
		Renderer = Character.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		int mult = -1;
		float euler = 0;
		if (Character.eulerAngles.y > 6)
		{
			mult = 1;
			euler = 180;
		}

		var pos = ArmPositions[Renderer.sprite.name];
		pos.x *= mult;
		transform.localPosition = pos;
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.1f);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, euler, transform.eulerAngles.z);
	}
}
