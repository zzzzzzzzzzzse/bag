namespace ChosTIS
{
    /// <summary>
    /// 库存功能容器接口
    /// 扩展基础库存容器接口，添加功能性容器特有的方法
    /// </summary>
    public interface IInventoryFunctionalContainer : IInventoryContainer
    {
        /// <summary>
        /// 尝试放置物品到功能容器
        /// </summary>
        /// <param name="tetrisItem">要放置的物品</param>
        /// <returns>是否成功放置</returns>
        public bool TryPlaceTetrisItem(TetrisItem tetrisItem);
        
        /// <summary>
        /// 放置物品到功能容器
        /// </summary>
        /// <param name="tetrisItem">要放置的物品</param>
        public void PlaceTetrisItem(TetrisItem tetrisItem);
        
        /// <summary>
        /// 从功能容器移除物品
        /// </summary>
        public void RemoveTetrisItem();

    }
}