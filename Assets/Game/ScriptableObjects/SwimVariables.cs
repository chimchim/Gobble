using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Swim", order = 1)]
public class SwimVariables : ScriptableObject
{
	public  float WaterJumpSpeed = 10.0f;
	public  float WaterGravity = 0.3f;
	public  float SwimUpExtraSpeed = 0.5f;
	public  float SwimDownMult = 0.77f;
	public  float MaxWaterSpeed = 2.0f;
	public  float SwimSpeed = 0.6f;
	public  float OxygenTime = 12;
	public  float LoseOxygenAfter = 0.5f;
}