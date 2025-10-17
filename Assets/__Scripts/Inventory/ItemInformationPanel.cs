using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChosTIS
{
    /// <summary>
    /// 物品信息面板：支持拖拽移动、展示图片与文案，并限制在画布范围内。
    /// </summary>
    public class ItemInformationPanel : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public Image itemImage;
        public Sprite itemDefaultSprite;
        public Text ItemName;
        public Text ItemDescription;
        [SerializeField] private Button CloseBtn;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private RectTransform itemImageParentRect;
        [SerializeField] private RectTransform itemImageRect;
        [SerializeField] private Canvas canvas;
        private Vector2 offset;

        private void Awake()
        {
            CloseBtn.onClick.AddListener(OnCloseBtnClick);
        }

        private void OnCloseBtnClick()
        {
            Show(false);
        }

        /// <summary>
        /// 开始拖拽面板：计算拖拽偏移量，用于保持光标与面板位置关系。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPoint);
            offset = panelRect.anchoredPosition - localPoint;
        }

        /// <summary>
        /// 拖拽中：根据屏幕坐标更新面板锚点位置，并限制在画布范围内。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPoint))
            {
                panelRect.anchoredPosition = localPoint + offset;
            }
            ClampToCanvas();
        }


        /// <summary>
        /// 显示或隐藏信息面板。
        /// </summary>
        /// <param name="isShow">为 true 则显示，否则隐藏。</param>
        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        /// <summary>
        /// 设置面板中的物品图片，并在超出父容器约束时按比例收缩。
        /// </summary>
        /// <param name="newSprite">新图片资源。</param>
        public void SetItemImage(Sprite newSprite)
        {
            itemImage.sprite = newSprite;
            itemImage.SetNativeSize();

            Vector2 maxSize = new Vector2(
                itemImageParentRect.rect.width,
                itemImageParentRect.rect.height
            );

            if(itemImageRect.sizeDelta.x > maxSize.x || itemImageRect.sizeDelta.y > maxSize.y)
            {
                float widthRatio = maxSize.x / itemImageRect.sizeDelta.x;
                float heightRatio = maxSize.y / itemImageRect.sizeDelta.y;
                float scaleFactor = Mathf.Min(widthRatio, heightRatio);

                itemImageRect.sizeDelta *= scaleFactor;
            }

        }
        /// <summary>
        /// 限制面板位置在画布范围内，避免拖拽导致超出可视区域。
        /// </summary>
        private void ClampToCanvas()
        {
            Vector3 pos = panelRect.localPosition;
            Vector2 canvasSize = canvas.GetComponent<RectTransform>().rect.size;
            Vector2 panelSize = panelRect.rect.size;

            pos.x = Mathf.Clamp(pos.x,
                -canvasSize.x / 2 + panelSize.x / 2,
                canvasSize.x / 2 - panelSize.x / 2);
            pos.y = Mathf.Clamp(pos.y,
                -canvasSize.y / 2 + panelSize.y / 2,
                canvasSize.y / 2 - panelSize.y / 2);

            panelRect.localPosition = pos;
        }

    }
}