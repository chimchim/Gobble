using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponent : MonoBehaviour
{
	public TileMap.TileType TileType;
	public TileMap.IngredientType IngredientType;
	public int HitsTaken;
	public SpriteRenderer Renderer;
	public int Mod;
	public int X;
	public int Y;
	int current;


	public void SetResource(Game.GameManager game)
	{
		current++;
		if (current == 1)
			Renderer.sprite = game.GameResources.ScriptResources.Crack1;
		if (current == 2)
			Renderer.sprite = game.GameResources.ScriptResources.Crack2;
		if (current == 3)
			Renderer.sprite = game.GameResources.ScriptResources.Crack3;
		if (current == 4)
			Renderer.sprite = game.GameResources.ScriptResources.Crack4;
	}
	public IEnumerator Shake()
	{
		float counter = 0f;
		Vector3 cameraPosition = Renderer.transform.position;
		float maxX = 0.2f;
		float maxY = 0.2f;
		float ShakeTime = 0.2f;
		while (true)
		{
			counter += Time.deltaTime;
			if (counter >= ShakeTime)
			{
				yield break;
			}
			else
			{
				Renderer.transform.position = cameraPosition + new Vector3((ShakeTime - counter) * Random.Range(-maxX, maxX), (ShakeTime - counter) * Random.Range(-maxY, maxY));
			}
			yield return null;
		}
	}
}
