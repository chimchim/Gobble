using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour {

	public bool randomizePosition;


	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	/// <summary>
	/// This will switch the randomize position of instantiated effects on or off
	/// </summary>
	/// <param name="newValue"></param>
	public void RandomToggle(bool newValue)
	{
		if (randomizePosition)
		{
			randomizePosition = false;
        }
        else
        {
            randomizePosition = true;

        }




    }

}
