using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保存所有物品静态数据（尺寸、形状、堆叠、图像等），提供按 ID 与索引检索接口。
/// </summary>
[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Inventory/ItemDataList")]
public class ItemDataList_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;

    /// <summary>
    /// 按物品 ID 检索静态数据。
    /// </summary>
    /// <param name="ID">物品 ID。</param>
    /// <returns>对应的 ItemDetails；找不到返回 null。</returns>
    public ItemDetails GetItemDetailsByID(int ID)
    {
        return itemDetailsList.Find(i => i.itemID == ID);
    }

    /// <summary>
    /// 按列表索引返回静态数据（用于快速访问或调试）。
    /// </summary>
    /// <param name="index">列表索引。</param>
    /// <returns>对应的 ItemDetails。</returns>
    public ItemDetails GetItemDetailsByIndex(int index)
    {
        return itemDetailsList[index];
    }

}
