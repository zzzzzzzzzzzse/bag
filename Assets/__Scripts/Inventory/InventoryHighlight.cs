using ChosTIS.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    /// <summary>
    /// 库存高亮显示系统
    /// 负责在拖拽物品时显示网格高亮效果，提供视觉反馈
    /// 使用对象池管理高亮瓦片，支持不同颜色表示不同状态
    /// </summary>
    public class InventoryHighlight : MonoBehaviour
    {
        [SerializeField] RectTransform highlighter;
        [SerializeField] private GameObject highlightTilePrefab;
        private List<GameObject> activeTiles = new();

        public void UpdateShapeHighlight(TetrisItemGhost ghost, Vector2Int originPos, TetrisItemGrid selectedTetrisItemGrid)
        {
            ClearTiles();

            foreach (var point in ghost.TetrisPieceShapePos)
            {
                Vector2Int actualPos = originPos + point + ghost.RotationOffset;
                if (!selectedTetrisItemGrid.BoundryCheck(actualPos.x, actualPos.y, 1, 1))
                    continue;

                Vector2 tilePos = selectedTetrisItemGrid.CalculateTilePosition(ghost, point.x, point.y);
                GameObject tile = PoolManager.Instance.GetObject(highlightTilePrefab);
                SetColor(tile, selectedTetrisItemGrid, actualPos);
                tile.transform.SetParent(highlighter);
                tile.transform.localScale = Vector3.one;
                tile.transform.localPosition = tilePos;
                activeTiles.Add(tile);
            }
        }

        /// <summary>
        /// 清理所有高亮瓦片，回收到对象池
        /// </summary>
        private void ClearTiles()
        {
            foreach (var tile in activeTiles)
            {
                PoolManager.Instance.PushObject(tile);
            }
            activeTiles.Clear();
        }

        /// <summary>
        /// 设置高亮显示器的大小
        /// </summary>
        /// <param name="targetItem">目标物品</param>
        public void SetSize(TetrisItem targetItem)
        {
            Vector2 size = new Vector2();
            size.x = targetItem.WIDTH * TetrisItemGrid.tileSizeWidth;
            size.y = targetItem.HEIGHT * TetrisItemGrid.tileSizeHeight;
            highlighter.sizeDelta = size;
        }

        /// <summary>
        /// 显示或隐藏高亮显示器
        /// </summary>
        /// <param name="b">是否显示</param>
        public void Show(bool b)
        {
            highlighter.gameObject.SetActive(b);
        }

        /// <summary>
        /// 设置高亮显示器的父对象
        /// </summary>
        /// <param name="targetGrid">目标网格</param>
        public void SetParent(TetrisItemGrid targetGrid)
        {
            highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
            highlighter.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// 设置高亮显示器的位置
        /// </summary>
        /// <param name="targetGrid">目标网格</param>
        /// <param name="targetItem">目标物品</param>
        /// <param name="posX">X坐标</param>
        /// <param name="posY">Y坐标</param>
        public void SetPosition(TetrisItemGrid targetGrid, TetrisItem targetItem, int posX, int posY)
        {
            Vector2 pos = targetGrid.CalculateHighlighterPosition(posX, posY);
            highlighter.localPosition = pos;
        }

        /// <summary>
        /// 设置高亮瓦片的颜色，根据位置状态显示不同颜色
        /// </summary>
        /// <param name="tile">高亮瓦片</param>
        /// <param name="targetGrid">目标网格</param>
        /// <param name="tileOnGridPos">网格位置</param>
        private void SetColor(GameObject tile, TetrisItemGrid targetGrid, Vector2Int tileOnGridPos)
        {
            TetrisItem tetrisItem = targetGrid.GetTetrisItem(tileOnGridPos.x, tileOnGridPos.y);
            if (targetGrid.HasItem(tileOnGridPos.x, tileOnGridPos.y))
            {
                if (tetrisItem.ItemDetails.maxStack > 0
                    && TetrisItemMediator.Instance.GetTetrisItemGhost().selectedTetrisItem.ItemDetails.itemID == tetrisItem.ItemDetails.itemID)
                {
                    tile.GetComponent<Image>().color = new Color(1f, 1f, 0f, 100f / 255f);
                }
                else
                {
                    tile.GetComponent<Image>().color = new Color(1f, 0f, 0f, 100f / 255f);
                }
            }
            else
            {
                tile.GetComponent<Image>().color = new Color(0f, 1f, 0f, 100f / 255f);
            }
        }

    }
}