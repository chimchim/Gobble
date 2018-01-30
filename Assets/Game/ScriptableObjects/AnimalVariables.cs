using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Data", menuName = "Variables/AnimalVariables", order = 1)]
public class AnimalVariables : ScriptableObject
{
	[Header("Rabbit")]
	public float FleeDistance;
	public float RabbitDigTimer = 1.5f;
	public float RabbitSpeed = 3;
	public float RabbitChillTimer = 3;
	public float RabbitAggro = 3;

}