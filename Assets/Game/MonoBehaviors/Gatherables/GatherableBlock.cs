using System;
using Game;
using UnityEngine;
using System.Collections;

namespace Gatherables
{
	public class GatherableBlock : Gatherable
	{
		public SpriteRenderer Renderer;
		public int X;
		public int Y;
		int current;
		public override GameObject GetGameObject(GameManager game)
		{
			return game.TileMap.Blocks[X, Y];
		}

		public override void OnHit()
		{
			if (Renderer == null)
			{
				StartCoroutine(ShakeMain());
				return;
			}
			StartCoroutine(Shake());
		}
		public override void SetResource(GameManager game)
		{
			if (Renderer == null)
				return;
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
					Renderer.transform.position = cameraPosition + new Vector3((ShakeTime - counter) * UnityEngine.Random.Range(-maxX, maxX), (ShakeTime - counter) * UnityEngine.Random.Range(-maxY, maxY));
				}
				yield return null;
			}
		}
		public IEnumerator ShakeMain()
		{
			float counter = 0f;
			Vector3 cameraPosition = transform.position;
			float maxX = 0.3f;
			float maxY = 0.3f;
			float ShakeTime = 0.2f;
			while (true)
			{
				counter += Time.deltaTime;
				if (counter >= ShakeTime)
				{
					transform.position = cameraPosition;
					yield break;
				}
				else
				{
					transform.position = cameraPosition + new Vector3((ShakeTime - counter) * UnityEngine.Random.Range(-maxX, maxX), (ShakeTime - counter) * UnityEngine.Random.Range(-maxY, maxY));
				}
				yield return null;
			}
		}
	}
}
