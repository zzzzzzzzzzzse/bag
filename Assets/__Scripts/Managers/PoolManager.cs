using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS.Utility
{
    /// <summary>
    /// 对象池管理器
    /// 管理游戏对象的对象池，优化性能，减少频繁创建和销毁
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, Queue<GameObject>> objectPool = new();
        private GameObject pool;

        /// <summary>
        /// 从对象池获取对象
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>激活的对象</returns>
        public GameObject GetObject(GameObject prefab)
        {
            //Declare the object to be retrieved
            GameObject _object;
            if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
            {
                _object = GameObject.Instantiate(prefab);
                PushObject(_object);
                if (pool == null)
                {
                    pool = new GameObject("ObjectPool");
                    pool.transform.SetParent(this.transform);
                }
                GameObject childPool = GameObject.Find(prefab.name + "Pool");
                if (!childPool)
                {
                    childPool = new GameObject(prefab.name + "Pool");
                    childPool.transform.SetParent(pool.transform);
                }
                _object.transform.SetParent(childPool.transform);
            }

            _object = objectPool[prefab.name].Dequeue();
            _object.SetActive(true);
            return _object;
        }

        /// <summary>
        /// 将对象回收到对象池
        /// </summary>
        /// <param name="prefab">要回收的对象</param>
        public void PushObject(GameObject prefab)
        {
            string _name = prefab.name.Replace("(Clone)", string.Empty);
            if (!objectPool.ContainsKey(_name))
            {
                objectPool.Add(_name, new Queue<GameObject>());
            }
            objectPool[_name].Enqueue(prefab);
            prefab.SetActive(false);
        }
    }
}