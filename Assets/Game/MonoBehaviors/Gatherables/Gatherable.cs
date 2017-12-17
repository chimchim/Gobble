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

	public virtual Vector2 GetForce()
	{
		float force = Random.Range(GatherScript.MinForce, GatherScript.MaxForce);
		float randomnedAngle = Random.Range(-GatherScript.RandomAngle/2, GatherScript.RandomAngle / 2);
		var vec = Utility.Rotate(GatherScript.ForceVectorDir, randomnedAngle) * force;
		Debug.Log("return vec " + vec);
		return vec;
	}

}
