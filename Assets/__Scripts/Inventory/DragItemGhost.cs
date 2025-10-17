using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChosTIS.Utilities;

namespace ChosTIS
{
    /// <summary>
    /// 物品拖拽时的幽灵预览体：负责可视化、拖拽交互、放置状态判定与最终应用。
    /// </summary>
    public class TetrisItemGhost : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, ITetrisRotatable
    {
        //Private fields
        public PlaceState PlaceState { get; set; } = PlaceState.InvalidPos;
        public bool OnDragging { get; set; } = false;
        public TetrisItemGrid selectedTetrisItemOrginGrid;
        public TetrisItem selectedTetrisItem;
        private RectTransform ghostRect;
        private Image ghostImage;
        private CanvasGroup canvasGroup;
        private Vector2Int oldPosition;
        private TetrisItem overlapItem;
        [SerializeField] RightClickMenuPanel tetrisItemMenuPanel;

        //Public fields
        public ItemDetails ItemDetails { get; set; }
        public int onGridPositionX;
        public int onGridPositionY;

        //Rotate fields
        public bool Rotated { get; set; } = false;
        public Dir Dir { get; set; } = Dir.Down;
        public Vector2Int RotationOffset { get; set; }
        public List<Vector2Int> TetrisPieceShapePos { get; set; }

        public int WIDTH => Rotated ? ItemDetails.yHeight : ItemDetails.xWidth;
        public int HEIGHT => Rotated ? ItemDetails.xWidth : ItemDetails.yHeight;

        public InventorySlot CurrentSlot { get; set; }

        private void Awake()
        {
            ghostRect = GetComponent<RectTransform>();
            ghostImage = GetComponent<Image>();
            canvasGroup = GetComponent<CanvasGroup>();
            ghostImage.alphaHitTestMinimumThreshold = 0.1f;
        }

        void Update()
        {
            if (InventoryManager.Instance.selectedTetrisItemGrid != null)
            {
                Vector2Int positionOnGrid = InventoryManager.Instance.GetGhostTileGridOriginPosition();

                onGridPositionX = positionOnGrid.x;
                onGridPositionY = positionOnGrid.y;
            }

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                tetrisItemMenuPanel.menuRect.position = selectedTetrisItem.transform.position;
                tetrisItemMenuPanel._currentItem = selectedTetrisItem;
                tetrisItemMenuPanel.Show(true);
                tetrisItemMenuPanel.SetBtnActive();
            }

        }

        /// <summary>
        /// 开始拖拽幽灵体：禁用射线阻挡、加深透明度并缓存源物品与幽灵状态。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            canvasGroup.blocksRaycasts = false;
            OnDragging = true;
            ghostImage.color = new Color(1, 1, 1, 0.8f);
            //Cache source status
            TetrisItemMediator.Instance.CacheItemState(selectedTetrisItem);
            TetrisItemMediator.Instance.CacheGhostState(this);
            OnBeginAction(eventData);

        }

        /// <summary>
        /// 拖拽过程中根据屏幕坐标更新幽灵体的局部位置。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            //ghostRect.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                ghostRect.parent as RectTransform,
                eventData.position,
                null,
                //canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint))
            {
                ghostRect.localPosition = localPoint;
            }
        }

        /// <summary>
        /// 结束拖拽：恢复射线阻挡、隐藏幽灵体并触发放置状态更新。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            OnDragging = false;
            ghostImage.color = new Color(1, 1, 1, 0);
            OnEndAction(eventData);
            UpdatePlaceState();
        }

        /// <summary>
        /// 旋转幽灵体：更新方向、旋转偏移与形状点集，并同步中介器缓存。
        /// </summary>
        public void Rotate()
        {
            Dir = Utilities.RotationHelper.GetNextDir(Dir);
            Rotated = !Rotated;
            RotationOffset = Utilities.RotationHelper.GetRotationOffset(Dir, WIDTH, HEIGHT);
            TetrisPieceShapePos = Utilities.RotationHelper.RotatePointsClockwise(TetrisPieceShapePos);
            TetrisItemMediator.Instance.CacheGhostState(this);
            transform.rotation = Quaternion.Euler(0, 0, -Utilities.RotationHelper.GetRotationAngle(Dir));
        }

        /// <summary>
        /// 拖拽开始的业务处理：根据射线命中移除源格子或槽位中的物品。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        private void OnBeginAction(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                GameObject target = result.gameObject;
                if (target.CompareTag("TetrisGrid"))
                {
                    target.GetComponent<TetrisItemGrid>().RemoveTetrisItem(
                        selectedTetrisItem,
                        selectedTetrisItem.onGridPositionX,
                        selectedTetrisItem.onGridPositionY,
                        selectedTetrisItem.RotationOffset,
                        selectedTetrisItem.TetrisPieceShapePos);
                }
                else if (target.CompareTag("TetrisSlot"))
                {
                    target.GetComponent<InventorySlot>().RemoveTetrisItem();
                }
            }
        }

        private void OnEndAction(PointerEventData eventData)
        {
            // Radiographic inspection of all UI objects
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                GameObject target = result.gameObject;

                // Skip oneself
                if (target == gameObject) continue;

                // Determine the target type
                if (target.CompareTag("TetrisGrid"))
                {
                    if (!target.GetComponent<TetrisItemGrid>().BoundryCheck(
                           onGridPositionX, onGridPositionY,
                           WIDTH, HEIGHT))
                    {
                        PlaceState = PlaceState.InvalidPos;
                        return;
                    }

                    foreach (Vector2Int v2i in TetrisPieceShapePos)
                    {
                        PlaceState = target.GetComponent<TetrisItemGrid>().HasItem(
                            onGridPositionX + v2i.x + RotationOffset.x,
                            onGridPositionY + v2i.y + RotationOffset.y) ?
                        PlaceState.OnGridHasItem : PlaceState.OnGridNoItem;
                        if (PlaceState == PlaceState.OnGridHasItem)
                        {
                            if (overlapItem == null)
                            {
                                overlapItem = target.GetComponent<TetrisItemGrid>().GetTetrisItem(
                                onGridPositionX + v2i.x + RotationOffset.x,
                                onGridPositionY + v2i.y + RotationOffset.y);
                                return;
                            }
                            else
                            {
                                //If you find multiple overlapping items in the range
                                if (overlapItem != target.GetComponent<TetrisItemGrid>().GetTetrisItem(
                                onGridPositionX + v2i.x + RotationOffset.x,
                                onGridPositionY + v2i.y + RotationOffset.y))
                                {
                                    overlapItem = null;
                                    return;
                                }
                            }
                        }
                    }
                    return;
                }
                else if (target.CompareTag("TetrisSlot"))
                {

                    PlaceState = target.GetComponent<InventorySlot>().HasItem() ?
                        PlaceState.OnSlotHasItem : PlaceState.OnSlotNoItem;
                    return;
                }

                // No valid area detected
                PlaceState = PlaceState.InvalidPos;
            }
        }

        /// <summary>
        /// 根据当前放置状态枚举执行具体落点逻辑（叠加、落格、落槽、重置）。
        /// </summary>
        private void UpdatePlaceState()
        {
            switch (PlaceState)
            {
                case PlaceState.OnGridHasItem:
                    PlaceOnOverlapItem(selectedTetrisItem);
                    break;
                case PlaceState.OnSlotHasItem:
                    ResetState(selectedTetrisItem);
                    break;
                case PlaceState.OnGridNoItem:
                    PlaceOnGrid(selectedTetrisItem);
                    break;
                case PlaceState.OnSlotNoItem:
                    PlaceOnSlot(selectedTetrisItem);
                    break;
                case PlaceState.InvalidPos:
                    ResetState(selectedTetrisItem);
                    break;
            }
        }

        /// <summary>
        /// 将物品落到目标网格：同步中介器状态、调用放置接口并更新位置与存档。
        /// </summary>
        /// <param name="selectedTetrisItem">被拖拽的源物品。</param>
        private void PlaceOnGrid(TetrisItem selectedTetrisItem)
        {
            TetrisItemMediator.Instance.ApplyStateToItem(selectedTetrisItem);
            InventoryManager.Instance.PlaceGhostItem(
                InventoryManager.Instance.GetGhostTileGridOriginPosition(),
                selectedTetrisItem,
                InventoryManager.Instance.selectedTetrisItemGrid,
                selectedTetrisItem.CurrentInventoryContainer as InventorySlot);
            selectedTetrisItem.CurrentInventoryContainer = InventoryManager.Instance.selectedTetrisItemGrid;
            transform.position = selectedTetrisItem.transform.position;
            selectedTetrisItem.SetItemData(selectedTetrisItem.GetInstanceID());
        }

        /// <summary>
        /// 将物品落到槽位：移除原容器并调用槽位放置，更新位置与存档。
        /// </summary>
        /// <param name="selectedTetrisItem">被拖拽的源物品。</param>
        private void PlaceOnSlot(TetrisItem selectedTetrisItem)
        {
            TetrisItemMediator.Instance.ApplyStateToItem(selectedTetrisItem);
            bool canPlace = CurrentSlot.TryPlaceTetrisItem(selectedTetrisItem);
            if (canPlace)
            {
                selectedTetrisItem.CurrentInventoryContainer = CurrentSlot;
                transform.position = CurrentSlot.transform.position;
                selectedTetrisItem.SetItemData(selectedTetrisItem.GetInstanceID());
            }
            else
            {
                ResetState(selectedTetrisItem);
            }
        }

        private void PlaceOnOverlapItem(TetrisItem selectedTetrisItem)
        {
            bool canStack = overlapItem != null
                && overlapItem.ItemDetails.itemID == selectedTetrisItem.ItemDetails.itemID
                && selectedTetrisItem.ItemDetails.maxStack != 0
                && Utilities.TetrisItemUtilities.TryStackItems(overlapItem, selectedTetrisItem);

            if (canStack)
            {
                selectedTetrisItem.SetItemData(selectedTetrisItem.GetInstanceID());
                overlapItem.SetItemData(overlapItem.GetInstanceID());
                selectedTetrisItem.TryGetItemComponent<StackableComponent>(out StackableComponent stackableComponent);
                if (stackableComponent.CurrentStack <= 0)
                {
                    TetrisItemGrid otherGrid = (TetrisItemGrid)selectedTetrisItem.CurrentInventoryContainer;
                    otherGrid.RemoveTetrisItem(selectedTetrisItem,
                        selectedTetrisItem.onGridPositionX,
                        selectedTetrisItem.onGridPositionY,
                        selectedTetrisItem.RotationOffset,
                        selectedTetrisItem.TetrisPieceShapePos);
                    selectedTetrisItem.RemoveItemData(selectedTetrisItem.GetInstanceID());
                    Destroy(selectedTetrisItem.gameObject);
                    TetrisItemUtilities.TriggerPointerEnter(overlapItem.gameObject);
                    overlapItem = null;
                    return;
                }
            }

            ResetState(selectedTetrisItem);
            if (overlapItem != null) TetrisItemUtilities.TriggerPointerEnter(overlapItem.gameObject);
            overlapItem = null;
        }

        private void ResetState(TetrisItem selectedTetrisItem)
        {
            TetrisItemMediator.Instance.ApplyStateToGhost(this);
            if (selectedTetrisItem.IsItemLocationOnGrid())
            {
                InventoryManager.Instance.PlaceGhostItem(new Vector2Int(
                    selectedTetrisItem.onGridPositionX,
                    selectedTetrisItem.onGridPositionY),
                    selectedTetrisItem,
                    selectedTetrisItem.CurrentInventoryContainer as TetrisItemGrid,
                    selectedTetrisItem.CurrentInventoryContainer as InventorySlot);
            }
            else
            {
                InventorySlot slot = selectedTetrisItem.CurrentInventoryContainer as InventorySlot;
                slot.TryPlaceTetrisItem(selectedTetrisItem);
            }
            transform.position = selectedTetrisItem.transform.position;

        }

        public void InitializeFromItem(TetrisItem item)
        {
            if (item == null) return;
            if (!OnDragging)
            {
                // Synchronize basic attributes
                transform.SetPositionAndRotation(
                    item.transform.position,
                    Quaternion.Euler(0, 0, -Utilities.RotationHelper.GetRotationAngle(item.Dir))
                );
                ghostImage.sprite = item.GetComponent<Image>().sprite;
                ghostRect.sizeDelta = item.GetComponent<RectTransform>().sizeDelta;

                // Synchronize business logic attributes
                selectedTetrisItemOrginGrid = item.CurrentInventoryContainer as TetrisItemGrid;
                selectedTetrisItem = item;
                ItemDetails = item.ItemDetails;
                Rotated = item.Rotated;
                Dir = item.Dir;
                RotationOffset = item.RotationOffset;
                onGridPositionX = item.onGridPositionX;
                onGridPositionY = item.onGridPositionY;
                TetrisPieceShapePos = item.TetrisPieceShapePos;
            }
        }

    }
}