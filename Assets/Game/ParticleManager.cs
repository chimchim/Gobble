using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//blog.ashwanik.in/2013/11/objects-pool-manager-in-unity3d.html
namespace Game
{
    public class ParticleManager
    {
        public Dictionary<string, GameObjectPool> ParticlePools = new Dictionary<string, GameObjectPool>();

        private GameObjectPool GunShotPool;
		private GameObjectPool MuzzlePool;
        public void Initiate()
        {
            //GameObject instance = GameObject.Instantiate(Resources.Load("gunshot", typeof(GameObject))) as GameObject;
            //GunShotPool = new GameObjectPool(20, instance, "Gunshots");
            //ParticlePools.Add("gunshot", GunShotPool);
        }

        public GameObject GetGameobject(string name)
        {
            return ParticlePools[name].GetObject();
        }
        public void Update()
        {
            GunShotPool.CheckIfParticlesDone();
        }

    }

    public class GameObjectPool
    {
        Stack<GameObject> inactive;
        List<GameObject> active;

        private GameObject _prefab;
		private GameObject _PoolParent;
        public GameObjectPool(int size, GameObject prefab, string poolname)
        {
            _prefab = prefab;
            inactive = new Stack<GameObject>();
            active = new List<GameObject>();
			_PoolParent = new GameObject();
			_PoolParent.name = poolname;
            for(int i = 0 ; i < size; i++)
            {
                GameObject obj = GameObject.Instantiate(prefab);
				obj.transform.parent = _PoolParent.transform;
                obj.SetActive(false);
                inactive.Push(obj);
            }
            //GameObject.Destroy(prefab);
        }

        public GameObject GetObject()
        {
            if (inactive.Count > 0)
            {
                GameObject obj = inactive.Pop();
                active.Add(obj);
				obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = GameObject.Instantiate(_prefab);
				obj.transform.parent = _PoolParent.transform;
                active.Add(obj);
				obj.SetActive(true);
                return obj;
            }
        }

        public void CheckIfParticlesDone()
        {
            for (int i = 0; i < active.Count; i++)
            {
                if (!active[i].gameObject.GetComponent<ParticleSystem>().IsAlive(true))
                {
                    inactive.Push(active[i]);
                    active[i].SetActive(false);
                }
            }
        }
		public void CheckIfTimerDone()
		{
			for (int i = 0; i < active.Count; i++)
			{
				if (active[i].gameObject.GetComponent<Timer>().calcTimer < 0)
				{
					active[i].gameObject.GetComponent<Timer>().Reset();
					inactive.Push(active[i]);
					active[i].SetActive(false);
				}
			}
		}
    }
    
}
