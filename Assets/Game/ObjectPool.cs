using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public interface IObjectPool<T>
    {
        int Count { get; }
        void Resize(int size);
        T GetNext();
        bool Recycle(T obj);
        void Release(T obj);
    }

    public class FakeObjectPool<T> : IObjectPool<T> where T : class, new()
    {
        public int Count { get { return 0; } }
        public void Resize(int size)
        {

        }

        public T GetNext()
        {
            return new T();
        }

        public bool Recycle(T obj)
        {
            return true;
        }

        public void Release(T obj)
        {

        }
    }

    public class ObjectPool<T> : IObjectPool<T> where T : class, new()
    {
        public delegate T Allocator();

        private T[] _pool;
        private bool[] _live;

        private int _size;
        private object _lock = new object();

        private Allocator _alloc;
        public ObjectPool(int size, Allocator alloc = null)
        {
            _alloc = alloc;
            _size = size;
            _pool = new T[_size];
            _live = new bool[_size];
            for (int i = 0; i < _size; ++i)
            {
                if (_alloc == null)
                {
                    _pool[i] = new T();
                }
                else
                {
                    _pool[i] = _alloc();
                }
                _live[i] = false;
            }
        }

        public int Count { get { lock (_lock) { return _size; } } }

        public void Resize(int size)
        {
            try
            {
                lock (_lock)
                {
                    int oldSize = _size;
                    _size = size;

                    T[] newPool = new T[size];
                    bool[] newLive = new bool[size];

                    CopyArray(_pool, newPool);
                    CopyArray(_live, newLive);

                    _pool = newPool;
                    _live = newLive;

                    for (int i = oldSize; i < size; ++i)
                    {
                        if (_alloc == null)
                        {
                            _pool[i] = new T();
                        }
                        else
                        {
                            _pool[i] = _alloc();
                        }
                        _live[i] = false;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public T GetNext()
        {
            try
            {
                lock (_lock)
                {
                    for (int i = 0; i < _size; ++i)
                    {
                        if (!_live[i])
                        {
                            _live[i] = true;
                            return _pool[i];
                        }
                    }

                    int oldSize = _size;

                    Resize(_size < 10 ? 20 : _size * 2);

                    _live[oldSize] = true;
                    return _pool[oldSize];
                }
            }
            catch (Exception e)
            {
                if (_alloc == null)
                {
                    return new T();
                }
                else
                {
                    return _alloc();
                }
            }
        }

        public void Release(T obj)
        {
            try
            {
                lock (_lock)
                {
                    for (int i = 0; i < _size; ++i)
                    {
                        if (object.ReferenceEquals(_pool[i], obj))
                        {
                            if (_alloc == null)
                            {
                                _pool[i] = new T();
                            }
                            else
                            {
                                _pool[i] = _alloc();
                            }
                            _live[i] = false;
                            return;
                        }
                    }
                    throw new InvalidOperationException("No such reference");
                }
            }
            catch (Exception e)
            {
            }
        }

        public bool Recycle(T obj)
        {
            try
            {
                lock (_lock)
                {
                    for (int i = 0; i < _size; ++i)
                    {
                        if (object.ReferenceEquals(_pool[i], obj))
                        {
                            _live[i] = false;
                            return true;
                        }
                    }

                    throw new InvalidOperationException("No such reference");
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void CopyArray<U>(U[] from, U[] to)
        {
            for (int i = 0; i < from.Length && i < to.Length; ++i)
            {
                to[i] = from[i];
            }
        }
    }

