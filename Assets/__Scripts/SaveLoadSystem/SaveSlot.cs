using UnityEngine;
using UnityEngine.UI;

namespace ChosTIS.SaveLoadSystem
{
    /// <summary>
    /// 存档槽位UI组件
    /// 管理单个存档槽位的显示和操作，包括加载、保存、删除功能
    /// </summary>
    public class SaveSlot : MonoBehaviour
    {
        public Text saveTime;
        [SerializeField] private Button delletBtn, saveBtn, startBtn;
        private DataSlot currentData;
        private int Index => transform.GetSiblingIndex();

        /// <summary>
        /// 初始化按钮事件监听
        /// </summary>
        private void Awake()
        {
            startBtn.onClick.AddListener(LoadGameData);
            saveBtn.onClick.AddListener(SaveGameData);
            delletBtn.onClick.AddListener(DeleteGameData);
        }

        /// <summary>
        /// 设置槽位UI显示
        /// </summary>
        private void Start()
        {
            SetupSlotUI();

        }

        /// <summary>
        /// 根据存档数据设置槽位UI状态
        /// </summary>
        private void SetupSlotUI()
        {
            currentData = SaveLoadManager.Instance.dataSlots[Index];

            if (currentData != null)
            {
                saveTime.text = "Save Time��" + currentData.SaveTime;
                delletBtn.gameObject.SetActive(true);
            }
            else
            {
                saveTime.text = "NUll";
                delletBtn.gameObject.SetActive(false);
                saveBtn.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 加载游戏数据
        /// </summary>
        private void LoadGameData()
        {
            if (currentData != null)
            {
                Debug.Log("[LoadGame]" + Index);
                ChosTIS.Utility.EventHandler.CallStartGameEvent(Index);
                ChosTIS.Utility.EventHandler.CallInstantiateInventoryItemUI();
            }
        }

        /// <summary>
        /// 保存游戏数据
        /// </summary>
        private void SaveGameData()
        {
            Debug.Log("[SaveGame]" + Index);
            ChosTIS.Utility.EventHandler.CallSaveGameEvent(Index);
            SetupSlotUI();
        }

        /// <summary>
        /// 删除游戏数据
        /// </summary>
        private void DeleteGameData()
        {
            if (currentData != null)
            {
                Debug.Log("[DeleteGame]" + Index);
                ChosTIS.Utility.EventHandler.CallDeleteDataEvent(Index);
                ChosTIS.Utility.EventHandler.CallDeleteObjectEvent();
                SetupSlotUI();
            }

            Utilities.TetrisItemUtilities.TriggerPointerEnter(InventoryManager.Instance.depositoryGrid.gameObject);
        }

    }
}