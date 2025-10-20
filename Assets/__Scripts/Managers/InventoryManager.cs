using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChosTIS
{
    /// <summary>
    /// 背包系统的核心管理器：维护当前选中网格与物品，处理高亮、预览与程序化放置。
    /// </summary>
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("Current Tetris Item Grid")]
        public TetrisItemGrid selectedTetrisItemGrid;
        [Header("Tetris Item Details Data")]
        public ItemDataList_SO itemDataList_SO;
        [Header("Tetris Item Points Set Data")]
        public TetrisItemPointSet_SO tetrisItemPointSet_SO;
        [Header("Depository")]
        public TetrisItemGrid depositoryGrid;
        [Header("Components")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private InventoryHighlight inventoryHighlight;
        [SerializeField] private TetrisItemGhost tetrisItemGhost;
        [SerializeField] private RightClickMenuPanel rightClickMenuPanel;
        public TetrisItem selectedTetrisItem;
        public Vector2Int tileGridOriginPosition;

        private TetrisItem overlapItem;
        private Vector2Int oldPosition;
        private int selectedItemIndex;

        /// <summary>
        /// 每帧更新，处理输入事件和物品操作
        /// </summary>
        private void Update()
        {
            // 动态添加物品（调试用）
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (selectedItemIndex >= itemDataList_SO.itemDetailsList.Count)
                {
                    selectedItemIndex = 0;
                }
                if (selectedTetrisItem != null)
                {
                    Destroy(selectedTetrisItem.gameObject);
                    selectedTetrisItem = null;
                }
                CreateItemInOrder(selectedItemIndex);
                ++selectedItemIndex;
            }

            // 旋转物品
            if (Input.GetKeyDown(KeyCode.R))
            {
                oldPosition = new();
                RotateItemGhost();
            }

            // 更新选中物品的UI位置
            if (selectedTetrisItem) selectedTetrisItem.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                TetrisItemGrid tetrisItemGrid = GetGridUnderMouse();
                if (tetrisItemGrid != null)
                {
                    // 获取当前鼠标位置在网格中的坐标
                    Vector2Int tileGridOriginPosition = GetTileGridOriginPosition(tetrisItemGrid);
                    if (selectedTetrisItem == null)
                    {
                        return;
                    }
                    else
                    {
                        PlaceItem(tileGridOriginPosition, tetrisItemGrid);
                        selectedItemIndex = 0;
                    }

                }

            }

            HandleHighlight(selectedTetrisItemGrid != null);
        }

        /// <summary>
        /// 获取指定形状的俄罗斯方块点集坐标，用于网格放置与旋转计算。
        /// </summary>
        /// <param name="shape">物品的俄罗斯方块形状枚举。</param>
        /// <returns>该形状对应的网格坐标列表。</returns>
        public List<Vector2Int> GetTetrisPieceShapePos(TetrisPieceShape shape)
        {
            return tetrisItemPointSet_SO.TetrisPieceShapeList[(int)shape].points;
        }

        /// <summary>
        /// 旋转物品幽灵
        /// </summary>
        private void RotateItemGhost()
        {
            if (tetrisItemGhost.ItemDetails == null) return;
            tetrisItemGhost.Rotate();
        }

        /// <summary>
        /// 获取物品幽灵的网格原点坐标，如果物品未被拾取则返回鼠标位置网格坐标
        /// </summary>
        /// <returns>网格坐标</returns>
        public Vector2Int GetGhostTileGridOriginPosition()
        {
            if (selectedTetrisItemGrid == null) return new Vector2Int();
            Vector2 origin = Input.mousePosition;
            Vector2Int tileGridPosition = selectedTetrisItemGrid.GetTileGridPosition(origin);
            if (tetrisItemGhost.ItemDetails != null)
            {
                int offsetX = Mathf.FloorToInt((tetrisItemGhost.WIDTH - 1) / 2);
                int offsetY = Mathf.FloorToInt((tetrisItemGhost.HEIGHT - 1) / 2);
                tileGridPosition.x -= offsetX;
                tileGridPosition.y -= offsetY;
            }
            return tileGridPosition;
        }

        /// <summary>
        /// 获取俄罗斯方块物品的网格原点坐标，如果物品未被拾取则返回鼠标位置网格坐标
        /// </summary>
        /// <param name="tetrisItemGrid">目标网格</param>
        /// <returns>网格坐标</returns>
        private Vector2Int GetTileGridOriginPosition(TetrisItemGrid tetrisItemGrid)
        {
            Vector2 origin = Input.mousePosition;
            if (selectedTetrisItem != null)
            {
                origin.x -= (selectedTetrisItem.WIDTH - 1) * TetrisItemGrid.tileSizeWidth / 2;
                origin.y += (selectedTetrisItem.HEIGHT - 1) * TetrisItemGrid.tileSizeHeight / 2;
            }
            Vector2Int tileGridPosition = tetrisItemGrid.GetTileGridPosition(origin);
            return tileGridPosition;
        }

        /// <summary>
        /// 处理高亮显示
        /// </summary>
        /// <param name="isShow">是否显示高亮</param>
        private void HandleHighlight(bool isShow)
        {
            Vector2Int positionOnGrid = GetGhostTileGridOriginPosition();
            if (oldPosition == positionOnGrid) return;
            oldPosition = positionOnGrid;
            if (tetrisItemGhost.OnDragging && isShow)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.UpdateShapeHighlight(tetrisItemGhost, positionOnGrid, selectedTetrisItemGrid);
                inventoryHighlight.SetParent(selectedTetrisItemGrid);
                inventoryHighlight.SetPosition(selectedTetrisItemGrid,
                tetrisItemGhost.selectedTetrisItem,
                positionOnGrid.x, positionOnGrid.y);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }

        /// <summary>
        /// 拖拽放置俄罗斯方块物品
        /// </summary>
        /// <param name="tileGridOriginPosition">网格原点位置</param>
        /// <param name="tetrisItem">要放置的物品</param>
        /// <param name="targetGrid">目标网格</param>
        /// <param name="fromSlot">来源槽位</param>
        public void PlaceGhostItem(Vector2Int tileGridOriginPosition, TetrisItem tetrisItem, TetrisItemGrid targetGrid, InventorySlot fromSlot)
        {
            if (targetGrid == null) return;
            bool isDone = targetGrid.TryPlaceTetrisItem(tetrisItem, tileGridOriginPosition.x, tileGridOriginPosition.y);
            StartCoroutine(PlaceChildItem(tetrisItem, targetGrid, fromSlot));

        }

        /// <summary>
        /// 程序化将当前选中物品放置到指定容器的网格起始位置。
        /// </summary>
        /// <param name="tileGridOriginPosition">网格起始坐标（左上或定义的原点）。</param>
        /// <param name="container">目标容器，实现 IInventoryContainer 的实例。</param>
        private void PlaceItem(Vector2Int tileGridOriginPosition, IInventoryContainer container)
        {
            if (container == null) return;
            TetrisItemGrid tetrisItemGrid = container as TetrisItemGrid;
            selectedTetrisItem.CurrentInventoryContainer = tetrisItemGrid;
            bool isDone = tetrisItemGrid.TryPlaceTetrisItem(
                ref selectedTetrisItem,
                tileGridOriginPosition.x, 
                tileGridOriginPosition.y, 
                ref overlapItem);
            overlapItem = null;
        }

        /// <summary>
        /// 在活动背包中放置子物品，库存中不允许嵌套物品
        /// </summary>
        /// <param name="parentItem">父物品</param>
        /// <param name="targetGrid">目标网格</param>
        /// <param name="fromSlot">来源槽位</param>
        /// <returns>协程迭代器</returns>
        private IEnumerator PlaceChildItem(TetrisItem parentItem, TetrisItemGrid targetGrid, InventorySlot fromSlot)
        {
            if (!parentItem.TryGetItemComponent<GridPanelComponent>(out GridPanelComponent gridPanel)) yield break;
            yield return null;
            if (gridPanel.TetrisItemGrids.Count > 0)
            {
                foreach (TetrisItemGrid fromGrid in gridPanel.TetrisItemGrids)
                {
                    if (fromGrid.OwnerItemDic.Count > 0)
                    {
                        bool isDone = false;
                        Dictionary<int, TetrisItem> _ownerItemDic = new(fromGrid.OwnerItemDic);
                        foreach (TetrisItem item in _ownerItemDic.Values)
                        {
                            int _itemPosX = item.onGridPositionX;
                            int _itemPosY = item.onGridPositionY;
                            Vector2Int _rotationOffset = item.RotationOffset;
                            List<Vector2Int> _tetrisPieceShapePos = item.TetrisPieceShapePos;
                            for (int row = 0; row < targetGrid.gridSizeHeight; row++)
                            {
                                for (int column = 0; column < targetGrid.gridSizeWidth; column++)
                                {
                                    isDone = targetGrid.TryPlaceTetrisItem(item, column, row);
                                    if (isDone)
                                    {
                                        fromGrid.RemoveTetrisItem(item, _itemPosX, _itemPosY, _rotationOffset, _tetrisPieceShapePos);
                                        item.CurrentInventoryContainer = targetGrid;
                                        item.SetItemData(item.GetInstanceID());
                                        break;
                                    }
                                }
                                if (isDone)
                                {
                                    break;
                                }
                            }
                            if (!isDone)
                            {
                                Debug.Log("Nesting items is not allowed in inventory");
                                targetGrid.RemoveTetrisItem(parentItem, parentItem.onGridPositionX, parentItem.onGridPositionY, parentItem.RotationOffset, parentItem.TetrisPieceShapePos);
                                ResetParent(parentItem, fromSlot);
                                break;
                            }
                            yield return null;
                        }
                        if (!isDone) break;
                    }
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 重置父物品到原槽位
        /// </summary>
        /// <param name="parentItem">父物品</param>
        /// <param name="fromSlot">来源槽位</param>
        private void ResetParent(TetrisItem parentItem, InventorySlot fromSlot)
        {
            if (parentItem.CurrentInventoryContainer as InventorySlot == null)
            {
                fromSlot.PlaceTetrisItem(parentItem);
                parentItem.CurrentInventoryContainer = fromSlot;
                parentItem.SetItemData(parentItem.GetInstanceID());
            }
        }

        /// <summary>
        /// 创建新的可堆叠物品
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <returns>堆叠组件</returns>
        public StackableComponent CreateNewStackableItem(int itemID)
        {
            if (selectedTetrisItem) return null;
            selectedTetrisItem = Instantiate(itemDataList_SO.GetItemDetailsByID(itemID).uiPrefab).GetComponent<TetrisItem>();
            selectedTetrisItem.transform.SetParent(canvas.transform, false);
            selectedTetrisItem.transform.SetAsLastSibling();
            selectedTetrisItem.GetComponent<Image>().raycastTarget = false;
            TetrisItemGrid tetrisItemGrid = rightClickMenuPanel._currentItem.GetComponentInParent<TetrisItemGrid>();
            selectedTetrisItem.Initialize(itemDataList_SO.GetItemDetailsByID(itemID), null, tetrisItemGrid);
            selectedTetrisItem.TryGetItemComponent<StackableComponent>(out var stackableComponent);
            return stackableComponent;
        }

        /// <summary>
        /// 按顺序创建物品
        /// </summary>
        /// <param name="selectedItemIndex">选中的物品索引</param>
        private void CreateItemInOrder(int selectedItemIndex)
        {
            if (selectedTetrisItem) return;

            selectedTetrisItem = Instantiate(itemDataList_SO.GetItemDetailsByIndex(selectedItemIndex).uiPrefab).GetComponent<TetrisItem>();
            selectedTetrisItem.transform.SetParent(canvas.transform, false);
            selectedTetrisItem.transform.SetAsLastSibling();
            selectedTetrisItem.GetComponent<Image>().raycastTarget = false;
            selectedTetrisItem.Initialize(itemDataList_SO.itemDetailsList[selectedItemIndex], null, selectedTetrisItemGrid);
        }

        /// <summary>
        /// 获取鼠标下方的网格
        /// </summary>
        /// <returns>网格组件，如果没有则返回null</returns>
        private TetrisItemGrid GetGridUnderMouse()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("TetrisGrid"))
                {
                    return result.gameObject.GetComponent<TetrisItemGrid>();
                }
            }

            return null;
        }

    }
}