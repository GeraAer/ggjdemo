using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameLoopController : MonoBehaviour
{
    // --- UI 引用插槽 (在 Inspector 面板中拖入对应的 Hierarchy 物体) ---
    [Header("UI 引用")]
    public Transform handParent;       // 拖入 HandPanel
    public Button challengeButton;     // 拖入 ChallengeButton
    public TextMeshProUGUI themeText;  // 拖入显示主题的文字物体

    // --- 游戏逻辑引用 ---
    private GameManager gm;
    private TrophyManager trophyManager;

    [Header("游戏状态")]
    public string currentTheme = "Animal";
    private List<string> cardsOnTable = new List<string>();
    private Player lastPlayer;
    private Player challenger;

    private void Start()
    {
        gm = GameManager.Instance;
        trophyManager = new TrophyManager();

        // 确保初始 UI 显示正确
        UpdateThemeUI();

        gm.StartGame();
        StartCoroutine(MainGameLoop());
    }

    // --- 公开方法：供 Unity 界面调用 ---

    // 每次更新主题时同步到屏幕上
    public void UpdateThemeUI()
    {
        if (themeText != null)
        {
            themeText.text = "当前主题: " + currentTheme;
        }
    }

    // 处理质疑逻辑 (按钮点击时触发)
    public void ExecuteChallengeLogic()
    {
        // 如果当前没有人在出牌，则不执行逻辑
        if (cardsOnTable.Count == 0) return;

        bool isLie = false;
        foreach (string card in cardsOnTable)
        {
            // 判断逻辑：如果不包含主题且不是万能牌(Wild)，就是撒谎
            if (!card.Contains(currentTheme) && card != "Wild")
            {
                isLie = true;
                break;
            }
        }

        if (isLie)
        {
            Debug.Log("质疑成功！");
            challenger.AddMask();
            lastPlayer.RemoveMask();
            HandleCheckHandStatus(true);
        }
        else
        {
            Debug.Log("质疑失败！");
            challenger.RemoveMask();
            lastPlayer.AddMask();
            HandleCheckHandStatus(false);
        }

        // 质疑结束后清空桌面
        cardsOnTable.Clear();
    }

    // --- 内部游戏循环协程 ---

    IEnumerator MainGameLoop()
    {
        while (true)
        {
            // 阶段 1 & 2: 摸牌与主题
            UpdateThemeUI();
            Debug.Log($"当前选择的主题: {currentTheme}");

            // 阶段 3 & 4: 判定玩家
            lastPlayer = gm.GetCurrentPlayer();
            Debug.Log($"轮到玩家: {lastPlayer.name} 出牌");

            // 阶段 5: 等待出牌
            yield return StartCoroutine(WaitForPlayerPlayCards());

            // 阶段 6: 等待质疑 (这里保留自动模拟质疑，你也可以手动点按钮)
            yield return StartCoroutine(WaitForChallenge());

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator WaitForPlayerPlayCards()
    {
        Debug.Log("等待玩家选牌...");
        cardsOnTable.Clear();
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            if (lastPlayer.GetHandCount() > 0)
            {
                string c = lastPlayer.hand[0];
                lastPlayer.RemoveCard(c);
                cardsOnTable.Add(c);
            }
        }
        yield return new WaitForSeconds(1f);
    }

    IEnumerator WaitForChallenge()
    {
        Debug.Log("等待质疑 (3秒倒计时，你也可以点击 UI 按钮)...");
        bool isChallenged = false;
        float timer = 3f;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // 这里的自动随机模拟保留，方便你测试
            if (Random.value > 0.95f) { isChallenged = true; break; }
            yield return null;
        }

        if (!isChallenged)
        {
            Debug.Log("无人质疑，顺利通过");
            HandleCheckHandStatus(false);
        }
        else
        {
            // 模拟第二个玩家发起质疑
            challenger = gm.GetAllPlayers()[1];
            ExecuteChallengeLogic();
        }
    }

    void HandleCheckHandStatus(bool wasChallengedSuccess)
    {
        if (lastPlayer.GetHandCount() == 0)
        {
            int count = cardsOnTable.Count;
            for (int i = 0; i < count; i++)
            {
                lastPlayer.AddCard(gm.GetDeck().DrawCard());
            }
            Debug.Log("没牌了，重新补牌");
        }
        else
        {
            if (!wasChallengedSuccess)
            {
                AddCardsToTrophyPile();
            }
            gm.NextPlayer();
        }
    }

    void AddCardsToTrophyPile()
    {
        if (lastPlayer == null || cardsOnTable.Count == 0) return;
        TrophyType trophyType = GetTrophyTypeByTheme(currentTheme);
        foreach (string card in cardsOnTable)
        {
            trophyManager.AddTrophy(lastPlayer.id, trophyType);
        }
        Debug.Log($"玩家 {lastPlayer.name} 获得了战利品: {trophyType}");
    }

    TrophyType GetTrophyTypeByTheme(string theme)
    {
        switch (theme)
        {
            case "Animal": return TrophyType.Type1;
            case "Plant": return TrophyType.Type2;
            case "Food": return TrophyType.Type3;
            case "Color": return TrophyType.Type4;
            default: return TrophyType.Type5;
        }
    }
}