using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildConsole : MonoBehaviour
{
	public Text LogTemplate;
	public Text ExceptionTemplate;
	public int MaxLogs = 15;
	[HideInInspector]
	public List<Text> Texts = new List<Text>();
	[HideInInspector]
	public int LogIndex;
	private void Update()
	{
		var wheel = Input.GetAxis("Mouse ScrollWheel");
		Debug.Log("wheel " + wheel);
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
		Texts.Add(text);
		text.gameObject.SetActive(true);
		if (Texts.Count > MaxLogs)
		{
			Texts[Texts.Count - (MaxLogs - 1)].gameObject.SetActive(false);
		}
	}
}