using System;
using System.Collections.Concurrent;

namespace FuelRushMaui.Services
{
    /// <summary>
    /// Thread-safe generic Object Pool for high-frequency gameplay entities (Particles, Obstacles, Collectibles)
    /// to eliminate Garbage Collection allocations and memory leaks.
    /// </summary>
    public class ObjectPool<T> where T : class, new()
    {
        private readonly ConcurrentBag<T> _pool = new();
        private readonly Func<T> _generator;
        private readonly Action<T>? _resetAction;

        public ObjectPool(Func<T>? generator = null, Action<T>? resetAction = null)
        {
            _generator = generator ?? (() => new T());
            _resetAction = resetAction;
        }

        public T Get()
        {
            if (_pool.TryTake(out var item))
            {
                return item;
            }
            return _generator();
        }

        public void Return(T item)
        {
            if (item == null) return;
            _resetAction?.Invoke(item);
            _pool.Add(item);
        }

        public int Count => _pool.Count;
    }
}
