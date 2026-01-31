using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("资源设置")]
    // 这里拖入 Project 里的 CardPrefab
    public GameObject cardPrefab;

    // 这里拖入 Canvas 下的 4 个座位 (Pos_Player, Pos_Left, Pos_Top, Pos_Right)
    // 务必确保 Size = 4 且已拖入物体
    public Transform[] playerHandSlots;

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 刷新指定玩家的手牌显示
    /// </summary>
    /// <param name="player">玩家数据对象</param>
    public void UpdateHandVisuals(Player player)
    {
        // 安全检查：防止数组越界
        if (playerHandSlots == null || player.id >= playerHandSlots.Length)
        {
            Debug.LogError($"UIManager 错误: 找不到 ID 为 {player.id} 的座位，请检查 Player Hand Slots 数组设置。");
            return;
        }

        // 1. 获取该玩家的座位节点
        Transform seat = playerHandSlots[player.id];

        // 2. 清空该位置现有的所有卡牌 (防止每次更新都叠加)
        foreach (Transform child in seat)
        {
            Destroy(child.gameObject);
        }

        // 3. 遍历玩家手牌数据，生成新卡牌
        foreach (string cardData in player.hand)
        {
            // =======================================================
            // 关键修复代码 START
            // =======================================================

            // 生成卡牌：参数 false 表示"不保留世界坐标"，让它自动适应父节点
            GameObject newCard = Instantiate(cardPrefab, seat, false);

            // 双重保险：强制把坐标和旋转归零，缩放归一
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localRotation = Quaternion.identity;
            newCard.transform.localScale = Vector3.one;

            // =======================================================
            // 关键修复代码 END
            // =======================================================

            // 4. 设置卡牌显示内容 (正面/背面)
            CardUI cardScript = newCard.GetComponent<CardUI>();

            if (cardScript != null)
            {
                // 假设 ID 0 是玩家自己，显示正面
                if (player.id == 0)
                {
                    cardScript.SetCard(cardData); // 调用 CardUI 显示花色文字
                }
                else
                {
                    cardScript.SetBack(); // 其他人显示背面
                }
            }
        }
    }
}