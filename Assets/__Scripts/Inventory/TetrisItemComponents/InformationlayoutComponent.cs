using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS
{
    /// <summary>
    /// 信息布局组件
    /// 负责在物品上显示基本信息，如名称和堆叠数量
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class InformationlayoutComponent : MonoBehaviour, IItemComponent, IInformation
    {
        private Image bgImage;
        private RectTransform rectTransform;
        private Text itemName;
        private Text StackNum;



        private TetrisItem _owner;
        
        /// <summary>
        /// 组件附加到物品时调用
        /// </summary>
        /// <param name="item">目标物品</param>
        public void OnAttach(TetrisItem item) => _owner = item;
        
        /// <summary>
        /// 组件从物品分离时调用
        /// </summary>
        public void OnDetach() => _owner = null;

        /// <summary>
        /// 初始化信息布局组件
        /// </summary>
        public void Initialize()
        {

        }




    }
}