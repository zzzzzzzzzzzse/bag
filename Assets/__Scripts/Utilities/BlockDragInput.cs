using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDragInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��ֹ�¼����ݵ�ScrollRect
        eventData.pointerDrag = null;
    }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) { }
}
