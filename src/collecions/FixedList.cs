namespace BesmashContent.Collections {
    using System.Collections.Generic;

    public class FixedList<T> : List<T> {
        public int Limit {get;}

        public FixedList(int limit) : base(limit) {
            Limit = limit;
        }

        public new void Add(T obj) {
            if(Count == Limit) RemoveAt(0);
            base.Add(obj);
        }

        public new void Insert(int i, T obj) {
            if(Count == Limit) RemoveAt(Count-1);
            base.Insert(i, obj);
        }
    }
}