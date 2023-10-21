using System.Collections.Generic;
using UnityEngine;

namespace BiniGames {
    public class PolyMonoObjectPool<TPoolable> where TPoolable : Component, IPolyPoolable {
        private Dictionary<int, Stack<TPoolable>> pool;
        private IMonoPoolableFactory<TPoolable> factory;
        private Transform poolRoot;
        public PolyMonoObjectPool(IMonoPoolableFactory<TPoolable> factory, Transform poolRoot) {
            pool = new Dictionary<int, Stack<TPoolable>>();
            this.factory = factory;
            this.poolRoot = poolRoot;
        }

        public TPoolable Pop(int key, Transform parent) {
            Stack<TPoolable> stack;
            var isFound = pool.TryGetValue(key, out stack);
            if (!isFound) {
                stack = new Stack<TPoolable>();
                pool.Add(key, stack);
            }

            return Pop(stack, key, parent);
        }

        private TPoolable Pop(Stack<TPoolable> stack, int key, Transform parent) {
            var result = stack.Count > 0 ? stack.Pop() : CreateNewInstance(key, poolRoot);
            result.OnPop();
            result.transform.SetParent(parent);
            return result;
        }

        public void Pool(TPoolable poolable, Transform parent) {
            Stack<TPoolable> stack;
            var isFound = pool.TryGetValue(poolable.Key, out stack);
            if (!isFound) {
                stack = new Stack<TPoolable>();
                pool.Add(poolable.Key, stack);
            }

            Pool(stack, poolable, parent);
        }

        private void Pool(Stack<TPoolable> stack, TPoolable poolable, Transform parent) {
            poolable.transform.SetParent(parent);
            poolable.OnPool();
            stack.Push(poolable);
        }

        private TPoolable CreateNewInstance(int key, Transform parent) {
            return factory.CreateInstance(key, parent);
        }
    }
}
