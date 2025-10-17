using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChosTIS.SaveLoadSystem
{
    /// <summary>
    /// 维护存档中的背包物品列表，提供检索与存在性判断。
    /// </summary>
    [CreateAssetMenu(fileName = "InventoryData_SO", menuName = "SaveLoad/InventoryDataList")]
    public class InventoryData_SO : ScriptableObject
    {
        public List<GridItem> inventoryItemList;

        /// <summary>
        /// 根据运行时索引检索对应的存档物品条目。
        /// </summary>
        /// <param name="index">物品的运行时索引（InstanceID）。</param>
        /// <returns>找到的 GridItem；若不存在则返回 null。</returns>
        public GridItem GetGridItem(int index)
        {
            return inventoryItemList.Find(e => e.itemIndex == index);
        }

        /// <summary>
        /// 判断指定运行时索引的物品是否存在于存档列表。
        /// </summary>
        /// <param name="index">物品的运行时索引（InstanceID）。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool HasGridItem(int index)
        {
            return inventoryItemList.Any(p => p.itemIndex == index);
        }

    }
}