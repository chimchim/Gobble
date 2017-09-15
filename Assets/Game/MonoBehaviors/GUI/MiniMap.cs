using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour
{

	public GameObject player;
	public Shader SpriteDefault;
	// Use this for initialization
	void Start()
	{
		var cam = GetComponent<Camera>();
		//cam.SetReplacementShader(SpriteDefault, "Transparent");
	}


	void LateUpdate()
	{

		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -12);
	}
}