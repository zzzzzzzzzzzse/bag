using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 阻止拖拽输入组件
/// 用于阻止拖拽事件传播到父级UI元素，避免与ScrollRect等组件冲突
/// </summary>
public class BlockDragInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 开始拖拽时阻止事件传播
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��ֹ�¼����ݵ�ScrollRect
        eventData.pointerDrag = null;
    }
    /// <summary>
    /// 拖拽过程中不处理
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnDrag(PointerEventData eventData) { }
    /// <summary>
    /// 结束拖拽时不处理
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnEndDrag(PointerEventData eventData) { }
}
