using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    /// <summary>
    /// 右键菜单面板
    /// 处理物品的右键操作菜单，提供查看、分割、使用等功能
    /// </summary>
    public class RightClickMenuPanel : MonoBehaviour
    {
        public TetrisItem _currentItem;
        public RectTransform menuRect;
        [SerializeField] private ItemInformationPanel ItemInformationPanel;
        [SerializeField] private Button CheckBtn, SplitBtn, UseBtn;
        [SerializeField] private float distanceThreshold = 150f;


        /// <summary>
        /// 初始化按钮事件监听
        /// </summary>
        private void Awake()
        {
            CheckBtn.onClick.AddListener(OnCheckBtnClick);
            SplitBtn.onClick.AddListener(OnSplitBtnClick);
            UseBtn.onClick.AddListener(OnUseBtnClick);
        }

        /// <summary>
        /// 检查鼠标距离，超出阈值时隐藏菜单
        /// </summary>
        private void Update()
        {
            if (Vector2.Distance(Input.mousePosition, transform.position) > distanceThreshold)
            {
                Show(false);
            }
        }

        /// <summary>
        /// 查看按钮点击事件
        /// </summary>
        private void OnCheckBtnClick()
        {
            Show(false);
            ItemInformationPanel.Show(true);
            ItemDetails itemDetails = _currentItem.ItemDetails;
            ItemInformationPanel.SetItemImage(newSprite: itemDetails?.itemUI ?? ItemInformationPanel.itemDefaultSprite);
            ItemInformationPanel.ItemName.text = itemDetails.itemName;
            ItemInformationPanel.ItemDescription.text = itemDetails.itemDescription;
        }

        /// <summary>
        /// 分割按钮点击事件
        /// </summary>
        private void OnSplitBtnClick()
        {
            if (_currentItem == null) return;
            _currentItem.TryGetItemComponent<StackableComponent>(out var stackableComponent);
            bool canSplit = stackableComponent.TrySplitStack(stackableComponent.CurrentStack / 2);
            if (canSplit)
            {
                _currentItem.SetItemData(_currentItem.GetInstanceID());
            }
            Show(false);
        }

        /// <summary>
        /// 使用按钮点击事件
        /// </summary>
        private void OnUseBtnClick()
        {
            Show(false);
            Debug.Log("UseBtn Clicked");
            // Implement your use logic here
        }

        /// <summary>
        /// 显示或隐藏菜单
        /// </summary>
        /// <param name="isClick">是否显示</param>
        public void Show(bool isClick)
        {
            gameObject.SetActive(isClick);
        }

        /// <summary>
        /// 根据物品类型设置按钮激活状态
        /// </summary>
        public void SetBtnActive()
        {
            if (_currentItem == null) return;
            _currentItem.TryGetItemComponent<StackableComponent>(out var stackableComponent);
            if (stackableComponent != null)
            {
                CheckBtn.gameObject.SetActive(true);
                SplitBtn.gameObject.SetActive(true);
                UseBtn.gameObject.SetActive(true);
            }
            else
            {
                CheckBtn.gameObject.SetActive(true);
                SplitBtn.gameObject.SetActive(false);
                UseBtn.gameObject.SetActive(true);
            }
        }

    }
}