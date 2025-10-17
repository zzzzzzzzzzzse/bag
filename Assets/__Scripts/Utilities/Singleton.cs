using UnityEngine;

/// <summary>
/// 单例模式基类
/// 确保全局只有一个实例，自动处理实例创建和销毁
/// </summary>
/// <typeparam name="T">继承自Singleton的类型</typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance
    {
        get => instance;
    }

    /// <summary>
    /// 单例初始化，确保只有一个实例存在
    /// </summary>
    protected virtual void Awake()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    /// <summary>
    /// 清理单例实例
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

}
