using UnityEngine;

namespace ChosTIS
{
    /// <summary>
    /// 槽位布局控制器
    /// 动态调整槽位和网格面板的大小，根据子元素数量自动调整布局
    /// </summary>
    public class SlotLayoutController : MonoBehaviour
    {
        public RectTransform slotRect;
        public RectTransform gridPanelRect;
        private int childCount = 0;
        private bool isRemoved = false;

        /// <summary>
        /// 每帧检查子元素变化并调整布局
        /// </summary>
        private void Update()
        {
            if (gridPanelRect.childCount < childCount) isRemoved = true;

            if (HasGrid(gridPanelRect) && !isRemoved)
            {
                SetUp();
            }

            if (isRemoved)
            {
                gridPanelRect.sizeDelta = new Vector2(
                    gridPanelRect.rect.width,
                    50f);
                slotRect.sizeDelta = new Vector2(slotRect.rect.width, gridPanelRect.rect.height + 10f);
                isRemoved = false;
            }
        }

        /// <summary>
        /// 设置槽位和网格面板的大小
        /// </summary>
        public void SetUp()
        {
            gridPanelRect.sizeDelta = new Vector2(
                gridPanelRect.rect.width,
                gridPanelRect.GetChild(0).GetComponent<RectTransform>().rect.height);
            slotRect.sizeDelta = new Vector2(slotRect.rect.width, gridPanelRect.rect.height + 10f);
        }

        /// <summary>
        /// 检查网格面板是否有子元素变化
        /// </summary>
        /// <param name="gridPanel">网格面板</param>
        /// <returns>是否有变化</returns>
        private bool HasGrid(RectTransform gridPanel)
        {
            //if (gridPanel.childCount == 0) return false;
            if (gridPanel.childCount != childCount)
            {
                childCount = gridPanel.childCount;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}