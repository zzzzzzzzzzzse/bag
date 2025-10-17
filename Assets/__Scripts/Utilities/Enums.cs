/// <summary>
/// 俄罗斯方块形状枚举
/// 定义各种俄罗斯方块形状类型
/// </summary>
public enum TetrisPieceShape
{
    Frame,
    //Domino
    Domino,
    //Tromino
    Tromino_I, Tromino_L, Tromino_J,
    //Tetromino
    Tetromino_I, Tetromino_O, Tetromino_T, Tetromino_J, Tetromino_L, Tetromino_S, Tetromino_Z,
    //Pentomino
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
/// 定义各种装备槽位和容器类型
/// </summary>
public enum InventorySlotType
{
    Pocket, Coat, Chesthanging, BackPack, WaistHanging, Coffer, Depository,
    LongWeapon, ShortWeapon, LargeConsume, MiddleConsume, SmallConsume
}

/// <summary>
/// 持久化网格类型枚举
/// 定义需要持久化保存的网格类型
/// </summary>
public enum PersistentGridType
{
    None,
    Pocket, Coffer, Depository
}

/// <summary>
/// 放置状态枚举
/// 定义物品放置时的各种状态
/// </summary>
public enum PlaceState
{
    OnGridHasItem, OnGridNoItem,
    OnSlotHasItem, OnSlotNoItem,
    InvalidPos
}