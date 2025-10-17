using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 俄罗斯方块可旋转接口
/// 定义可旋转对象的基本属性和方法
/// </summary>
public interface ITetrisRotatable
{
    /// <summary>
    /// 是否已旋转
    /// </summary>
    bool Rotated { get; set; }
    
    /// <summary>
    /// 当前方向
    /// </summary>
    Dir Dir { get; set; }
    
    /// <summary>
    /// 旋转偏移量
    /// </summary>
    Vector2Int RotationOffset { get; set; }
    
    /// <summary>
    /// 物品详情
    /// </summary>
    ItemDetails ItemDetails { get; set; }
    
    /// <summary>
    /// 俄罗斯方块形状点位列表
    /// </summary>
    List<Vector2Int> TetrisPieceShapePos { get; set; }

    /// <summary>
    /// 当前宽度
    /// </summary>
    int WIDTH { get; }
    
    /// <summary>
    /// 当前高度
    /// </summary>
    int HEIGHT { get; }
}
