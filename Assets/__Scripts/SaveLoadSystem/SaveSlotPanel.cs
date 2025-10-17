using System;
using System.Collections.Generic;

namespace ChosTIS.SaveLoadSystem
{
    /// <summary>
    /// 存档槽位面板管理器
    /// 负责管理存档时间信息的保存和恢复
    /// </summary>
    public class SaveSlotPanel : Singleton<SaveSlotPanel>, ISaveable
    {
        public string GUID => GetComponent<DataGUID>().guid;

        /// <summary>
        /// 注册为可保存对象
        /// </summary>
        private void Start()
        {
            ISaveable saveable = this;
            saveable?.RegisterSaveable();
        }

        /// <summary>
        /// 获取当前保存时间
        /// </summary>
        private string SaveTime
        {
            get
            {
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;
                int second = DateTime.Now.Second;
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;
                return string.Format("{0:D4}/{1:D2}/{2:D2}" + " " + "{3:D2}:{4:D2}:{5:D2} ", year, month, day, hour, minute, second);
            }
        }

        /// <summary>
        /// 生成保存数据
        /// </summary>
        /// <returns>包含保存时间的游戏存档数据</returns>
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new()
            {
                gameSaveTimeDict = new Dictionary<string, string>
                {
                    { "SaveTime", SaveTime }
                }
            };

            return saveData;
        }

        /// <summary>
        /// 恢复数据
        /// </summary>
        /// <param name="saveData">要恢复的存档数据</param>
        public void RestoreData(GameSaveData saveData)
        {

        }
    }
}