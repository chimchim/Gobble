using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Data", menuName = "Variables/RealTimeVariables", order = 1)]
public class RealTimeVariables : ScriptableObject
{
	public bool HoveringUI;
	public bool ChangingItem;
	public ItemImage CurrentSwitch;
	public ItemImage CurrentSwitch2;
}