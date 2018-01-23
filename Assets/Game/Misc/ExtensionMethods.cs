using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Helper
{
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
}

