using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildConsole : MonoBehaviour
{
	public Text LogTemplate;
	public Text ExceptionTemplate;
	public int MaxLogs = 15;

	private List<Text> _texts = new List<Text>();

	public int _logIndex;
	private bool _isScrolled;
	private void Update()
	{
		var wheel = Input.GetAxis("Mouse ScrollWheel");
		if (wheel != 0 && _texts.Count > MaxLogs)
		{
			_isScrolled = true;
			int wheelValue = (int)(wheel * 10);

			if (wheelValue > 0 && _logIndex > MaxLogs -1)
			{
				for (int i = _logIndex - 1; i >= _logIndex - wheelValue; i--)
				{	if (i < _texts.Count)
						break;
					_texts[i].gameObject.SetActive(false);
				}
			}

			if (wheelValue < 0)
			{
				for (int i = _logIndex - MaxLogs; i < (_logIndex - MaxLogs) - wheelValue; i++)
				{
					if (i+MaxLogs >= _texts.Count)
					{
						_isScrolled = false;
                        break;
					}
					_texts[i].gameObject.SetActive(false);
				}
			}
			_logIndex -= wheelValue;
		}
		_logIndex = Mathf.Clamp(_logIndex, MaxLogs, _texts.Count - 1);
		if(!_isScrolled)
			_logIndex = _texts.Count;
		if (_isScrolled && _logIndex > MaxLogs)
		{
			for (int i = _logIndex -1; i > (_logIndex - MaxLogs); i--)
			{
				_texts[i].gameObject.SetActive(true);
            }
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			for (int i = 0; i < _texts.Count; i++)
			{
				GameLobby.Destroy(_texts[i].gameObject);
			}
			_texts.Clear();
			_isScrolled = false;
			
		}
	}
	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}
	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}
	void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Warning || type == LogType.Error || type == LogType.Assert)
			return;

		Text text = null;
		if (type == LogType.Log)
		{
			text = Instantiate(LogTemplate) as Text;
		}
		if (type == LogType.Exception)
		{
			text = Instantiate(ExceptionTemplate) as Text;
		}
		
		text.transform.parent = transform;
		text.text = logString;
		_texts.Add(text);

		if (!_isScrolled)
		{
			_logIndex = _texts.Count;
		}
		text.gameObject.SetActive(!_isScrolled);
		
		if (_texts.Count > MaxLogs)
		{
			_texts[_texts.Count - (MaxLogs + 1)].gameObject.SetActive(false);
		}
	}
}