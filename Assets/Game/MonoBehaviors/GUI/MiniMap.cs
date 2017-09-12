using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour
{

	public GameObject player;

	// Use this for initialization
	void Start()
	{

	}


	void LateUpdate()
	{

		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -12);
	}
}