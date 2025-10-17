using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChosTIS.Utilities;
using ChosTIS.SaveLoadSystem;

namespace ChosTIS
{
    /// <summary>
    /// 背包中的基础物品对象：负责自身尺寸、旋转状态、组件挂载与数据同步。
    /// </summary>
    public class TetrisItem : MonoBehaviour, IPointerEnterHandler, ITetrisRotatable
    {
        //Private fields
        private List<IItemComponent> _components = new();
        private Image image;

        //Public fields
        public ItemDetails ItemDetails { get; set; }
        public IInventoryContainer CurrentInventoryContainer { get; set; }
        public InventorySlotType InventorySlotType { get; set; } = InventorySlotType.Pocket;
        public int onGridPositionX;
        public int onGridPositionY;

        //Rotate fields
        public bool Rotated { get; set; } = false;
        public Dir Dir { get; set; } = Dir.Down;
        public Vector2Int RotationOffset { get; set; }
        public List<Vector2Int> TetrisPieceShapePos { get; set; }

        public int WIDTH => Rotated ? ItemDetails.yHeight : ItemDetails.xWidth;
        public int HEIGHT => Rotated ? ItemDetails.xWidth : ItemDetails.yHeight;

        private void Awake()
        {
            InstanceIDManager.Register(this);
            image = GetComponent<Image>();
        }

        private void OnDestroy()
        {
            InstanceIDManager.Unregister(this);
            foreach (var component in _components)
                component.OnDetach();
        }

        /// <summary>
        /// 初始化物品的静态属性、旋转点集、尺寸与挂载的业务组件。
        /// </summary>
        /// <param name="itemDetails">物品静态数据（尺寸、形状、槽位类型等）。</param>
        /// <param name="itemData">用于堆叠等的运行时存档数据。</param>
        /// <param name="grid">初始所在的网格容器。</param>
        public void Initialize(ItemDetails itemDetails, GridItem itemData, TetrisItemGrid grid)
        {
            image.alphaHitTestMinimumThreshold = 0.1f;
            image.raycastPadding = new Vector4(-1, -1, -1, -1);
            ItemDetails = itemDetails;
            InventorySlotType = itemDetails.inventorySlotType;
            CurrentInventoryContainer = grid;
            TetrisPieceShapePos = Utilities.RotationHelper.RotatePoints(InventoryManager.Instance.GetTetrisPieceShapePos(itemDetails.tetrisPieceShape), Dir);
            RotationOffset = RotationHelper.GetRotationOffset(Dir, WIDTH, HEIGHT);
            //Modify item size
            Vector2 size = new Vector2();
            size.x = itemDetails.xWidth * TetrisItemGrid.tileSizeWidth;
            size.y = itemDetails.yHeight * TetrisItemGrid.tileSizeHeight;
            GetComponent<RectTransform>().sizeDelta = size;
            //Initialize item components
            AddItemComponent<InformationlayoutComponent>().Initialize();
            if (itemDetails.maxStack > 0) AddItemComponent<StackableComponent>().Initialize(itemData);
            if (itemDetails.gridUIPrefab != null) AddItemComponent<GridPanelComponent>().Initialize();

        }

        /// <summary>
        /// 指针进入物品时，同步幽灵预览以便开始拖拽或显示高亮。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Update only when not selected
            TetrisItemMediator.Instance.SyncGhostFromItem(this);
        }

        /// <summary>
        /// 为物品挂载一个业务组件并完成绑定流程。
        /// </summary>
        /// <typeparam name="T">组件类型，需同时继承 Component 与 IItemComponent。</typeparam>
        /// <returns>挂载后的组件实例。</returns>
        public T AddItemComponent<T>() where T : Component, IItemComponent
        {
            var component = gameObject.AddComponent<T>();
            component.OnAttach(this);
            _components.Add(component);
            return component;
        }

        /// <summary>
        /// 移除指定类型的业务组件，并执行解绑与销毁。
        /// </summary>
        /// <typeparam name="T">组件类型，实现 IItemComponent。</typeparam>
        public void RemoveItemComponent<T>() where T : IItemComponent
        {
            var component = GetComponent<T>();
            if (component != null)
            {
                component.OnDetach();
                _components.Remove(component);
                Destroy(component as Component);
            }
        }

        /// <summary>
        /// 尝试获取指定类型的业务组件。
        /// </summary>
        /// <typeparam name="T">组件类型，实现 IItemComponent。</typeparam>
        /// <param name="result">输出的组件实例，若不存在则为 null。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool TryGetItemComponent<T>(out T result) where T : IItemComponent
        {
            result = GetComponent<T>();
            return result != null;
        }

        /// <summary>
        /// 判断物品当前是否位于网格容器中。
        /// </summary>
        /// <returns>在网格中返回 true，否则返回 false。</returns>
        public bool IsItemLocationOnGrid()
        {
            if (CurrentInventoryContainer is TetrisItemGrid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 通过物品的运行时索引在存档数据中检索 GridItem，并同步当前物品状态。
        /// </summary>
        /// <param name="itemIndex">物品的运行时索引（InstanceID）。</param>
        public void SetItemData(int itemIndex)
        {
            GridItem gridItem = InventorySaveLoadService.Instance.inventoryData_SO.GetGridItem(itemIndex);
            if (IsItemLocationOnGrid())
            {
                if (gridItem != null)
                {
                    SetGridItemData(gridItem);
                }
                else
                {
                    GridItem item = new GridItem();
                    SetGridItemData(item);
                    InventorySaveLoadService.Instance.inventoryData_SO.inventoryItemList.Add(item);
                }
            }
            else
            {
                SetGridItemData(gridItem);
            }
        }

        public void RemoveItemData(int itemIndex)
        {
            GridItem gridItem = InventorySaveLoadService.Instance.inventoryData_SO.GetGridItem(itemIndex);
            if (gridItem != null)
            {
                InventorySaveLoadService.Instance.inventoryData_SO.inventoryItemList.Remove(gridItem);
            }
        }

        /// <summary>
        /// 将当前物品的相关字段写入到 GridItem，用于保存或运行时同步。
        /// </summary>
        /// <param name="gridItem">目标存档条目对象。</param>
        private void SetGridItemData(GridItem gridItem)
        {
            gridItem.itemID = ItemDetails.itemID;
            gridItem.itemIndex = GetInstanceID();
            gridItem.parentItemIndex = transform.parent?.GetComponent<TetrisItemGrid>()?.RelatedTetrisItem?.GetInstanceID() ?? 0;
            gridItem.gridPIndex = transform?.parent.GetSiblingIndex() ?? -1;
            gridItem.isParent = transform.GetComponentInChildren<Transform>(true).CompareTag("GridPanel");
            gridItem.isOnSlot = !IsItemLocationOnGrid();
            gridItem.persistentGridTypeIndex = transform.parent?.GetComponent<TetrisItemGrid>()?.GetPersistentGridIdentification()?.GetPersistentGridTypeIndex() ?? 0;
            gridItem.orginPosition = new Vector2Int(onGridPositionX, onGridPositionY);
            gridItem.direction = Dir;
            if (TryGetItemComponent<StackableComponent>(out var stackableComponent))
            {
                gridItem.stack = stackableComponent.CurrentStack;
            }
        }

    }
}
