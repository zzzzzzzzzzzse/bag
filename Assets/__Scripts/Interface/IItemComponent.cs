
namespace ChosTIS
{
    /// <summary>
    /// 物品组件接口
    /// 定义物品组件的生命周期方法
    /// </summary>
    public interface IItemComponent
    {
        /// <summary>
        /// 组件附加到物品时调用
        /// </summary>
        /// <param name="item">目标物品</param>
        void OnAttach(TetrisItem item);
        
        /// <summary>
        /// 组件从物品分离时调用
        /// </summary>
        void OnDetach();
    }

    /// <summary>
    /// 信息显示接口标记
    /// </summary>
    public interface IInformation { }
    
    /// <summary>
    /// 网格面板接口标记
    /// </summary>
    public interface IGridPanel { }
    
    /// <summary>
    /// 可堆叠接口标记
    /// </summary>
    public interface IStackable { }
    
    /// <summary>
    /// 可附加接口标记
    /// </summary>
    public interface IAttachable { }
}