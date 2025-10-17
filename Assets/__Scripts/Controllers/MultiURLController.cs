using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 多URL控制器
/// 管理多个按钮与URL的对应关系，支持点击按钮打开指定网页
/// </summary>
public class MultiURLController : MonoBehaviour
{
    /// <summary>
    /// 按钮URL配对数据结构
    /// </summary>
    [System.Serializable]
    public class ButtonURLPair
    {
        public Button button;
        public string targetURL;
    }

    public ButtonURLPair[] buttonLinks = new ButtonURLPair[4];

    /// <summary>
    /// 初始化按钮事件监听
    /// </summary>
    void Start()
    {
        if (buttonLinks.Length != 4)
        {
            Debug.LogError("Need to configure 4 Button-URL correspondences��");
            return;
        }

        foreach (var pair in buttonLinks)
        {
            if (pair.button != null)
            {
                pair.button.onClick.AddListener(() => OpenURL(pair.targetURL));
            }
            else
            {
                Debug.LogWarning("Exist unconfigured button references��");
            }
        }
    }

    /// <summary>
    /// 打开指定URL
    /// </summary>
    /// <param name="url">目标URL</param>
    void OpenURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
            Debug.Log($"Openning��{url}");
        }
        else
        {
            Debug.LogWarning("URL is null, please check the configuration��");
        }
    }
}