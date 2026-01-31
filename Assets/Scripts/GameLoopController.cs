using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLoopController : MonoBehaviour
{
    // 引用已有组件
    private GameManager gm;

    // 当前回合状态数据
    public string currentTheme = "Animal"; // 示例主题
    private List<string> cardsOnTable = new List<string>(); // 当前打出的牌
    private Player lastPlayer; // 当前出牌的人
    private Player challenger; // 质疑的人

    private void Start()
    {
        gm = GameManager.Instance;
        // 1. 游戏初始化 (使用队友的 GameManager)
        gm.StartGame();
        // 2. 开启核心循环协程
        StartCoroutine(MainGameLoop());
    }

    IEnumerator MainGameLoop()
    {
        while (true) // 核心玩法循环
        {
            // --- 步骤 1: 摸牌 ---
            Debug.Log("进入摸牌阶段...");
            // TODO: 这里可以添加摸牌逻辑

            // --- 步骤 2: 主题选择 ---
            // 这里可以弹出 UI 让玩家选，现在暂定为自动
            Debug.Log($"当前选择的主题是: {currentTheme}");

            // --- 步骤 3 & 4: 判定出牌者 & 激活战利品 ---
            lastPlayer = gm.GetCurrentPlayer();
            Debug.Log($"轮到玩家: {lastPlayer.name} 出牌");

            // --- 步骤 5: 出牌 (1~3张) ---
            yield return StartCoroutine(WaitForPlayerPlayCards());

            // --- 步骤 6: 质疑 ---
            yield return StartCoroutine(WaitForChallenge());

            // 流程图中的判定逻辑已经在协程中处理完毕
            yield return new WaitForSeconds(1f);
        }
    }

    // 等待玩家点击 UI 出牌
    IEnumerator WaitForPlayerPlayCards()
    {
        Debug.Log("等待出牌者选择卡牌...");
        // 这里应该等待 UI 事件。模拟逻辑：随机出1-2张牌
        cardsOnTable.Clear();
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            if (lastPlayer.GetHandCount() > 0)
            {
                string c = lastPlayer.hand[0];
                lastPlayer.RemoveCard(c); // 使用队友的 RemoveCard
                cardsOnTable.Add(c);
            }
        }
        yield return new WaitForSeconds(1f);
    }

    // 等待其他玩家点击质疑
    IEnumerator WaitForChallenge()
    {
        Debug.Log("等待质疑 (3秒内无人质疑则跳过)...");
        bool isChallenged = false;
        float timer = 3f;

        // 这里在实际开发中会监听按钮点击
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // 模拟随机发生质疑
            if (Random.value > 0.8f) { isChallenged = true; break; }
            yield return null;
        }

        if (!isChallenged)
        {
            // --- 分支：无人质疑 ---
            Debug.Log("无人质疑，牌打出");
            HandleCheckHandStatus(false);
        }
        else
        {
            // 假设第2个玩家发起了质疑
            challenger = gm.GetAllPlayers()[1];
            ExecuteChallengeLogic();
        }
    }

    void ExecuteChallengeLogic()
    {
        bool isLie = false;
        foreach (string card in cardsOnTable)
        {
            // 判定逻辑：不含主题且不是 Wild 就算说谎
            if (!card.Contains(currentTheme) && card != "Wild")
            {
                isLie = true;
                break;
            }
        }

        if (isLie)
        {
            // --- 质疑成功 ---
            Debug.Log("质疑成功！");
            challenger.AddMask(); // 队友写的 AddMask
            lastPlayer.RemoveMask(); // 队友写的 RemoveMask

            // 判定是否没牌 (对应流程图)
            HandleCheckHandStatus(true);
        }
        else
        {
            // --- 质疑失败 ---
            Debug.Log("质疑失败！");
            challenger.RemoveMask();
            lastPlayer.AddMask();

            // 牌打出逻辑
            HandleCheckHandStatus(false);
        }
    }

    void HandleCheckHandStatus(bool wasChallengedSuccess)
    {
        // 流程图节点：是否没牌
        if (lastPlayer.GetHandCount() == 0)
        {
            // Yes 分支：从牌库抽取相同数量的牌
            int count = cardsOnTable.Count;
            for (int i = 0; i < count; i++)
            {
                lastPlayer.AddCard(gm.GetDeck().DrawCard());
            }
            Debug.Log("没牌了，重新补牌，回到摸牌阶段");
            // 流程图逻辑线连回“摸牌”
        }
        else
        {
            // No 分支：进入下一位出牌者
            if (!wasChallengedSuccess)
            {
                // 牌成功打出，归入战利品库 (这里可以写你的战利品逻辑)
                Debug.Log("牌归入战利品库");
            }
            gm.NextPlayer();
        }
    }
}