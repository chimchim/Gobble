using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnFX : MonoBehaviour {

	public GameObject effect;

	public Transform spawnObject;

	private Vector2 spawnLocation;

	public float timeToLive;

	public bool canLoop;

	private bool effectActive;

	private GameObject loopingEffect;

	private Button theButton;
	private ColorBlock theColour;

   

	// Use this for initialization
	void Start () {


		theColour = GetComponent<Button>().colors;
		theButton = GetComponent<Button>();

		effectActive = false;
	}
	
	// Update is called once per frame
	void Update () {

	  
		
	}


	public void SpawnEffect()
	{
		bool isRandom = FindObjectOfType<SceneControl>().randomizePosition;


		if (isRandom)
		{
			spawnLocation = new Vector2(spawnObject.transform.position.x + Random.Range(-1,1f), spawnObject.transform.position.y + Random.Range(-1,1f));
		}
		else
		{
			spawnLocation = new Vector2(spawnObject.transform.position.x, spawnObject.transform.position.y);
		}
	
	


		if (canLoop)
		{
			if (!effectActive)
			{
				loopingEffect = Instantiate(effect, spawnLocation, spawnObject.rotation);
				effectActive = true;

				theColour.normalColor = Color.green;
				theColour.pressedColor = Color.green;
				theColour.highlightedColor = Color.green;

				theButton.colors = theColour;


			}
			else
			{
				Destroy(loopingEffect);
				effectActive = false;


				theColour.normalColor = Color.white;
				theColour.pressedColor = Color.white;
				theColour.highlightedColor = Color.white;

				theButton.colors = theColour;
			}
		  
		 
		}
		else
		{
			GameObject clone = Instantiate(effect, spawnLocation, spawnObject.rotation);
			Destroy(clone, timeToLive);
		}

		

	}






}
