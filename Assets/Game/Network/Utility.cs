using Game;
using Game.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{

	private const float DegToRad = Mathf.PI / 180;

	public static Vector2 Rotate(Vector2 v, float degrees)
	{
		return RotateRadians(v, degrees * DegToRad);
	}

	public static Vector2 RotateRadians(Vector2 v, float radians)
	{
		var ca = Mathf.Cos(radians);
		var sa = Mathf.Sin(radians);
		return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
	}

	public static int IsHost(Game.GameManager game)
	{

		var players = game.Entities.GetEntitiesWithComponents(Bitmask.MakeFromComponents<Player>());
		foreach (int p in players)
		{
			var player = game.Entities.GetComponentOf<Player>(p);
			if (player.Owner && player.IsHost)
				return p;
		}
		return -1;
	}
}
