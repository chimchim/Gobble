using UnityEngine;
using System.Collections;
using LayerMaskEnum = Game.Systems.Movement.LayerMaskEnum;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/LayerMasksVariables", order = 1)]
public class LayerMasksVariables : ScriptableObject
{
	public LayerMaskEnum Enums;
	public MappedMasks[] MappedMasks;

}
[Serializable]
public struct MappedMasks
{
	public LayerMaskEnum Mask;
	public LayerMask[] UpLayers;
	public LayerMask[] DownLayers;
}