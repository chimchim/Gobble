using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Stats", order = 1)]
public class StatsVariables : ScriptableObject
{
	public float OxygenDPS = 10;
	public float FallDamage = 40;
	public float MaxHP = 100f;
}