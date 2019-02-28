namespace BesmashContent.Collections {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class PriorityQueue<K, V> where K : IComparable {
        public int Count {get {return count;}}

        public List<V> Values {get {
            List<V> values = new List<V>();
            map.Values.ToList().ForEach(q =>
                values.AddRange(q));
            return values;
        }}

        private int count;
        private SortedDictionary<K, Queue<V>> map;

        public PriorityQueue() {
            map = new SortedDictionary<K, Queue<V>>();
        }

        public void enqueue(K key, V value) {
            Queue<V> queue = null;
            map.TryGetValue(key, out queue);
            if(queue == null) {
                queue = new Queue<V>();
                map[key] = queue;
            }

            queue.Enqueue(value);
            ++count;
        }

        public V dequeue() {
            if(map.Values.Count == 0)
                return default(V);

            --count;
            KeyValuePair<K, Queue<V>> first = map.First();
            Queue<V> queue = first.Value;

            if(queue.Count == 1)
                map.Remove(first.Key);
                
            return queue.Dequeue();
        }

        public void remove(K key, V value) {
            Queue<V> queue = null;
            map.TryGetValue(key, out queue);

            if(queue != null) {
                Queue<V> newQueue = new Queue<V>(queue.Where(v => !v.Equals(value)));
                int dif = queue.Count - newQueue.Count;
                if(dif > 0) {
                    map[key] = newQueue;
                    count -= dif;
                }
            }
        }
    }
}