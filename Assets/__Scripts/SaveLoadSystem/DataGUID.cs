using UnityEngine;

/// <summary>
/// 数据GUID组件
/// 为游戏对象提供唯一标识符，用于存档系统
/// </summary>
[ExecuteAlways]
public class DataGUID : MonoBehaviour
{
    public string guid;

    /// <summary>
    /// 如果GUID为空，则生成新的唯一标识符
    /// </summary>
    private void Awake()
    {
        if (guid == string.Empty)
        {
            guid = System.Guid.NewGuid().ToString();
        }
    }
}