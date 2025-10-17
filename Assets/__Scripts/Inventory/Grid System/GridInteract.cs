using UnityEngine;
using UnityEngine.EventSystems;

namespace ChosTIS
{
    /// <summary>
    /// 处理网格的指针进入/离开事件，用于更新当前选中的 TetrisItemGrid。
    /// </summary>
    [RequireComponent(typeof(TetrisItemGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private InventoryManager inventoryManager;
        [SerializeField] private TetrisItemGrid tetrisItemGrid;

        private void Awake()
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            tetrisItemGrid = GetComponent<TetrisItemGrid>();
        }

        /// <summary>
        /// 指针进入当前网格时回调，将该网格设为选中。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryManager.selectedTetrisItemGrid = tetrisItemGrid;
        }

        /// <summary>
        /// 指针离开当前网格时回调，清除选中网格引用。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryManager.selectedTetrisItemGrid = null;
        }
    }
}