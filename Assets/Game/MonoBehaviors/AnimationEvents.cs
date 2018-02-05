using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
	public Transform Transform1;
	public Transform Transform2;
	public Transform Transform3;
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
