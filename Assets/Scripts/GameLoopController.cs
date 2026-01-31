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
    private Player lastPlayer; // 当前出牌的人 (引用你的纯数据类 Player)
    private Player challenger; // 质疑的人

    private void Start()
    {
        gm = GameManager.Instance;

        // 1. 游戏初始化 
        // 确保 GameManager 内部已经实例化了 new Deck() 和创建了 List<Player>
        gm.StartGame();

        // 2. 开启核心循环协程
        StartCoroutine(MainGameLoop());
    }

    IEnumerator MainGameLoop()
    {
        while (true) // 核心玩法循环
        {
            // =================================================
            // --- 步骤 1: 摸牌阶段 (4人各摸4张) ---
            // =================================================
            Debug.Log(">>> 进入摸牌阶段...");

            // 1. 获取牌库和玩家列表
            Deck gameDeck = gm.GetDeck();
            List<Player> allPlayers = gm.GetAllPlayers();

            // 2. 遍历每个玩家进行发牌
            if (gameDeck != null && allPlayers != null)
            {
                foreach (Player p in allPlayers)
                {
                    // 每个人摸 4 张
                    for (int i = 0; i < 5; i++)
                    {
                        string drawnCard = gameDeck.DrawCard();

                        // 如果牌库有牌，则添加
                        if (!string.IsNullOrEmpty(drawnCard))
                        {
                            p.AddCard(drawnCard);
                        }
                    }
                    Debug.Log($"玩家 {p.name} (ID:{p.id}) 完成摸牌，当前手牌数: {p.GetHandCount()}");

                    // TODO: 如果需要视觉表现（生成卡牌Prefab），请在此处通知 UI Manager
                    // 例如: UIManager.Instance.UpdatePlayerHandVisuals(p);
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.UpdateHandVisuals(p);
                    }
                }
            }
            else
            {
                Debug.LogError("严重错误: 牌库或玩家列表为空！");
            }

            // 模拟发牌动画等待时间
            yield return new WaitForSeconds(1f);


            // =================================================
            // --- 步骤 2: 主题选择 ---
            // =================================================
            // 这里可以弹出 UI 让玩家选，现在暂定为自动
            Debug.Log($"当前回合主题: {currentTheme}");


            // =================================================
            // --- 步骤 3 & 4: 判定出牌者 & 激活战利品 ---
            // =================================================
            lastPlayer = gm.GetCurrentPlayer();
            Debug.Log($"轮到玩家: {lastPlayer.name} 出牌");


            // =================================================
            // --- 步骤 5: 出牌 (1~3张) ---
            // =================================================
            yield return StartCoroutine(WaitForPlayerPlayCards());


            // =================================================
            // --- 步骤 6: 质疑 ---
            // =================================================
            yield return StartCoroutine(WaitForChallenge());

            // 流程图中的判定逻辑已经在协程中处理完毕
            yield return new WaitForSeconds(1f);
        }
    }

    // 等待玩家点击 UI 出牌
    IEnumerator WaitForPlayerPlayCards()
    {
        Debug.Log("等待出牌者选择卡牌...");

        cardsOnTable.Clear();

        // --- 模拟逻辑：随机出 1-3 张牌 ---
        // 实际开发中，这里应该是一个 while(!playerHasPlayed) yield return null;
        int count = Random.Range(1, 4); // 随机 1 到 3

        // 确保不要出超过手牌数的牌
        int actualPlayCount = Mathf.Min(count, lastPlayer.GetHandCount());

        for (int i = 0; i < actualPlayCount; i++)
        {
            if (lastPlayer.GetHandCount() > 0)
            {
                // 简单模拟：总是出第一张牌
                string c = lastPlayer.hand[0];

                // 从玩家数据移除
                lastPlayer.RemoveCard(c);

                // 加到桌面上
                cardsOnTable.Add(c);
            }
        }

        Debug.Log($"玩家打出了 {cardsOnTable.Count} 张牌");
        yield return new WaitForSeconds(1f);
    }

    // 等待其他玩家点击质疑
    IEnumerator WaitForChallenge()
    {
        Debug.Log("等待质疑 (3秒内)...");
        bool isChallenged = false;
        float timer = 3f;

        // 模拟倒计时
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // 模拟：20% 概率有人质疑
            if (Random.value > 0.8f)
            {
                isChallenged = true;
                break;
            }
            yield return null;
        }

        if (!isChallenged)
        {
            // --- 分支：无人质疑 ---
            Debug.Log("无人质疑，流程继续");
            HandleCheckHandStatus(false);
        }
        else
        {
            // 假设是列表中的第2个玩家发起的质疑（模拟）
            List<Player> players = gm.GetAllPlayers();
            if (players.Count > 1)
                challenger = players[1];
            else
                challenger = lastPlayer; // 防止出错的兜底

            ExecuteChallengeLogic();
        }
    }

    void ExecuteChallengeLogic()
    {
        bool isLie = false;

        // 遍历桌上的牌检查是否撒谎
        foreach (string card in cardsOnTable)
        {
            // 判定逻辑：
            // 1. 如果是 Wild (万能牌)，不算撒谎
            // 2. 如果牌名包含当前主题 (例如 "Tiger_Animal" 包含 "Animal")，不算撒谎
            // 3. 否则就是撒谎

            if (card == "Wild") continue;

            if (!card.Contains(currentTheme))
            {
                isLie = true;
                break;
            }
        }

        if (isLie)
        {
            // --- 质疑成功 (抓到说谎) ---
            Debug.Log($"质疑成功！牌里有假！({currentTheme})");
            challenger.AddMask();
            lastPlayer.RemoveMask();

            // 质疑成功，牌通常归还或者弃掉，这里根据你的流程图进入判空逻辑
            HandleCheckHandStatus(true);
        }
        else
        {
            // --- 质疑失败 (真的是实话) ---
            Debug.Log("质疑失败！牌是真的！");
            challenger.RemoveMask();
            lastPlayer.AddMask();

            HandleCheckHandStatus(false);
        }
    }

    void HandleCheckHandStatus(bool wasChallengedSuccess)
    {
        // 流程图节点：是否没牌
        if (lastPlayer.GetHandCount() == 0)
        {
            // --- Yes 分支：没牌了，补牌 ---
            // 规则：从牌库抽取与打出数量相同的牌（或者补满，看具体规则，这里按流程图补相同数量）
            // 这里的逻辑是：玩家刚才打出了 cardsOnTable.Count 张牌，现在要补回来

            Deck deck = gm.GetDeck();
            int countToDraw = cardsOnTable.Count;

            Debug.Log("玩家手牌为空，触发补牌逻辑...");

            for (int i = 0; i < countToDraw; i++)
            {
                string newCard = deck.DrawCard();
                if (newCard != null)
                    lastPlayer.AddCard(newCard);
            }

            Debug.Log("补牌完毕，回到摸牌阶段");
            // 流程图逻辑线连回“摸牌” -> 主循环 while(true) 会自动回到 Step 1
        }
        else
        {
            // --- No 分支：还有牌 ---
            if (!wasChallengedSuccess)
            {
                // 牌成功打出，归入战利品库 (这里可以写你的战利品逻辑)
                Debug.Log("牌成功打出，进入战利品堆");
            }

            // 切换到下一位玩家
            gm.NextPlayer();
        }
    }
}