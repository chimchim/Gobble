using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxingObject : MonoBehaviour
{

	public Transform Background;
	public Vector2 OffsetBackGround;
	public float Smoothing;

	private float _zPosition;
	public float MaxY;
	public float MinY;

	public float MaxX;
	public float MinX;
	void Start()
	{
		_zPosition = Background.position.z;
	}

	void LateUpdate()
	{
		float fullWidhth = (GameUnity.MapWidth + (GameUnity.WidhtBound * 2)) * 1.28f;
		float fullHeight = (GameUnity.MapHeight + (GameUnity.HeightBound * 2)) * 1.28f;
		Vector2 midPos = new Vector2((fullWidhth / 2), (fullHeight / 2));
		Vector2 cameraPosition = transform.position;

		Vector2 diff = cameraPosition - midPos;
		
		float yPercent = cameraPosition.y / (fullHeight);
		float rangeY = MaxY - MinY;
		float valueY = (rangeY * yPercent) + MinY;

		float xPercent = cameraPosition.x / (fullWidhth);
		float rangeX = MaxX - MinX;
		float valueX = (rangeX * xPercent) + MinX;
		
		Background.transform.localPosition = new Vector3(valueX, valueY, _zPosition);
		Background.transform.position = new Vector3(Background.transform.position.x, Background.transform.position.y, _zPosition);

	}
}
