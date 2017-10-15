using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour {

	// Use this for initialization
	public ChooseCharacter[] ChooseCharacters;
	public Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
	public GameObject Characters;
	void Start () {

		var images = Characters.GetComponentsInChildren<ChooseCharacter>();
		ChooseCharacters = GetComponentsInChildren<ChooseCharacter>();
		foreach (ChooseCharacter character in images)
		{
			var img = character.GetComponent<Image>();
			Sprites.Add(img.sprite.name, img.sprite);
		}
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		for (int i = 0; i < ChooseCharacters.Length; i++)
		{
			ChooseCharacters[i].Clicked = false;

			
		}
	}
}
