using System;

namespace ChosTIS.Utility
{
    /// <summary>
    /// 事件处理器
    /// 提供全局事件总线，用于系统间解耦通信
    /// </summary>
    public static class EventHandler
    {
        /// <summary>
        /// 开始新游戏事件
        /// </summary>
        public static event Action<int> StartNewGameEvent;
        /// <summary>
        /// 触发开始新游戏事件
        /// </summary>
        /// <param name="index">存档槽位索引</param>
        public static void CallStartNewGameEvent(int index)
        {
            StartNewGameEvent?.Invoke(index);
        }

        /// <summary>
        /// 继续游戏事件
        /// </summary>
        public static event Action<int> ContinueGameEvent;
        /// <summary>
        /// 触发继续游戏事件
        /// </summary>
        /// <param name="index">存档槽位索引</param>
        public static void CallContinueGameEvent(int index)
        {
            ContinueGameEvent?.Invoke(index);
        }

        /// <summary>
        /// 开始游戏事件
        /// </summary>
        public static event Action BegineGameEvent;
        /// <summary>
        /// 触发开始游戏事件
        /// </summary>
        public static void CallBegineGameEvent()
        {
            BegineGameEvent?.Invoke();
        }

        /// <summary>
        /// 启动游戏事件
        /// </summary>
        public static event Action<int> StartGameEvent;
        /// <summary>
        /// 触发启动游戏事件
        /// </summary>
        /// <param name="index">存档槽位索引</param>
        public static void CallStartGameEvent(int index)
        {
            StartGameEvent?.Invoke(index);
        }

        /// <summary>
        /// 保存游戏事件
        /// </summary>
        public static event Action<int> SaveGameEvent;
        /// <summary>
        /// 触发保存游戏事件
        /// </summary>
        /// <param name="index">存档槽位索引</param>
        public static void CallSaveGameEvent(int index)
        {
            SaveGameEvent?.Invoke(index);
        }

        /// <summary>
        /// 删除数据事件
        /// </summary>
        public static event Action<int> DeleteDataEvent;
        /// <summary>
        /// 触发删除数据事件
        /// </summary>
        /// <param name="index">存档槽位索引</param>
        public static void CallDeleteDataEvent(int index)
        {
            DeleteDataEvent?.Invoke(index);
        }

        /// <summary>
        /// 删除对象事件
        /// </summary>
        public static event Action DeleteObjectEvent;
        /// <summary>
        /// 触发删除对象事件
        /// </summary>
        public static void CallDeleteObjectEvent()
        {
            DeleteObjectEvent?.Invoke();
        }

        /// <summary>
        /// 实例化库存物品UI事件
        /// </summary>
        public static event Action InstantiateInventoryItemUI;
        /// <summary>
        /// 触发实例化库存物品UI事件
        /// </summary>
        public static void CallInstantiateInventoryItemUI()
        {
            InstantiateInventoryItemUI?.Invoke();
        }

    }
}