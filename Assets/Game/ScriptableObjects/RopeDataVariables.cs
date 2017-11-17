using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Rope", order = 1)]
public class RopeDataVariables : ScriptableObject
{
	public float RopeGravity = 0.5f;
	public float RopeSpeedMult = 0.3f;
	public float RopeDamping = 0.995f;
	public float RopeLength = 30;
	public float RopeThrowStartSpeed = 35f;
	public float RopeBouncy = 0.66f;
	public Vector2 RopeHitBox = new Vector2(0.35f, 0.65f);
}