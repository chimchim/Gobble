using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/ScriptableItems/SpearScriptable", order = 1)]
public class SpearScriptable : ScriptableItem
{
	public bool UseAnimations;
	public float OutTime = 1;
	public float InTime = 1;
	public float Length = 1;
}
