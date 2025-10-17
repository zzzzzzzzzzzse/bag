using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品详细信息数据结构
/// 包含物品的所有静态属性，如ID、名称、描述、形状、图标等
/// </summary>
[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public string itemDescription;

    public GameObject itemEntity;

    public TetrisPieceShape tetrisPieceShape;
    public Sprite itemIcon;
    public Sprite itemUI;

    public Transform uiPrefab;
    public Transform gridUIPrefab;
    public InventorySlotType inventorySlotType;
    public int itemDamage;
    public int maxStack;
    public float reloadTime;

    public int yHeight;
    public int xWidth;
    public float weight;
    public Dir dir;

    public int itemPrice;
    [Range(0f, 1f)]
    public float sellPercentage;
    
}

/// <summary>
/// 点位集合数据结构
/// 定义俄罗斯方块形状的点位坐标
/// </summary>
[System.Serializable]
public class PointSet
{
    public int itemShapeID;
    public TetrisPieceShape tetrisPieceShape;
    public List<Vector2Int> points;
}

/// <summary>
/// 游戏存档数据结构
/// 包含库存数据和存档时间信息
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public Dictionary<string, List<GridItem>> inventoryDict;
    public Dictionary<string, string> gameSaveTimeDict;
}

/// <summary>
/// 网格物品数据结构
/// 用于存档系统中存储物品在网格中的位置和状态信息
/// </summary>
[System.Serializable]
public class GridItem
{
    public int itemID;
    public int itemIndex;
    public int parentItemIndex;
    public int gridPIndex;
    public int persistentGridTypeIndex;
    public bool isParent;
    public bool isOnSlot;
    public Vector2Int orginPosition;
    public Dir direction;
    public int stack;
}