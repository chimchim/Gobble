using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/ScriptableCharacter", order = 1)]
public class ScriptableCharacter : ScriptableObject
{
	public GameObject Prefab;
	public float Health;
	public float ArmRotation;
	public float JumpSpeed;
	public float MoveSpeed;
	public float Weight;
	public Game.CharacterStats GetStats()
	{
		return new Game.CharacterStats()
		{
			MaxHealth = Health,
			ArmRotationSpeed = ArmRotation,
			JumpSpeed = JumpSpeed,
			MoveSpeed = MoveSpeed,
			Weight = Weight
		};
	}
}