using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/GatherableScriptable", order = 1)]
public class GatherableScriptable : ScriptableObject
{
	public enum GatherLevel
	{
		Hands,
		Pickaxe
	}
	public GatherLevel Level;
	public int HitsNeeded;
}
