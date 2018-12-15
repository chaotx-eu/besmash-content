namespace BesmashContent.Collections {
    using System.Collections.Generic;

    public class FixedQueue<T> : Queue<T> {
        public int Limit {get;}

        public FixedQueue(int limit) : base(limit) {
            Limit = limit;
        }

        public new void Enqueue(T obj) {
            if(Count == Limit) Dequeue();
            base.Enqueue(obj);
        }
    }
}