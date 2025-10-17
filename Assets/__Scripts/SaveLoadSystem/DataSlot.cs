using System.Collections.Generic;

namespace ChosTIS.SaveLoadSystem
{
    public class DataSlot
    {
        // String is GUID
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region UI��ʾ
        public string SaveTime => 
            dataDict.TryGetValue(SaveSlotPanel.Instance.GUID, out var data) 
                ? data.gameSaveTimeDict.GetValueOrDefault("SaveTime") 
                : string.Empty;
        #endregion
    }
}