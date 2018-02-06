using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFollowMouse : MonoBehaviour {

	void Update ()
	{
		transform.position = Input.mousePosition + new Vector3(0, 35, 0);
	}
}
