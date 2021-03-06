﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Game.Actions;
using Game.GEntity;
using Game.Component;
using Game.Systems;

namespace Game.Movement
{
	public class RabbitDig : AnimalState
	{
		float digTimer;
		float timer;
		bool Digged;

		public RabbitDig(int index) : base(index) { }
		public override void EnterState(GameManager game, Animal animal, Entity entity, Player host)
		{
			entity.Animator.SetBool("Dig", true);
			timer = 0;
			digTimer = 0;
		}

		public override void Update(GameManager game, Animal animal, Entity entity, Player host, float delta)
		{
			var position = entity.gameObject.transform.position;
			digTimer += delta;
			if (!Digged && digTimer > game.Animals.RabbitDigTimer)
			{
				Digged = true;
				entity.gameObject.transform.position = new Vector3(-1000, -1000, position.z);
			}
			if (Digged)
			{
				timer -= delta;
				if (timer > 550)
				{
					entity.Animator.SetBool("DigReverse", true);
					if (entity.Animator.GetCurrentAnimatorStateInfo(0).IsName("DigReverse"))
					{
						animal.TransitionState(game, entity, this.GetType(), typeof(RabbitChill), host);
					}
				}
			}
		}

		public override void InnerSerialize(GameManager game, Animal animal, List<byte> byteArray)
		{

		}

		public override void InnerDeSerialize(GameManager game, Animal animal, byte[] byteData, ref int index)
		{

		}

		public override void LeaveState(GameManager game, Animal animal, Entity entity, Player host)
		{
			timer = 0;
			Digged = false;
			entity.Animator.SetBool("DigReverse", false);
			entity.Animator.SetBool("Dig", false);
		}
	}
}
