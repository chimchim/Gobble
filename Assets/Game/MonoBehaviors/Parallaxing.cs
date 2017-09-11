using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {

	public Transform BackGround;
	public Vector2 OffsetBackGround;
	public float Smoothing;

	void Start ()
	{
		
	}
	


	void LateUpdate ()
	{
		float xSize = BackGround.GetComponent<SpriteRenderer>().sprite.bounds.size.x * BackGround.localScale.x / 2;
		float ySize = BackGround.GetComponent<SpriteRenderer>().sprite.bounds.size.y * BackGround.localScale.y / 2;
		OffsetBackGround = new Vector2(xSize, ySize);
		int fullWidhth = GameUnity.MapWidth + (GameUnity.WidhtBound * 2);
		int fullHeight = GameUnity.MapHeight + (GameUnity.HeightBound * 2);

		float fullLengthX = fullWidhth * 1.28f;
		float fullLengthY = fullHeight * 1.28f;

		Vector2 midPos = new Vector2((fullLengthX / 2), (fullLengthY / 2));
		Vector2 cameraPosition = transform.position;
		Vector2 diff = cameraPosition - midPos;

		float xScale = diff.x / (fullLengthX / 2);
		float yScale = diff.y / (fullLengthY / 2);

		float newXPos = xScale * xSize + (BackGround.GetComponent<SpriteRenderer>().sprite.bounds.size.x * -xScale / 2);
		float newYPos = yScale * ySize + (BackGround.GetComponent<SpriteRenderer>().sprite.bounds.size.y * -yScale / 2);

		BackGround.transform.position = transform.position + new Vector3(-newXPos, -newYPos, 0);
		BackGround.transform.position = new Vector3(BackGround.transform.position.x, BackGround.transform.position.y, 10);
		


		//for (int i = 0; i < Backgrounds.Length; i++)
		//{
		//
		//}
	}
}
