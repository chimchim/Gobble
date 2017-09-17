using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {

	public Transform Background;
	public Vector2 OffsetBackGround;
	public float Smoothing;

	private float _zPosition;
	void Start ()
	{
		_zPosition = Background.position.z;
	}
	
	void LateUpdate ()
	{
		float xSize = Background.GetComponent<SpriteRenderer>().sprite.bounds.size.x * Background.localScale.x / 2;
		float ySize = Background.GetComponent<SpriteRenderer>().sprite.bounds.size.y * Background.localScale.y / 2;
		OffsetBackGround = new Vector2(xSize, ySize);

		float fullWidhth = GameUnity.FullWidth * 1.28f;
		float fullHeight = GameUnity.FullHeight * 1.28f;

		Vector2 midPos = new Vector2((fullWidhth / 2), (fullHeight / 2));
		Vector2 cameraPosition = transform.position;
		Vector2 diff = cameraPosition - midPos;

		float xScale = diff.x / (fullWidhth / 2);
		float yScale = diff.y / (fullHeight / 2);

		float newXPos = xScale * xSize + (Background.GetComponent<SpriteRenderer>().sprite.bounds.size.x * -xScale / 2);
		float newYPos = yScale * ySize + (Background.GetComponent<SpriteRenderer>().sprite.bounds.size.y * -yScale / 2);

		Background.transform.position = transform.position + new Vector3(-newXPos, -newYPos, 0);
		Background.transform.position = new Vector3(Background.transform.position.x, Background.transform.position.y, _zPosition);
		
	}
}
