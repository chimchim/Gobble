using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Variables/Minerals", order = 1)]
public class MineralsGenVariables : ScriptableObject
{
	//Rock
	public int RockMiddleOneIn = 6;

	//Gold
	public int GoldRandomOneIn = 6;
	public int GoldBotOnIn = 6;
	public int GoldLeftRightBot = 6;
	public int GoldChanceIslandLimit = 32;
	public int MaxExtraGoldChance = 3;

	//Iron
	public int IronRandomOneIn = 6;
	public int IronMiddleOnIn = 10;
	public int IronlevelChanceIncrease = 2;

	public int CopperRandomOneIn = 6;
	public int CopperSideOneIn = 10;

	public int NormalMod = 2;
	public int RocksMod = 3;
	public int CopperMod = 3;
	public int IronMod = 4;
	public int GoldMod = 5;
}