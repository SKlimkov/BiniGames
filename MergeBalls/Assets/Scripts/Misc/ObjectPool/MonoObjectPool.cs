using System.Collections.Generic;
using UnityEngine;

namespace BiniGames {
    public class MonoObjectPool<TPoolable> where TPoolable : Component, IPoolable {
        private Stack<TPoolable> poolables = new Stack<TPoolable>(32);
        private Transform poolRoot;

        public MonoObjectPool(Transform poolRoot) {
            this.poolRoot = poolRoot;
        }

        public int Count { get { return poolables.Count; } }

        public TPoolable Pop(TPoolable prefab, Vector3 position, Transform parent) {
            var isSuccess = poolables.TryPop(out var result);
            if (!isSuccess) result = Create(prefab, position, parent);
            else {
                var gameObject = result.gameObject;
                gameObject.transform.SetParent(parent);
                gameObject.transform.position = position;
                gameObject.SetActive(true);
            }
            result.OnPop();

            return result;
        }

        public void Pool(TPoolable poolable) {
            poolables.Push(poolable);
            var gameObject = poolable.gameObject;
            gameObject.transform.SetParent(poolRoot);
            poolable.OnPool();
            gameObject.SetActive(false);
        }

        private TPoolable Create(TPoolable prefab, Vector3 position, Transform parent) {
            return GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
        }
    }
}
