using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxingGroup : MonoBehaviour
{

	public Transform Background;
	public Vector2 OffsetBackGround;
	public float Smoothing;

	private float _xSpriteSize;
	private float _ySpriteSize;
	private float _zPosition;

	[Range(0.0f, 1.0f)]
	public float AddedYSpriteMult;
	[Range(0.0f, 1.0f)]
	public float AddedXSpriteMult;
	void Start()
	{
		_zPosition = Background.position.z;
		float fullWidhth = GameUnity.FullWidth * 1.28f;
		float fullHeight = GameUnity.FullHeight * 1.28f;

		var spriteRenderers = Background.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			_xSpriteSize += spriteRenderers[i].sprite.bounds.size.x;
			_ySpriteSize += spriteRenderers[i].sprite.bounds.size.y;
		}
		_xSpriteSize += AddedXSpriteMult * fullWidhth / 2;
		_ySpriteSize += AddedYSpriteMult * fullHeight / 2;
	}

	void LateUpdate()
	{
		float xSize = _xSpriteSize / 2;
		float ySize = _ySpriteSize / 2;
		OffsetBackGround = new Vector2(xSize, ySize);
		float fullWidhth = GameUnity.FullWidth * 1.28f;
		float fullHeight = GameUnity.FullHeight * 1.28f;

		Vector2 midPos = new Vector2((fullWidhth / 2), (fullHeight / 2));
		Vector2 cameraPosition = transform.position;
		Vector2 diff = cameraPosition - midPos;

		float xScale = diff.x / (fullWidhth / 2);
		float yScale = diff.y / (fullHeight / 2);

		float newXPos = xScale * xSize;
		float newYPos = yScale * ySize;

		Background.transform.position = transform.position + new Vector3(-newXPos, -newYPos, 0);
		Background.transform.position = new Vector3(Background.transform.position.x, Background.transform.position.y, _zPosition);

	}
}
