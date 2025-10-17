using UnityEngine;

namespace ChosTIS
{
    /// <summary>
    /// 持久化网格标识组件
    /// 为网格提供持久化标识，用于存档系统识别网格类型
    /// </summary>
    [RequireComponent(typeof(TetrisItemGrid))]
    public class PersistentGridIdentification : MonoBehaviour
    {
        [Header("Manual Configuration")]
        public TetrisItemGrid persistentGridObject;
        public PersistentGridType persistentGridType;

        /// <summary>
        /// 获取持久化网格类型索引
        /// </summary>
        /// <returns>网格类型索引</returns>
        public int GetPersistentGridTypeIndex()
        {
            return (int)persistentGridType;
        }
    }
}