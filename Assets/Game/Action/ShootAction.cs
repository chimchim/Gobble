using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Game.Component;
namespace Game.Actions
{
    public class ShootAction : Action
    {
        private static ObjectPool<ShootAction> _pool = new ObjectPool<ShootAction>(100);

        public ShootAction()
        {

        }

        public static ShootAction Make()
        {
            ShootAction shootAction = _pool.GetNext();
            return shootAction;
        }

        public override void Apply(GameManager game, int owner)
        {

            GameObjects gameObjects = game.Entities.GetComponentOf<GameObjects>(owner);
			foreach (Animator a in gameObjects.Head.GetComponentsInChildren<Animator>())
			{
				a.SetTrigger("shoot");
			}
			//return;
            Transform shooter = gameObjects.Barrel.transform;
			//Transform muzzler = shooter.GetChild(0);

			//muzzler.gameObject.SetActive(true);
			//muzzler.GetComponent<Timer>().Reset();
			
			//AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/gunshot"), muzzler.position);

            Vector2 rayPosition = new Vector2(shooter.position.x, shooter.position.y + (shooter.position.z * (19.2f / 90f)));
            Vector2 rayDirection = new Vector2(-shooter.forward.x, -shooter.forward.y);

            RaycastHit2D[] hited2D = Physics2D.RaycastAll(rayPosition, rayDirection).OrderBy(h => h.distance).ToArray();
            RaycastHit hit3D;

			for (int i = 0; i < hited2D.Length; i++)
            {
				if (hited2D[i].transform != gameObjects.Collider.transform)
				{

					int id = hited2D[i].transform.GetComponent<EntityID>().ID;
					Transform hittedTransform = game.Entities.GetComponentOf<GameObjects>(id).Head.transform;

					float y = hited2D[i].point.y - (hittedTransform.position.z * (19.2f / 90f));
					float z = hittedTransform.position.z;
					Vector3 direction = new Vector3(hited2D[i].point.x, y, z) - shooter.position;

					Debug.DrawLine(shooter.position, direction * 100, Color.blue);
                    bool breakFull = false;
					RaycastHit[] hited3D = Physics.RaycastAll(shooter.position, direction.normalized).OrderBy(h => h.distance).ToArray();
					for (int j = 0; j < hited3D.Length; j++)
					{
						if (hited3D[j].transform != gameObjects.Head.transform)
						{
                            float f = Mathf.Abs(hited3D[j].point.y - hited3D[j].transform.position.y);
                            if (hittedTransform == hited3D[j].transform)
                            {
                                if ((1.6f - f) < 0.3f)
                                {
                                    game.Entities.GetComponentOf<Stats>(id).HP -= 100;
                                }
                                else
                                {
                                    game.Entities.GetComponentOf<Stats>(id).HP -= (f / 1.3f) * 100;
                                }
                            }
                            SpawnParticle(game, hited3D[j].point);
							i = hited2D.Length;
                            breakFull = true;
                            break;
						}
					}
                    if (breakFull)
                        break;
				}
				else if (hited2D.Length == 1)
				{
					Vector3 missedDirection = new Vector3(-shooter.forward.x, -shooter.forward.y, 0);
					if (Physics.Raycast(shooter.position, missedDirection.normalized, out hit3D, 100.0F))
					{
						//Debug.Log(hited2D[0].collider.name + " as " + hit3D.collider.name);
						SpawnParticle(game, hit3D.point);
					}
				}
            }
			if (hited2D.Length == 0)
			{
				Vector3 missedDirection = new Vector3(-shooter.forward.x, -shooter.forward.y, 0);
				if (Physics.Raycast(shooter.position, missedDirection.normalized, out hit3D, 100.0F))
			    {
					//Debug.Log(hit3D.collider.name);
			        SpawnParticle(game, hit3D.point);
			    }
			}
        }
        public void SpawnParticle(GameManager game, Vector3 Point)
        {
            GameObject gunShot = game.Particles.GetGameobject("gunshot");
            gunShot.transform.position = Point;
            gunShot.GetComponent<ParticleSystem>().Play();
        }

        public override void Recycle()
        {
            _pool.Recycle(this);
        }

        public override string Type()
        {
            return "MoveAction";
        }
    }
}