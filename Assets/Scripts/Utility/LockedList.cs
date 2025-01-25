using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class LockedList<T>
    {
        private List<T> values = new List<T>();
        private Queue<T> removals = new Queue<T>();

        public int Count => values.Count;
        public bool Contains(T item) => values.Contains(item);
        public T[] Values => values.ToArray();
        public void Add(T item) => values.Add(item);

        public T Get(int idx) 
        {
            return values[idx];
        }

        public void ProcessRemovals() 
        {
            while (removals.Count > 0)
            {
                values.Remove(removals.Dequeue());
            }
        }
    
        public void QueueRemoval(T item)
        {
            if (values.Contains(item) && !removals.Contains(item)) removals.Enqueue(item);
        }

        public void QueueRemoval(int idx)
        {
            if (!removals.Contains(values[idx])) removals.Enqueue(values[idx]);
        }

        internal void Sort()
        {
            values.Sort();
        }
    }
}
