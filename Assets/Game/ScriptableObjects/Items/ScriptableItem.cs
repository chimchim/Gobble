using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/ScriptableItems", order = 1)]
public class ScriptableItem : ScriptableObject
{
	public GameObject Prefab;
	public Sprite Sprite;

}
