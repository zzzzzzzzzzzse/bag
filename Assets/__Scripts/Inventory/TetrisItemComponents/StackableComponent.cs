using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    /// <summary>
    /// 物品的堆叠组件：管理最大堆叠、当前堆叠以及合并/拆分逻辑。
    /// </summary>
    public class StackableComponent : MonoBehaviour, IItemComponent, IStackable
    {
        public int MaxStack { get; set; }
        public int CurrentStack { get; set; }
        public Text stackNumText;
        private TetrisItem _owner;

        /// <summary>
        /// 当组件挂载到物品时回调，记录宿主引用。
        /// </summary>
        /// <param name="item">宿主物品。</param>
        public void OnAttach(TetrisItem item) => _owner = item;
        /// <summary>
        /// 当组件从物品卸载时回调，清除宿主引用。
        /// </summary>
        public void OnDetach() => _owner = null;

        /// <summary>
        /// 初始化堆叠数据并刷新 UI 文本显示。
        /// </summary>
        /// <param name="itemData">可选的存档条目；若为 null 则默认满堆叠。</param>
        public void Initialize(GridItem itemData)
        {
            MaxStack = _owner.ItemDetails.maxStack;
            if (itemData != null)
            {
                CurrentStack = itemData.stack;
            }
            else
            {
                CurrentStack = MaxStack;
            }
            if (stackNumText == null)
            {
                stackNumText = transform.Find("StackNum").GetComponent<Text>();
            }
            if (stackNumText != null)
            {
                stackNumText.text = CurrentStack.ToString();
            }
        }

        /// <summary>
        /// 尝试将另一个堆叠的数量合并到当前堆叠，受最大堆叠限制。
        /// </summary>
        /// <param name="other">另一个堆叠组件。</param>
        /// <returns>成功合并返回 true，若当前已满或无法合并则返回 false。</returns>
        public bool TryMergeStack(StackableComponent other)
        {
            if (CurrentStack >= MaxStack) return false;

            var transfer = Mathf.Min(other.CurrentStack, MaxStack - CurrentStack);
            CurrentStack += transfer;
            other.CurrentStack -= transfer;
            UpdateCurrentStackText(this);
            UpdateCurrentStackText(other);
            return true;
        }

        /// <summary>
        /// 尝试拆分当前堆叠，创建新的堆叠物品
        /// </summary>
        /// <param name="amount">要拆分的数量</param>
        /// <returns>拆分成功返回true，否则返回false</returns>
        public bool TrySplitStack(int amount)
        {
            if (CurrentStack <= 1 || amount <= 0 || amount >= CurrentStack) return false;
            StackableComponent newStack = InventoryManager.Instance.CreateNewStackableItem(_owner.ItemDetails.itemID);
            newStack.CurrentStack = amount;
            CurrentStack -= amount;
            UpdateCurrentStackText(this);
            UpdateCurrentStackText(newStack);
            return true;
        }

        /// <summary>
        /// 更新堆叠数量显示文本
        /// </summary>
        /// <param name="stackableComponent">堆叠组件</param>
        private void UpdateCurrentStackText(StackableComponent stackableComponent)
        {
            if (stackableComponent.stackNumText != null)
            {
                stackableComponent.stackNumText.text = stackableComponent.CurrentStack.ToString();
            }
        }
    }
}