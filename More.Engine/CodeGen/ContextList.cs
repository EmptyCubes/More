using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace More.Engine.CodeGen
{
    /// <summary>
    /// This is a special list that can keep up with variable scope.
    /// Use this when you need to print some code and need to keep
    /// up with variables that can be accessed.
    /// </summary>
    public class ContextList<T> : IList<T>
    {
        private Stack<List<T>> _contextVariables;

        public IEnumerable<T> All
        {
            get { return ContextVariables.SelectMany(x => x.ToArray()).ToArray(); }
        }

        public Stack<List<T>> ContextVariables
        {
            get { return _contextVariables ?? (_contextVariables = new Stack<List<T>>()); }
            set { _contextVariables = value; }
        }

        public int Count
        {
            get { return All.Count(); }
        }

        public List<T> CurrentList
        {
            get
            {
                return ContextVariables.Peek();
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return CurrentList[index]; }
            set { CurrentList[index] = value; }
        }

        public void Add(T item)
        {
            CurrentList.Add(item);
        }

        public void Clear()
        {
            ContextVariables.Clear();
        }

        public bool Contains(T item)
        {
            return All.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            return;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return All.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return 0;
        }

        public void Insert(int index, T item)
        {
            CurrentList.Insert(index, item);
        }

        public void PopContext()
        {
            ContextVariables.Pop();
        }

        public void PushContext()
        {
            ContextVariables.Push(new List<T>());
        }

        public bool Remove(T item)
        {
            return CurrentList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            CurrentList.RemoveAt(index);
        }
    }
}