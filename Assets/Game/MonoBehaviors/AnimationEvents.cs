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
	public Action NotAttackable;
	public void AttackableEvent()
	{
		Attackable.Invoke();
	}
	public void NotAttackableEvent()
	{
		NotAttackable.Invoke();
	}
}
