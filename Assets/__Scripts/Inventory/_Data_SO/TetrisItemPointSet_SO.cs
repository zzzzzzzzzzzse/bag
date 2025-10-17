using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储各个物品的俄罗斯方块形状点集，供旋转与网格放置计算使用。
/// </summary>
[CreateAssetMenu(fileName = "TetrisItemPointSet_SO", menuName = "Inventory/ItemPointSet")]
public class TetrisItemPointSet_SO : ScriptableObject
{
    public List<PointSet> TetrisPieceShapeList;

    //public List<Vector2Int> GetTetrisPointSetByID(int index)
    //{
    //    return TetrisPieceShapeList[index].points;
    //}

}
