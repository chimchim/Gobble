using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Ground", order = 1)]
public class GroundVariables : ScriptableObject
{
	public float PlayerSpeed = 6;
	public float JumpSpeed = 9;
	public float Gravity = 0.5f;
	public float MaxGravity = 15;
	public float ExtraFallSpeedAfter = 1;
	public float FallDamage = 40;
}