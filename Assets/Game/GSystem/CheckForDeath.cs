using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Actions;

namespace Game.Systems
{
	public class CheckForDeath : ISystem
	{
		private Dictionary<int, float> deathTimer = new Dictionary<int, float>();

		private float deathTime = 5;
        private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<GameObjects, Stats>();
		public void Update(GameManager game)
		{
            var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
            foreach (int entity in entities)
			{
				GameObjects gameObjects = game.Entities.GetComponentOf<GameObjects>(entity);
				Stats stats = game.Entities.GetComponentOf<Stats>(entity);

				
				if (stats.HP <= 0)
				{
                    Debug.Log("DISABLE");

					stats.Alive = false;
                    game.Entities.DisableComponent<Stats>(entity);
                    game.Entities.DisableComponent<Agent>(entity);
                    game.Entities.DisableComponent<Movement>(entity);

                    foreach (Animator a in gameObjects.Head.GetComponentsInChildren<Animator>())
                    {
                        a.SetTrigger("headshot");
                    }
                    //game.Entities.DisableComponent<>(entity);
                    //gameObjects.Aim.GetComponent<RotateArm>().UpdateShoulder();
                    //gameObjects.Head.GetComponent<EnableRagdoll>().EnableRagdoller();
                    //gameObjects.Head.GetComponent<BoxCollider>().enabled = false;
                    //gameObjects.Head.GetComponent<CharacterController>().enabled = false;
                    //gameObjects.Collider.GetComponent<BoxCollider2D>().enabled = false;
                    //gameObjects.Head.transform.GetChild(2).gameObject.SetActive(false);
                    GameObject.Destroy(gameObjects.Collider.gameObject);
                }

				//if (!deathTimer.ContainsKey(entity) && !stats.Alive)
				//{
				//	deathTimer.Add(entity, deathTime);
				//}
				//if (deathTimer.ContainsKey(entity))
				//{
				//	deathTimer[entity] -= Time.deltaTime;
                //
				//	if (deathTimer[entity] <= 0)
				//	{
				//		gameObjects.Head.GetComponent<EnableRagdoll>().DisableRagdoll();
				//		stats.Alive = true;
				//		stats.HP = 100;
				//		deathTimer.Remove(entity);
				//		gameObjects.Head.GetComponent<BoxCollider>().enabled = true;
				//		gameObjects.Head.transform.GetChild(2).gameObject.SetActive(true);
				//		gameObjects.Collider.GetComponent<BoxCollider2D>().enabled = true;
				//		gameObjects.Head.GetComponent<CharacterController>().enabled = true;
				//	}
				//}
			}
		}

        public void Initiate(GameManager game)
		{
		}
        public void SendMessage(GameManager game, int reciever, Message message)
        {

        }
	}
}
