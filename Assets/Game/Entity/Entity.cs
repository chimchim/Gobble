using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Misc;

namespace Game.GEntity
{
	public class Entity
	{
        private readonly Bitmask _lock;
		private int _id;
		private Dictionary<int,GComponent> _components = new Dictionary<int,GComponent>();

        public Bitmask Lock { get { return _lock; } }
        public int ID { get { return _id; } }
        public GameObject gameObject;
		public Animator Animator;
		public Vector3 LastPosition;
		public Vector2 PlayerSpeed;
		public void RecycleEntity()
		{
			foreach (GComponent comp in _components.Values)
			{
				comp.Recycle();
			}
			Animator = null;
			gameObject = null;
			_components = null;
		}
		public Entity(int replaceID = -1)
		{
            _lock = Bitmask.Zero;
			if (replaceID == -1)
			{
				_id = IDGiver.GetNewID();
			}
			else
			{
				_id = replaceID;
			}
			if (IDGiver.NextID < replaceID)
			{
				IDGiver.NextID = replaceID;
			}
            
		}
        public void DisableComponent<T>() where T : GComponent
        {
            int id = GComponent.GetID<T>();
            _lock.Unset(id);
            //if (!_components.ContainsKey(id))
            //{
            //    _components.Add(id, component);
            //}
        }
        public void AddComponent<T>(T component) where T:GComponent
		{
            int id = GComponent.GetID<T>();
            _lock.Set(id);
            if (!_components.ContainsKey(id))
			{
				_components.Add(id, component);
			}
		}

        public T GetComponent<T>() where T : GComponent
        {

            foreach (GComponent component in _components.Values)
            {
                if (component is T)
                {

                    return (T)component;
                }
            }
            return null;
        }

        public bool HasComponent(int familiyid)
        {
            return _components.ContainsKey(familiyid);
        }
	}
}
