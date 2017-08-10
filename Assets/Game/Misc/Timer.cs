using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{

	// Use this for initialization
	public float T = 0.1f;
	public float calcTimer;

	void Start()
	{
		calcTimer = 0.1f;
	}

	// Update is called once per frame
	void Update()
	{
		calcTimer -= Time.deltaTime;
		if (calcTimer < T)
		{
			gameObject.SetActive(false);
		}
	}

	public void Reset()
	{
		calcTimer = T;
	}
}
