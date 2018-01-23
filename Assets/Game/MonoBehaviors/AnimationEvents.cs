using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
	public Action OnArmHit;
	public void ArmHitEvent()
	{
		OnArmHit.Invoke();
	}
	public Action Attackable;
	public void AttackableEvent()
	{
		Attackable.Invoke();
	}
}
