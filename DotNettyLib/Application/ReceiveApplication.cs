using System.Collections.Generic;

namespace DotNettyLib.Application
{
    public class ReceiveApplication<T> : IConsumerProduct<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();

        public bool IsEmpty() => _queue.Count <= 0;
        public int Size() => _queue.Count;

        public bool TryTake(out T msg)
        {
            msg = default(T);

            if (IsEmpty())
                return false;

            msg = _queue.Dequeue();
            return true;
        }

        public bool TryAdd(T msg)
        {
            _queue.Enqueue(msg);

            return true;
        }
    }
}