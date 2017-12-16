using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gatherable : MonoBehaviour
{
	public TileMap.TileType TileType;
	public TileMap.IngredientType IngredientType;
	public int HitsTaken;
	public int Mod;

	public abstract GameObject GetGameObject(Game.GameManager game);
	public abstract void SetResource(Game.GameManager game);
	//{
	//	if (Renderer == null)
	//		return;
	//	current++;
	//	if (current == 1)
	//		Renderer.sprite = game.GameResources.ScriptResources.Crack1;
	//	if (current == 2)
	//		Renderer.sprite = game.GameResources.ScriptResources.Crack2;
	//	if (current == 3)
	//		Renderer.sprite = game.GameResources.ScriptResources.Crack3;
	//	if (current == 4)
	//		Renderer.sprite = game.GameResources.ScriptResources.Crack4;
	//}
	public abstract void OnHit();
	//{
	//	if (Renderer == null)
	//		StartCoroutine(ShakeMain());
	//	else
	//		StartCoroutine(Shake());
	//}
}
