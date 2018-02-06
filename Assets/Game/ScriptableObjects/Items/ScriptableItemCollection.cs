using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/ScriptableItems/ScriptableItemCollection", order = 1)]
public class ScriptableItemCollection : ScriptableItem
{
	[SerializeField]
	public ScriptableItem[] Collection;

}
