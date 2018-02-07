using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/CharacterScriptable", order = 1)]
public class CharacterScriptable : ScriptableObject
{
	public float Health;
	public float ArmRotation;
	public float JumpSpeed;
	public float MoveSpeed;

	public Game.CharacterStats GetStats()
	{
		return new Game.CharacterStats()
		{
			MaxHealth = Health,
			ArmRotationSpeed = ArmRotation,
			JumpSpeed = JumpSpeed,
			MoveSpeed = MoveSpeed
		};
	}
}