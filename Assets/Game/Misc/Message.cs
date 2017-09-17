using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
namespace Game
{
    public abstract class Message
    {
        private static Dictionary<string, Type> _derived;
        private static readonly object _lock = new object();


        public abstract void Recycle();
        public abstract void Release();

        public static List<Type> GetTypes()
        {
            List<Type> ret = new List<Type>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(typeof(Message)))
                {
                    ret.Add(type);
                }
            }
            return ret;
        }

        public static Type GetType(string name)
        {
            lock (_lock)
            {
                if (_derived == null)
                {
                    _derived = new Dictionary<string, Type>();

                    Type[] types = Assembly.GetExecutingAssembly().GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsSubclassOf(typeof(Message)))
                        {
                            _derived.Add(type.Name, type);
                        }
                    }
                }

                Type ret;
                if (_derived.TryGetValue(name, out ret))
                {
                    return ret;
                }
                return null;
            }
        }
    }

    public abstract class BaseMessage<T> : Message where T : Message, new()
    {

        protected static readonly ObjectPool<T> _pool = new ObjectPool<T>(20);

        private bool _released = false;
        private bool _recycleAfterSend = true;

        protected BaseMessage()
        {

        }

        /// <summary>
        /// Get next message from the pool
        /// </summary>
        /// <param name="sender">The entityId of the entity that sent the message, set to -1 if no sender.</param>
        /// <param name="reciever">The entityId of the reciever, set to -1 if there should be no specefic reciever.</param>
        /// <returns></returns>

        protected virtual void Reset()
        {

        }
        public sealed override void Recycle()
        {
            if (!_released)
            {
                _pool.Recycle(this as T);
            }
        }
        public sealed override void Release()
        {
            _pool.Release(this as T);
            _released = true;
        }
    }

    public class FoundPlayer : BaseMessage<FoundPlayer>
    {
        public Vector3 HitPoint;
        public int ID;
        public static FoundPlayer Make(Vector3 point, int id)
        {
            var ret = _pool.GetNext();
            ret.HitPoint = point;
            ret.ID = id;
            return ret;
        }
    }
    public class ShootPoint : BaseMessage<ShootPoint>
    {
        public Vector3 HitPoint;
        public static ShootPoint Make(Vector3 point)
        {
            var ret = _pool.GetNext();
            ret.HitPoint = point;
            return ret;
        }
    }
    public class MissPlayer : BaseMessage<MissPlayer>
    {
        public Vector3 LastPosition;
        public static MissPlayer Make(Vector3 last)
        {
            var ret = _pool.GetNext();
            ret.LastPosition = last;
            return ret;
        }
    }
}