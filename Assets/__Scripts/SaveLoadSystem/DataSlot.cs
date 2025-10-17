using System.Collections.Generic;

namespace ChosTIS.SaveLoadSystem
{
    /// <summary>
    /// 数据槽位类
    /// 存储单个存档槽位的所有数据，以GUID为键管理多个存档数据
    /// </summary>
    public class DataSlot
    {
        /// <summary>
        /// 存档数据字典，键为GUID，值为游戏存档数据
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region UI��ʾ
        /// <summary>
        /// 获取保存时间用于UI显示
        /// </summary>
        public string SaveTime => 
            dataDict.TryGetValue(SaveSlotPanel.Instance.GUID, out var data) 
                ? data.gameSaveTimeDict.GetValueOrDefault("SaveTime") 
                : string.Empty;
        #endregion
    }
}