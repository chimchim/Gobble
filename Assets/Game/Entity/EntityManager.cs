using System.Collections;
using System.Collections.Generic;
using Game.Misc;
using Game.Component;
using UnityEngine;
using Game;

namespace Game.GEntity
{
	public class EntityManager 
	{
		private Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();

		public void addEntity(Entity entity)
		{
			if(!_entities.ContainsKey(entity.ID))
			{
				_entities.Add(entity.ID, entity);
			}
			else
			{
				Debug.Log("Could not add entity, ID Taken!");
			}
		}

        public T GetComponentOf<T>(int entity) where T : GComponent
        {

            return _entities[entity].GetComponent<T>();
        }
        public void DisableComponent<T>(int entity) where T : GComponent
        {

            _entities[entity].DisableComponent<T>();
        }
        public Entity GetEntity(int entity)
        {
            return _entities[entity];
        }

        public bool HasComponent(int entityid, int familiyid)
        { 
            return _entities[entityid].HasComponent(familiyid);
        }
        public IEnumerable<int> GetEntites()
        {
            return _entities.Keys;
        }

        public IEnumerable<int> GetEntitiesWithComponents(Bitmask key)
        {
            foreach (var val in _entities)
            {
                if (key.Fits(val.Value.Lock))
                {
                    yield return val.Key;
                }
            }
            //return new EntityIterator.Proxy(_entities, key);
        }

        public void AddMessage(int id, Message mess)
        {
            _entities[id].NewMessages.Add(mess);
        }
	}
}
