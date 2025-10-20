/// <summary>
/// 俄罗斯方块形状枚举
/// 定义各种俄罗斯方块形状类型
/// </summary>
public enum TetrisPieceShape
{
    //Frame (一格拼图)
    Frame,
    //Domino (二格拼图)
    Domino,
    //Tromino (三格拼图)
    Tromino_I, Tromino_L, Tromino_J,
    //Tetromino(四格拼图)
    Tetromino_I, Tetromino_O, Tetromino_T, Tetromino_J, Tetromino_L, Tetromino_S, Tetromino_Z,
    //Pentomino(五格拼图)
    Pentomino_I, Pentomino_L, Pentomino_J, Pentomino_U, Pentomino_T, Pentomino_P
}

/// <summary>
/// 方向枚举
/// 定义物品的旋转方向
/// </summary>
public enum Dir
{
    Down,
    Left,
    Up,
    Right,
}

/// <summary>
/// 库存槽位类型枚举
/// 定义各种装备槽位和容器类型，用于物品分类和放置限制
/// </summary>
public enum InventorySlotType
{
    /// <summary>
    /// 口袋 - 基础存储槽位，可放置大部分物品
    /// </summary>
    Pocket,
    
    /// <summary>
    /// 外套 - 服装类物品专用槽位
    /// </summary>
    Coat,
    
    /// <summary>
    /// 胸前挂件 - 小型装饰品或工具槽位
    /// </summary>
    Chesthanging,
    
    /// <summary>
    /// 背包 - 大型存储容器，可放置多种物品
    /// </summary>
    BackPack,
    
    /// <summary>
    /// 腰挂 - 腰部装备槽位，通常用于工具或小型武器
    /// </summary>
    WaistHanging,
    
    /// <summary>
    /// 保险箱 - 贵重物品存储槽位，安全性较高
    /// </summary>
    Coffer,
    
    /// <summary>
    /// 仓库 - 大型存储设施，用于长期存储
    /// </summary>
    Depository,
    
    /// <summary>
    /// 长武器 - 长柄武器专用槽位
    /// </summary>
    LongWeapon,
    
    /// <summary>
    /// 短武器 - 短柄武器专用槽位（如剑、匕首等）
    /// </summary>
    ShortWeapon,
    
    /// <summary>
    /// 大型消耗品 - 大型药水、食物等消耗品槽位
    /// </summary>
    LargeConsume,
    
    /// <summary>
    /// 中型消耗品 - 中型药水、食物等消耗品槽位
    /// </summary>
    MiddleConsume,
    
    /// <summary>
    /// 小型消耗品 - 小型药水、食物等消耗品槽位
    /// </summary>
    SmallConsume
}

/// <summary>
/// 持久化网格类型枚举
/// 定义需要持久化保存的网格类型，用于存档系统中识别不同类型的存储容器
/// </summary>
public enum PersistentGridType
{
    /// <summary>
    /// 无类型 - 不进行持久化保存的网格
    /// </summary>
    None,
    
    /// <summary>
    /// 口袋网格 - 玩家基础存储网格，需要持久化保存
    /// </summary>
    Pocket,
    
    /// <summary>
    /// 保险箱网格 - 贵重物品存储网格，需要持久化保存
    /// </summary>
    Coffer,
    
    /// <summary>
    /// 仓库网格 - 大型存储设施网格，需要持久化保存
    /// </summary>
    Depository
}

/// <summary>
/// 放置状态枚举
/// 定义物品放置时的各种状态，用于拖拽和放置逻辑的状态判断
/// </summary>
public enum PlaceState
{
    /// <summary>
    /// 网格有物品 - 目标网格位置已有物品，可能发生堆叠或替换
    /// </summary>
    OnGridHasItem,
    
    /// <summary>
    /// 网格无物品 - 目标网格位置为空，可以正常放置物品
    /// </summary>
    OnGridNoItem,
    
    /// <summary>
    /// 槽位有物品 - 目标装备槽位已有物品，需要先移除现有物品
    /// </summary>
    OnSlotHasItem,
    
    /// <summary>
    /// 槽位无物品 - 目标装备槽位为空，可以正常装备物品
    /// </summary>
    OnSlotNoItem,
    
    /// <summary>
    /// 无效位置 - 目标位置不可放置物品（超出边界、类型不匹配等）
    /// </summary>
    InvalidPos
}