using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
	public Transform Transform1;
	public Transform Transform2;
	public Transform Transform3;
	public Transform Transform4;
	public Action OnArmHit;
	public void ArmHitEvent()
	{
		OnArmHit.Invoke();
	}
	public Action Attackable;
	public Action NotAttackable;
	public void AttackableEvent()
	{
		if(Attackable != null)
			Attackable.Invoke();
	}
	public void NotAttackableEvent()
	{
		if (NotAttackable != null)
			NotAttackable.Invoke();
	}
}
