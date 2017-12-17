using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GatherLevel = GatherableScriptable.GatherLevel;
public abstract class Gatherable : MonoBehaviour
{
	public TileMap.TileType TileType;
	public TileMap.IngredientType IngredientType;
	public int HitsTaken;
	public int Mod;
	public GatherableScriptable GatherScript;
	public abstract GameObject GetGameObject(Game.GameManager game);
	public abstract void SetResource(Game.GameManager game);
	public abstract bool OnHit(Game.GameManager game, GatherLevel level);

}
