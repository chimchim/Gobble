using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniMapRaycaster : MonoBehaviour, IPointerClickHandler
{

	// Use this for initialization
	public float DoubleClickTime = 0.4f;
	public float LerpTime = 0.7f;
	public float MaxSize = 52f;
	public float MinSize = 16f;
	public float ZoomSize = 6;
	private MiniMap _miniMap;
	private Camera _miniMapCamera;

	private float _lastClickTime;
	private PointerEventData.InputButton _lastButton;

	private float _lerpTime;
	private float _startSize;
	
	void Start ()
	{
		_miniMap = FindObjectOfType<MiniMap>();
		_miniMapCamera = _miniMap.GetComponent<Camera>();
		_lastClickTime = Time.time;
		_startSize = _miniMapCamera.orthographicSize;
	}
	

	IEnumerator ZoomOut()
	{
		while (_lerpTime < LerpTime)
		{
			_lerpTime += Time.deltaTime;
			float size = Mathf.Lerp(0, ZoomSize, _lerpTime / LerpTime) + _startSize;
			_miniMapCamera.orthographicSize = Mathf.Clamp(size, MinSize, MaxSize);
			yield return null;
		}
		_startSize = _miniMapCamera.orthographicSize;
		_lerpTime = 0;
	}

	IEnumerator ZoomIn()
	{
		while (_lerpTime < LerpTime)
		{
			_lerpTime += Time.deltaTime;
			float size = Mathf.Lerp(0, -ZoomSize, _lerpTime / LerpTime) + _startSize;
			_miniMapCamera.orthographicSize = Mathf.Clamp(size, MinSize, MaxSize);
			yield return null;
		}
		_startSize = _miniMapCamera.orthographicSize;
		_lerpTime = 0;
	}

	public void OnPointerClick(PointerEventData eventData) // 3
	{
		
		if ((_lastClickTime + DoubleClickTime) > Time.time)
		{
			if (eventData.button == _lastButton)
			{
				if (eventData.button == PointerEventData.InputButton.Left && _miniMapCamera.orthographicSize < MaxSize)
				{
					StartCoroutine(ZoomOut());
				}
				if (eventData.button == PointerEventData.InputButton.Right && _miniMapCamera.orthographicSize > MinSize)
				{
					StartCoroutine(ZoomIn());
				}
			}
		}
		_lastButton = eventData.button;
		_lastClickTime = Time.time;
	}

}
