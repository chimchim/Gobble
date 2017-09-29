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
	public float Weight = 2;
	public float RopeGravity = 0.5f;
	public float RopeSpeedMult = 0.5f;
	public float RopeDamping = 0.995f;
	public float RopeLength = 30;
	public float ForceDamper = 0.99f;

}