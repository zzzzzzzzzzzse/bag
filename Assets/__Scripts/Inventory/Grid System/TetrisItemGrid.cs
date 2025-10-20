using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChosTIS.Utilities;

namespace ChosTIS
{
    /// <summary>
    /// 以网格坐标管理物品放置与移除、重叠检测与坐标转换的核心容器。
    /// </summary>
    public class TetrisItemGrid : MonoBehaviour, IInventoryContainer, IPointerEnterHandler, IPointerExitHandler
    {
        private TetrisItem[,] itemSlot;
        public TetrisItem RelatedTetrisItem { get; set; }
        public Dictionary<int, TetrisItem> OwnerItemDic { get; set; } = new();

        [Header("Grid Unit Size")]
        public const float tileSizeWidth = 20;
        public const float tileSizeHeight = 20;
        [Header("Grid Size")]
        public int gridSizeWidth = 1;
        public int gridSizeHeight = 1;
        [Header("Component")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Canvas canvas;
        [SerializeField] private InventoryManager inventoryManager;

        // Calculate the position in the Grid panel
        private Vector2 positionOnTheGrid = new Vector2();
        // Calculate the coordinates in the Grid panel
        private Vector2Int tileGridPosition = new Vector2Int();

        private void Start()
        {
            InstanceIDManager.Register(this);

            itemSlot = new TetrisItem[gridSizeWidth, gridSizeHeight];

            rectTransform = GetComponent<RectTransform>();
            canvas = FindObjectOfType<Canvas>();
            inventoryManager = FindObjectOfType<InventoryManager>();

            Initialize(gridSizeWidth, gridSizeHeight);
        }

        private void OnDestroy()
        {
            InstanceIDManager.Unregister(this);
        }

        /// <summary>
        /// 指针进入当前网格时，将该网格设为选中用于高亮与放置计算。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryManager.selectedTetrisItemGrid = this;
        }

        /// <summary>
        /// 指针离开当前网格时，清除选中状态以避免误用。
        /// </summary>
        /// <param name="eventData">指针事件数据。</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryManager.selectedTetrisItemGrid = null;
        }

        /// <summary>
        /// 初始化网格的 RectTransform 尺寸（按网格宽高与单元尺寸计算）。
        /// </summary>
        /// <param name="width">网格列数。</param>
        /// <param name="height">网格行数。</param>
        private void Initialize(int width, int height)
        {
            Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
            rectTransform.sizeDelta = size;
        }

        /// <summary>
        /// 尝试将物品放置到当前网格的指定坐标，进行边界与占用校验。
        /// </summary>
        /// <param name="item">要放置的物品。</param>
        /// <param name="posX">目标列坐标。</param>
        /// <param name="posY">目标行坐标。</param>
        /// <returns>放置成功返回 true，否则返回 false。</returns>
        public bool TryPlaceTetrisItem(TetrisItem item, int posX, int posY)
        {
            if (BoundryCheck(posX, posY, item.WIDTH, item.HEIGHT) == false) return false;
            if (ValidPosCheck(item, posX, posY) == false) return false;
            PlaceTetrisItem(item, posX, posY);

            return true;
        }

        /// <summary>
        /// 尝试将物品放置到指定坐标，并返回唯一叠加物品用于堆叠；成功后写存档与容器引用。
        /// </summary>
        /// <param name="item">要放置的物品引用（成功后置空）。</param>
        /// <param name="posX">目标列坐标。</param>
        /// <param name="posY">目标行坐标。</param>
        /// <param name="overlapItem">输出的唯一重叠物品（用于堆叠），无则为 null。</param>
        /// <returns>放置成功返回 true，否则返回 false。</returns>
        public bool TryPlaceTetrisItem(ref TetrisItem item, int posX, int posY, ref TetrisItem overlapItem)
        {
            //Determine if the item is out of bounds
            if (BoundryCheck(posX, posY, item.WIDTH, item.HEIGHT) == false) return false;
            //Check for overlapping items in the specified location and range. Multiple overlapping items have exited
            if (OverlapCheck(item, posX, posY, ref overlapItem) == false) return false;
            if (overlapItem != null
                && overlapItem.ItemDetails.itemID == item.ItemDetails.itemID
                && item.ItemDetails.maxStack != 0)
            {
                //Debug.Log($"Stack {item.name} to {overlapItem.name}");
                return PlaceOnOverlapItem(item, overlapItem);
            }
            if (ValidPosCheck(item, posX, posY) == false) return false;
            PlaceTetrisItem(item, posX, posY);
            if (item != null)
            {
                item.SetItemData(item.GetInstanceID());
                item.CurrentInventoryContainer = this;
                item.GetComponent<Image>().raycastTarget = true;
                item = null;
            }
            return true;
        }

        /// <summary>
        /// 当目标位置存在同 ID 且可堆叠物品时，执行堆叠与清理逻辑。
        /// </summary>
        /// <param name="item">被拖拽物品。</param>
        /// <param name="overlapltem">已存在且候选堆叠的物品。</param>
        /// <returns>堆叠成功返回 true，否则返回 false。</returns>
        public bool PlaceOnOverlapItem(TetrisItem item, TetrisItem overlapltem)
        {
            if (Utilities.TetrisItemUtilities.TryStackItems(overlapltem, item))
            {
                item.SetItemData(item.GetInstanceID());
                overlapltem.SetItemData(overlapltem.GetInstanceID());
                item.TryGetItemComponent<StackableComponent>(out StackableComponent stackableComponent);
                if (stackableComponent.CurrentStack <= 0)
                {
                    TetrisItemGrid otherGrid = (TetrisItemGrid)item.CurrentInventoryContainer;
                    otherGrid.RemoveTetrisItem(item,
                    item.onGridPositionX,
                    item.onGridPositionY,
                    item.RotationOffset,
                    item.TetrisPieceShapePos);
                    item.RemoveItemData(item.GetInstanceID());
                    Destroy(item.gameObject);
                    Utilities.TetrisItemUtilities.TriggerPointerEnter(overlapltem.gameObject);
                }
                return true;
            }

            else
                return false;
        }

        /// <summary>
        /// 将物品按形状点集占据网格、设置位置与旋转，并记录持有字典。
        /// </summary>
        /// <param name="item">物品实例。</param>
        /// <param name="posX">列坐标。</param>
        /// <param name="posY">行坐标。</param>
        public void PlaceTetrisItem(TetrisItem item, int posX, int posY)
        {
            item.transform.SetParent(transform, false);
            if (item.WIDTH == item.HEIGHT)
            {
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    item.WIDTH * tileSizeWidth, item.HEIGHT * tileSizeHeight);
            }
            List<Vector2Int> tetrisPieceShapePos = item.TetrisPieceShapePos;

            item.transform.rotation = Quaternion.Euler(0, 0, -RotationHelper.GetRotationAngle(item.Dir));

            //Occupy the corresponding size of the grid according to the size of the item
            foreach (Vector2Int v2i in tetrisPieceShapePos)
            {
                itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y] = item;
            }

            item.onGridPositionX = posX;
            item.onGridPositionY = posY;
            item.transform.localPosition = CalculatePositionOnGrid(item, posX, posY);
            if (!OwnerItemDic.ContainsKey(item.GetInstanceID()))
            {
                OwnerItemDic.Add(item.GetInstanceID(), item);
                //Debug.Log($"Add {item.name} to {name}");
            }
        }

        /// <summary>
        /// 根据旧状态清理网格占用并从持有字典移除该物品，返回被移除物品。
        /// </summary>
        /// <param name="toReturn">要移除的物品。</param>
        /// <param name="x">旧列坐标。</param>
        /// <param name="y">旧行坐标。</param>
        /// <param name="oldRotationOffset">旧旋转偏移。</param>
        /// <param name="tetrisPieceShapePositions">旧形状点集。</param>
        /// <returns>被移除的物品引用；若传入为空则返回 null。</returns>
        public TetrisItem RemoveTetrisItem(TetrisItem toReturn, int x, int y, Vector2Int oldRotationOffset, List<Vector2Int> tetrisPieceShapePositions)
        {
            if (toReturn == null) return null;
            CleanGridReference(x, y, oldRotationOffset, tetrisPieceShapePositions);
            if (OwnerItemDic.ContainsKey(toReturn.GetInstanceID()))
            {
                OwnerItemDic.Remove(toReturn.GetInstanceID());
                //Debug.Log($"Remove {toReturn.name} from {name}");
            }
            return toReturn;
        }

        //Unoccupy the corresponding size of the grid by item size
        private void CleanGridReference(int startColumn, int startRow, Vector2Int oldRotationOffset, List<Vector2Int> tetrisItemGrids)
        {
            //Check starting coordinates and item dimensions for out-of-bounds
            if (startRow < 0 || startRow >= gridSizeHeight ||
                startColumn < 0 || startColumn >= gridSizeWidth)
            {
                Debug.LogError($"Starting coordinates ({startRow}, {startColumn}) invalid");
                return;
            }

            foreach (Vector2Int v2i in tetrisItemGrids)
            {
                itemSlot[startColumn + v2i.x + oldRotationOffset.x, startRow + v2i.y + oldRotationOffset.y] = null;
            }
        }

        /// <summary>
        /// 检查目标范围内的占用情况，若仅存在一个重叠物品则返回该引用；多个重叠则失败。
        /// </summary>
        /// <param name="item">待放置物品。</param>
        /// <param name="posX">列坐标。</param>
        /// <param name="posY">行坐标。</param>
        /// <param name="overlapItem">输出的唯一重叠物品。</param>
        /// <returns>检查成功（零或一个重叠）返回 true；发现多个重叠返回 false。</returns>
        public bool OverlapCheck(TetrisItem item, int posX, int posY, ref TetrisItem overlapItem)
        {
            foreach (Vector2Int v2i in item.TetrisPieceShapePos)
            {
                //If there are items in the current location
                if (itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y] != null)
                {
                    //If the overlapItem has not yet been assigned a value (the first time an overlapping item is found)
                    if (overlapItem == null)
                    {
                        overlapItem = itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y];
                    }
                    else
                    {
                        //If you find multiple overlapping items in the range
                        if (overlapItem != itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y])
                        {
                            overlapItem = null;
                            return false;
                        }
                    }
                }
            }

            //Return true if all the locations being checked have the same overlapping items
            return true;
        }

        /// <summary>
        /// 校验目标范围内是否全部为空位，确保可放置。
        /// </summary>
        /// <param name="item">待放置物品。</param>
        /// <param name="posX">列坐标。</param>
        /// <param name="posY">行坐标。</param>
        /// <returns>可放置返回 true，否则返回 false。</returns>
        private bool ValidPosCheck(TetrisItem item, int posX, int posY)
        {
            foreach (Vector2Int v2i in item.TetrisPieceShapePos)
            {
                if (itemSlot[posX + v2i.x + item.RotationOffset.x, posY + v2i.y + item.RotationOffset.y] != null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///Calculate the coordinates of position in the current grid
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2Int GetTileGridPosition(Vector2 position)
        {
            //Calculates the offset of the mouse position with respect to the RectTransform
            positionOnTheGrid.x = position.x - rectTransform.position.x;
            positionOnTheGrid.y = rectTransform.position.y - position.y;

            //Converts the offset to the grid position
            tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth / canvas.scaleFactor);
            tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight / canvas.scaleFactor);

            return tileGridPosition;
        }

        /// <summary>
        /// Determine if the item is out of bounds
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool BoundryCheck(int posX, int posY, int width, int height)
        {
            if (PositionCheck(posX, posY) == false) return false;//Top left corner
            posX += width - 1;
            posY += height - 1;
            if (PositionCheck(posX, posY) == false) return false;//Lower right corner
            return true;
        }

        //Determine whether the grid coordinates are out of line
        bool PositionCheck(int posX, int posY)
        {
            if (posX < 0 || posY < 0) return false;
            if (posX >= gridSizeWidth || posY >= gridSizeHeight) return false;
            return true;
        }

        public Vector2 CalculateHighlighterPosition(int gridX, int gridY)
        {
            return new Vector2(
                gridX * tileSizeWidth,
                -gridY * tileSizeHeight
            );
        }
        public Vector2 CalculateTilePosition(TetrisItemGhost ghost, int gridX, int gridY)
        {
            return new Vector2(
                gridX * tileSizeWidth + ghost.RotationOffset.x * tileSizeWidth,
                -gridY * tileSizeHeight - ghost.RotationOffset.y * tileSizeHeight
            );
        }

        /// <summary>
        /// The grid coordinates are calculated as the UI pivot position
        /// </summary>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public Vector2 CalculatePositionOnGrid(TetrisItem item, int posX, int posY)
        {
            return new Vector2(
                posX * tileSizeWidth + tileSizeWidth * item.WIDTH / 2,
                -(posY * tileSizeHeight + tileSizeHeight * item.HEIGHT / 2)
            );
        }

        //Get items by grid coordinates
        public TetrisItem GetTetrisItem(int x, int y)
        {
            return itemSlot[x, y];
        }

        public bool HasItem(int x, int y)
        {
            if (itemSlot[x, y] == null) return false;
            return true;
        }

        public PersistentGridIdentification GetPersistentGridIdentification()
        {
            return transform.GetComponent<PersistentGridIdentification>();
        }
    }
}