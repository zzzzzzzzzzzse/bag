using System.Collections.Generic;
using UnityEngine;

namespace ChosTIS
{
    /// <summary>
    /// 网格面板组件
    /// 为物品提供子网格功能，支持嵌套库存结构
    /// </summary>
    public class GridPanelComponent : MonoBehaviour, IItemComponent, IGridPanel
    {
        public Transform GridPanel { get; set; }
        public List<TetrisItemGrid> TetrisItemGrids { get; set; } = new();
        private TetrisItem _owner;

        /// <summary>
        /// 组件附加到物品时调用
        /// </summary>
        /// <param name="item">目标物品</param>
        public void OnAttach(TetrisItem item) => _owner = item;
        
        /// <summary>
        /// 组件从物品分离时调用，销毁网格面板
        /// </summary>
        public void OnDetach() => Destroy(GridPanel?.gameObject);

        /// <summary>
        /// 初始化网格面板组件
        /// </summary>
        public void Initialize()
        {
            if (_owner.ItemDetails.gridUIPrefab == null) return;

            GridPanel = Instantiate(_owner.ItemDetails.gridUIPrefab);
            GridPanel.SetParent(_owner.transform, false);
            GridPanel.GetComponent<RectTransform>().localPosition = new Vector3(60, 0, 0);
            GridPanel.gameObject.SetActive(false);

            TetrisItemGrids.AddRange(GridPanel.GetComponentsInChildren<TetrisItemGrid>(true));
            foreach (var subGrid in TetrisItemGrids)
                subGrid.RelatedTetrisItem = _owner;
        }

        /// <summary>
        /// 设置网格面板的父对象
        /// </summary>
        /// <param name="parent">新的父对象</param>
        public void SetGridPanelParent(Transform parent)
        {
            if (GridPanel == null) return;
            GridPanel.SetParent(parent, false);
            GridPanel.GetComponent<RectTransform>().localPosition = new Vector3(60, 0, 0);
            GridPanel.gameObject.SetActive(parent != _owner.transform);
        }

    }
}