using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS
{
    /// <summary>
    /// 背包容器接口：约定物品与网格/槽位的放置、移除与关联关系。
    /// </summary>
    public interface IInventoryContainer
    {
        /// <summary>
        /// 与当前容器关联的物品引用（如槽位内的物品或网格所拥有的父物品）。
        /// </summary>
        public TetrisItem RelatedTetrisItem { get; set; }

        /// <summary>
        /// 尝试将物品放置到容器的指定网格坐标位置。
        /// </summary>
        /// <param name="tetrisItem">要放置的物品。</param>
        /// <param name="posX">目标网格的 X 坐标。</param>
        /// <param name="posY">目标网格的 Y 坐标。</param>
        /// <returns>放置成功返回 true，否则返回 false。</returns>
        public bool TryPlaceTetrisItem(TetrisItem tetrisItem, int posX, int posY)
        {
            return false;
        }

        /// <summary>
        /// 将物品强制放置到容器的指定网格坐标位置（无需可行性检查）。
        /// </summary>
        /// <param name="tetrisItem">要放置的物品。</param>
        /// <param name="posX">目标网格的 X 坐标。</param>
        /// <param name="posY">目标网格的 Y 坐标。</param>
        public void PlaceTetrisItem(TetrisItem tetrisItem, int posX, int posY)
        {

        }

        /// <summary>
        /// 从容器中移除物品，并可基于传入的旧状态（坐标、旋转偏移、形状点集）处理回滚或清理。
        /// </summary>
        /// <param name="toReturn">要移除的物品。</param>
        /// <param name="x">物品在网格中的 X 坐标。</param>
        /// <param name="y">物品在网格中的 Y 坐标。</param>
        /// <param name="oldRotationOffset">移除前的旋转偏移。</param>
        /// <param name="tetrisPieceShapePositions">移除前的形状点集。</param>
        public void RemoveTetrisItem(TetrisItem toReturn, int x, int y, Vector2Int oldRotationOffset, List<Vector2Int> tetrisPieceShapePositions)
        {

        }

    }
}

