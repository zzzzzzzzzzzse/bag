using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChosTIS.Utilities;

namespace ChosTIS
{
    /// <summary>
    /// 库存槽位类
    /// 负责管理特定类型的装备槽位，支持类型约束验证和物品放置
    /// </summary>
    public class InventorySlot : MonoBehaviour, IInventoryFunctionalContainer, IPointerEnterHandler, IPointerExitHandler
    {
        private TetrisItemGhost tetrisItemGhost;
        public TetrisItem RelatedTetrisItem { get; set; }
        public Transform GridPanelParent { get; set; }
        [Header("Configuration")]
        [SerializeField] private InventorySlotType inventorySlotType;
        [SerializeField] private Image activeUIImage;

        private void Start()
        {
            InstanceIDManager.Register(this);

            tetrisItemGhost = TetrisItemMediator.Instance.GetTetrisItemGhost();
            GridPanelParent = transform.parent.Find("GridPanel");
        }

        private void OnDestroy()
        {
            InstanceIDManager.Unregister(this);
        }

        /// <summary>
        /// 鼠标进入槽位时设置当前槽位
        /// </summary>
        /// <param name="eventData">鼠标事件数据</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            tetrisItemGhost.CurrentSlot = this;
        }

        /// <summary>
        /// 鼠标离开槽位时清除当前槽位
        /// </summary>
        /// <param name="eventData">鼠标事件数据</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            tetrisItemGhost.CurrentSlot = null;
        }

        /// <summary>
        /// 尝试放置物品到槽位，检查类型匹配
        /// </summary>
        /// <param name="tetrisItem">要放置的物品</param>
        /// <returns>是否成功放置</returns>
        public bool TryPlaceTetrisItem(TetrisItem tetrisItem)
        {
            if (tetrisItem.InventorySlotType == inventorySlotType)
            {
                PlaceTetrisItem(tetrisItem);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将传入的物品放置到当前槽位，并调整其层级与尺寸。
        /// </summary>
        /// <param name="tetrisItem">要放置的物品。</param>
        public void PlaceTetrisItem(TetrisItem tetrisItem)
        {
            RelatedTetrisItem = tetrisItem;
            tetrisItem.transform.SetParent(transform, false);
            transform.GetChild(0).GetComponent<Image>().enabled = false;
            tetrisItem.transform.SetPositionAndRotation(
                transform.GetChild(0).position,
                    Quaternion.Euler(0, 0, 0));
            tetrisItem.GetComponent<RectTransform>().sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
            tetrisItem.TryGetItemComponent<GridPanelComponent>(out var gridPanelComponent);
            gridPanelComponent.SetGridPanelParent(GridPanelParent);
        }

        /// <summary>
        /// 移除槽位中的物品
        /// </summary>
        public void RemoveTetrisItem()
        {
            RelatedTetrisItem.TryGetItemComponent<GridPanelComponent>(out var gridPanelComponent);
            gridPanelComponent.SetGridPanelParent(RelatedTetrisItem.transform);
            RelatedTetrisItem = null;
            transform.GetChild(0).GetComponent<Image>().enabled = true;
        }

        /// <summary>
        /// 判断当前槽位是否已有物品。
        /// </summary>
        /// <returns>如果已有物品则返回 true，否则返回 false。</returns>
        public bool HasItem()
        {
            return RelatedTetrisItem != null;
        }

        /// <summary>
        /// 获取槽位内部用于定位物品的 RectTransform。
        /// </summary>
        /// <returns>槽位子节点的 RectTransform。</returns>
        public RectTransform GetSlotRectTransform()
        {
            return transform.GetChild(0).GetComponent<RectTransform>();
        }

        /// <summary>
        /// 获取槽位类型（如武器槽、护甲槽等）。
        /// </summary>
        /// <returns>槽位类型枚举值。</returns>
        public InventorySlotType GetInventorySlotType()
        {
            return inventorySlotType;
        }
    }
}