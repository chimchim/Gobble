using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Helper
{
	private static LayerMask itemLayer = LayerMask.GetMask("Item");
	private const float DegToRad = 3.14f / 180f;
	public static Vector2 RotateTowards(this Vector2 from, Vector2 to, float degrees)
	{
		float angle = Vector2.Angle(from, to);
		if (Math.Abs(degrees) > angle)
			degrees = angle * Math.Sign(degrees);
		float firstDot = Vector2.Dot(from, to);
		Vector2 rotated = Rotate(from, degrees);
		float secondDot = Vector2.Dot(rotated, to);
		if (secondDot > firstDot)
			return rotated;
		return Rotate(from, -degrees);
	}
	public static Vector2 Rotate(this Vector2 v, float degrees)
	{
		return RotateRadians(v, degrees * DegToRad);
	}

	public static Vector2 RotateRadians(this Vector2 v, float radians)
	{
		var ca = (float)Math.Cos(radians);
		var sa = (float)Math.Sin(radians);
		return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
	}

	public static void CheckItemPickup(GameManager game, int e)
	{ 
		Vector2 pos = game.Entities.GetEntity(e).gameObject.transform.position;
		pos += new Vector2(0, 0.5f);
		for (int i = -1; i < 2; i++)
		{
			pos += new Vector2(0.2f*i, 0);
			var hit = Physics2D.Raycast(pos, -Vector2.up, 1.2f, itemLayer);
			if (hit.transform != null)
			{
				var visible = hit.transform.GetComponent<VisibleItem>();
				visible.TryPick(e);
			}
		}
	}
}

//Vector2 segDir = seg.Pos1 - seg.Pos2;
//Vector2 segNormal = Vector3.Cross(Vector3.forward, new Vector3(segDir.x, segDir.y, 0)).normalized;
//float angleB = 90 - getAngle(bl, segDir.magnitude, cl);
//var bPos = Rotate(segNormal, angleB + 90) * cl + seg.Pos1;
//
//float angleB2 = 90 - getAngle(cl, segDir.magnitude, bl);
//var bPos2 = Rotate(segNormal, angleB + 90) * bl + seg.Pos1;
//bPos = bPos.x > bPos2.x? bPos : bPos2;
//
//		Segment segb = new Segment { Pos1 = seg.Pos1, Pos2 = bPos };
//Segment segc = new Segment { Pos1 = seg.Pos2, Pos2 = bPos };
//segments.Add(segb);
//		segments.Add(segc);