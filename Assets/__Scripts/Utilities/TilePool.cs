using UnityEngine;
using System.Collections.Generic;

namespace ChosTIS
{
    /// <summary>
    /// 瓦片对象池
    /// 专门用于管理高亮瓦片的对象池，优化性能
    /// </summary>
    public class TilePool : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private int initialPoolSize = 10;

        private Queue<GameObject> pool = new Queue<GameObject>();

        /// <summary>
        /// 初始化对象池
        /// </summary>
        private void Awake()
        {
            InitializePool();
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewTile();
            }
        }

        /// <summary>
        /// 创建新的瓦片并添加到池中
        /// </summary>
        /// <returns>新创建的瓦片</returns>
        private GameObject CreateNewTile()
        {
            GameObject tile = Instantiate(tilePrefab, transform);
            tile.SetActive(false);
            pool.Enqueue(tile);
            return tile;
        }

        /// <summary>
        /// 从池中获取可用的瓦片
        /// </summary>
        /// <returns>激活的瓦片对象</returns>
        public GameObject GetTile()
        {
            if (pool.Count == 0)
            {
                // 池为空时动态扩展
                CreateNewTile();
            }

            GameObject tile = pool.Dequeue();
            tile.SetActive(true);
            return tile;
        }

        /// <summary>
        /// 将瓦片返回到池中
        /// </summary>
        /// <param name="tile">要返回的瓦片</param>
        public void ReturnTile(GameObject tile)
        {
            tile.SetActive(false);
            pool.Enqueue(tile);
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void ClearPool()
        {
            foreach (var tile in pool)
            {
                Destroy(tile);
            }
            pool.Clear();
        }
    }
}