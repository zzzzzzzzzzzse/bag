namespace ChosTIS.SaveLoadSystem
{
    /// <summary>
    /// 可保存对象接口
    /// 定义可被存档系统管理的对象的基本契约
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// 对象的唯一标识符
        /// </summary>
        string GUID { get; }

        /// <summary>
        /// 注册为可保存对象
        /// </summary>
        void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);
        }

        /// <summary>
        /// 生成保存数据
        /// </summary>
        /// <returns>游戏存档数据</returns>
        GameSaveData GenerateSaveData();
        
        /// <summary>
        /// 恢复数据
        /// </summary>
        /// <param name="saveData">要恢复的存档数据</param>
        void RestoreData(GameSaveData saveData);
    }
}
