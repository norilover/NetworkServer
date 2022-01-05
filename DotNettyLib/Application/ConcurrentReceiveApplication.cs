using System.Collections.Concurrent;

namespace DotNettyLib.Application
{
    public class ConcurrentReceiveApplication<T> : IConsumerProduct<T>
    {
        private readonly IProducerConsumerCollection<T> _queue = new ConcurrentQueue<T>();

        public bool IsEmpty() => _queue.Count <= 0;
        public int Size() => _queue.Count;

        public bool TryTake(out T msg)
        {
            msg = default(T);

            if (IsEmpty())
                return false;

            return _queue.TryTake(out msg);
        }

        public bool TryAdd(T msg)
        {
            return _queue.TryAdd(msg);
        }
    }
}