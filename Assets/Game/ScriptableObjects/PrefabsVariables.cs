using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Prefabs", order = 1)]
public class PrefabsVariables : ScriptableObject
{
	[Header("Characters ")]
	public GameObject Peppermin;
	public GameObject Schmillo;
	public GameObject Milton;
	public GameObject Yolanda;

	[Header("Animals")]
	public GameObject Rabbit;

	public GameObject SpriteDiffuse;
	public GameObject Poof;
	public GameObject Ladder;

	[Header("Effects")]
	public GameObject[] Effects;

	[Header("TreeTile ")]
	public GameObject Level1_1;
	public GameObject Level1_2;
	public GameObject Level1_3;
	public GameObject Level1_4;
	public GameObject Level2_1;
	public GameObject Level2_2;
	public GameObject Level3_1;
	public GameObject Level3_2;
	public GameObject Level4_1;
	public GameObject Level4_2;
	public GameObject Level5_1;
	public GameObject Level5_2;
	public GameObject Level5_3;
	public GameObject Level5_4;
	public GameObject TwigLeft;
	public GameObject TwigRight;
	public GameObject TwigMiddle;


}