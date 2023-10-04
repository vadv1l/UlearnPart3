using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T>
    where T : IComparable
    {
        public T Value { get; private set; }
        public BinaryTree<T> Left { get; private set; }
        public BinaryTree<T> Right { get; private set; }

        private bool _isEmpty;

        public BinaryTree()
        {
            Value = default;
            _isEmpty = true;
            Left = null;
            Right = null;
        }

        public BinaryTree(IEnumerable<T> items) : this()
        {
            foreach (var item in items)
                Add(item);
        }

        public void Add(T item)
        {
            if (_isEmpty)
            {
                Value = item;
                _isEmpty = false;
                return;
            }
            if (item.CompareTo(Value) <= 0)
            {
                if (Left is null)
                    Left = new BinaryTree<T>();
                Left.Add(item);
                return;
            }
            if (Right is null)
                Right = new BinaryTree<T>(); 
            Right.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if(_isEmpty) yield break;
            if(!(Left is null))
                foreach(var item in Left)
                    yield return item;
            yield return Value;
            if(!(Right is null))
                foreach(var item in Right)
                    yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] items) where T : IComparable => new BinaryTree<T>(items);
    }
}