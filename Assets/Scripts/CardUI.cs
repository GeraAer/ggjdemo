using UnityEngine;
using UnityEngine.UI;
using TMPro; // 引用 TextMeshPro

public class CardUI : MonoBehaviour
{
    [Header("组件设置")]
    // 如果你在Inspector里不拖进去，代码会在Awake里自动找
    public Image cardBgImage;
    public TextMeshProUGUI nameText;

    private void Awake()
    {
        // 1. 自动查找自身的 Image 组件 (卡牌背景)
        if (cardBgImage == null)
            cardBgImage = GetComponent<Image>();

        // 2. 自动查找子物体的 TextMeshPro 组件 (卡牌文字)
        if (nameText == null)
            nameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// 设置为正面：显示米黄色背景 + 黑色文字
    /// </summary>
    public void SetCard(string cardName)
    {
        // --- 设置背景 ---
        if (cardBgImage != null)
        {
            // 关键修复：最后这个 1.0f 代表 Alpha=1 (完全不透明)
            // 使用米黄色 (0.9, 0.9, 0.8) 模拟纸张，比纯白更清楚
            cardBgImage.color = new Color(0.9f, 0.9f, 0.8f, 1.0f);

            // 确保组件是开启的
            cardBgImage.enabled = true;
        }

        // --- 设置文字 ---
        if (nameText != null)
        {
            nameText.text = cardName;
            // 关键修复：强制文字为黑色，防止白底白字
            nameText.color = Color.black;
            nameText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 设置为背面：显示深红色背景 + 隐藏文字
    /// </summary>
    public void SetBack()
    {
        // --- 设置背景 ---
        if (cardBgImage != null)
        {
            // 深红色背面
            cardBgImage.color = new Color(0.8f, 0.2f, 0.2f, 1.0f);
            cardBgImage.enabled = true;
        }

        // --- 设置文字 ---
        if (nameText != null)
        {
            // 背面不显示文字
            nameText.gameObject.SetActive(false);
        }
    }
}