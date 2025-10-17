using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS
{
    /// <summary>
    /// 物品与幽灵体的“状态中介器”：负责在拖拽/旋转过程中缓存并同步方向、旋转标记、旋转偏移与形状点集，支持双向（Item↔Ghost）状态应用。
    /// </summary>
    public class TetrisItemMediator : Singleton<TetrisItemMediator>
    {
        [SerializeField] TetrisItemGhost tetrisItemGhost;

        private Dir _cachedDir;
        private bool _cachedRotated;
        private Vector2Int _cachedRotationOffset;
        private List<Vector2Int> _cachedShapePos;

        private TetrisItem _cachedOrginItem;
        private TetrisItemGrid _cachedOrginGrid;
        private Dir _cachedItemDir;
        private bool _cachedItemRotated;
        private Vector2Int _cachedItemRotationOffset;
        private List<Vector2Int> _cachedItemShapePos;

        // Cache the rotation state of ghost
        /// <summary>
        /// 缓存幽灵体的旋转相关状态（方向、是否旋转、旋转偏移、形状点集）。
        /// </summary>
        /// <param name="ghost">要缓存状态的幽灵体实例。</param>
        public void CacheGhostState(TetrisItemGhost ghost)
        {
            _cachedDir = ghost.Dir;
            _cachedRotated = ghost.Rotated;
            _cachedRotationOffset = ghost.RotationOffset;
            _cachedShapePos = ghost.TetrisPieceShapePos;
        }

        // Cache the rotation state of the item
        /// <summary>
        /// 缓存物品本体的旋转相关状态与所在网格引用（用于拖拽开始或旋转前）。
        /// </summary>
        /// <param name="item">要缓存状态的物品实例。</param>
        public void CacheItemState(TetrisItem item)
        {
            _cachedOrginItem = item;
            _cachedOrginGrid = item.CurrentInventoryContainer as TetrisItemGrid;
            _cachedItemDir = item.Dir;
            _cachedItemRotated = item.Rotated;
            _cachedItemRotationOffset = item.RotationOffset;
            _cachedItemShapePos = item.TetrisPieceShapePos;
        }

        // Synchronize cache status to TetrisItemGhost
        /// <summary>
        /// 将已缓存的物品状态同步到幽灵体（方向、旋转标记、偏移、形状点集与源引用）。
        /// </summary>
        /// <param name="ghost">目标幽灵体实例。</param>
        public void ApplyStateToGhost(TetrisItemGhost ghost)
        {
            ghost.Dir = _cachedItemDir;
            ghost.Rotated = _cachedItemRotated;
            ghost.RotationOffset = _cachedItemRotationOffset;
            ghost.TetrisPieceShapePos = _cachedItemShapePos;
            ghost.selectedTetrisItem = _cachedOrginItem;
            ghost.selectedTetrisItemOrginGrid = _cachedOrginGrid;

        }

        // Synchronize cache status to TetrisItem
        /// <summary>
        /// 将已缓存的幽灵体状态同步回物品本体（方向、旋转标记、旋转偏移、形状点集）。
        /// </summary>
        /// <param name="item">目标物品实例。</param>
        public void ApplyStateToItem(TetrisItem item)
        {
            item.Dir = _cachedDir;
            item.Rotated = _cachedRotated;
            item.RotationOffset = _cachedRotationOffset;
            item.TetrisPieceShapePos = _cachedShapePos;

        }

        /// <summary>
        /// 直接将指定状态应用到物品本体（非缓存回放），用于程序化设置。
        /// </summary>
        /// <param name="item">目标物品实例。</param>
        /// <param name="dir">方向。</param>
        /// <param name="rotated">是否旋转。</param>
        /// <param name="rotationOffset">旋转偏移。</param>
        /// <param name="tetrisPieceShapePos">形状点集。</param>
        public void ApplyStateToItem(TetrisItem item, Dir dir, bool rotated, Vector2Int rotationOffset, List<Vector2Int> tetrisPieceShapePos)
        {
            item.Dir = dir;
            item.Rotated = rotated;
            item.RotationOffset = rotationOffset;
            item.TetrisPieceShapePos = tetrisPieceShapePos;
        }

        /// <summary>
        /// 从物品初始化幽灵体的展示与状态，用于开始拖拽或高亮预览。
        /// </summary>
        /// <param name="item">源物品实例。</param>
        public void SyncGhostFromItem(TetrisItem item)
        {
            tetrisItemGhost.InitializeFromItem(item);
        }

        /// <summary>
        /// 获取内部维护的幽灵体实例，供槽位与管理器使用。
        /// </summary>
        /// <returns>当前的幽灵体实例。</returns>
        public TetrisItemGhost GetTetrisItemGhost()
        {
            return tetrisItemGhost;
        }

    }
}