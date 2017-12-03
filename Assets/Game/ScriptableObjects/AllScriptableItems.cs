using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/AllScriptableItems", order = 1)]
public class AllScriptableItems : ScriptableObject
{
	public PickAxeScriptable PickAxe;
	public RopeScriptable Rope;
}