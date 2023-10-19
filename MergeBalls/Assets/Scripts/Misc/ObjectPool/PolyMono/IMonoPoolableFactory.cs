using UnityEngine;

namespace BiniGames {
    public interface IMonoPoolableFactory<TPoolable> where TPoolable : IPoolable {
        TPoolable CreateInstance(int key, Transform parent);
    }
}
