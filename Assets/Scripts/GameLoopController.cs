using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLoopController : MonoBehaviour
{
    // 游戏管理器
    private GameManager gm;
    private TrophyManager trophyManager;

    // 当前回合状态变量
    public string currentTheme = "Animal"; // 示例主题
    private List<string> cardsOnTable = new List<string>(); // 当前打出的牌
    private Player lastPlayer; // 当前出牌的玩家
    private Player challenger; // 质疑的玩家

    private void Start()
    {
        gm = GameManager.Instance;
        trophyManager = new TrophyManager();
        // 1. 游戏开始 (使用已有的 GameManager)
        gm.StartGame();
        // 2. 启动游戏循环协程
        StartCoroutine(MainGameLoop());
    }

    IEnumerator MainGameLoop()
    {
        while (true) // �����淨ѭ��
        {
            // --- ���� 1: ���� ---
            Debug.Log("�������ƽ׶�...");
            // TODO: ����������������߼�

            // --- ���� 2: ����ѡ�� ---
            // ������Ե��� UI �����ѡ�������ݶ�Ϊ�Զ�
            Debug.Log($"��ǰѡ���������: {currentTheme}");

            // --- ���� 3 & 4: �ж������� & ����ս��Ʒ ---
            lastPlayer = gm.GetCurrentPlayer();
            Debug.Log($"�ֵ����: {lastPlayer.name} ����");

            // --- ���� 5: ���� (1~3��) ---
            yield return StartCoroutine(WaitForPlayerPlayCards());

            // --- ���� 6: ���� ---
            yield return StartCoroutine(WaitForChallenge());

            // ����ͼ�е��ж��߼��Ѿ���Э���д������
            yield return new WaitForSeconds(1f);
        }
    }

    // �ȴ���ҵ�� UI ����
    IEnumerator WaitForPlayerPlayCards()
    {
        Debug.Log("�ȴ�������ѡ����...");
        // ����Ӧ�õȴ� UI �¼���ģ���߼��������1-2����
        cardsOnTable.Clear();
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            if (lastPlayer.GetHandCount() > 0)
            {
                string c = lastPlayer.hand[0];
                lastPlayer.RemoveCard(c); // ʹ�ö��ѵ� RemoveCard
                cardsOnTable.Add(c);
            }
        }
        yield return new WaitForSeconds(1f);
    }

    // �ȴ�������ҵ������
    IEnumerator WaitForChallenge()
    {
        Debug.Log("�ȴ����� (3������������������)...");
        bool isChallenged = false;
        float timer = 3f;

        // ������ʵ�ʿ����л������ť���
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // ģ�������������
            if (Random.value > 0.8f) { isChallenged = true; break; }
            yield return null;
        }

        if (!isChallenged)
        {
            // --- ��֧���������� ---
            Debug.Log("�������ɣ��ƴ��");
            HandleCheckHandStatus(false);
        }
        else
        {
            // �����2����ҷ���������
            challenger = gm.GetAllPlayers()[1];
            ExecuteChallengeLogic();
        }
    }

    void ExecuteChallengeLogic()
    {
        bool isLie = false;
        foreach (string card in cardsOnTable)
        {
            // �ж��߼������������Ҳ��� Wild ����˵��
            if (!card.Contains(currentTheme) && card != "Wild")
            {
                isLie = true;
                break;
            }
        }

        if (isLie)
        {
            // --- ���ɳɹ� ---
            Debug.Log("���ɳɹ���");
            challenger.AddMask(); // ����д�� AddMask
            lastPlayer.RemoveMask(); // ����д�� RemoveMask

            // �ж��Ƿ�û�� (��Ӧ����ͼ)
            HandleCheckHandStatus(true);
        }
        else
        {
            // --- ����ʧ�� ---
            Debug.Log("����ʧ�ܣ�");
            challenger.RemoveMask();
            lastPlayer.AddMask();

            // �ƴ���߼�
            HandleCheckHandStatus(false);
        }
    }

    void HandleCheckHandStatus(bool wasChallengedSuccess)
    {
        // ����ͼ�ڵ㣺�Ƿ�û��
        if (lastPlayer.GetHandCount() == 0)
        {
            // Yes ��֧�����ƿ��ȡ��ͬ��������
            int count = cardsOnTable.Count;
            for (int i = 0; i < count; i++)
            {
                lastPlayer.AddCard(gm.GetDeck().DrawCard());
            }
            Debug.Log("û���ˣ����²��ƣ��ص����ƽ׶�");
            // ����ͼ�߼������ء����ơ�
        }
        else
        {
            // No 分支：切换到下一位玩家
            if (!wasChallengedSuccess)
            {
                // 牌成功打出，添加到战利品库
                AddCardsToTrophyPile();
            }
            gm.NextPlayer();
        }
    }

    // 将打出的牌添加到战利品库
    void AddCardsToTrophyPile()
    {
        if (lastPlayer == null || cardsOnTable.Count == 0) return;

        // 根据当前主题确定战利品类型
        TrophyType trophyType = GetTrophyTypeByTheme(currentTheme);

        // 每张牌添加一个战利品
        foreach (string card in cardsOnTable)
        {
            trophyManager.AddTrophy(lastPlayer.id, trophyType);
        }

        Debug.Log($"Player {lastPlayer.name} added {cardsOnTable.Count} trophies (Type: {trophyType})");
    }

    // 根据主题获取战利品类型
    TrophyType GetTrophyTypeByTheme(string theme)
    {
        switch (theme)
        {
            case "Animal":
                return TrophyType.Type1;
            case "Plant":
                return TrophyType.Type2;
            case "Food":
                return TrophyType.Type3;
            case "Color":
                return TrophyType.Type4;
            default:
                return TrophyType.Type5; // Wild或其他情况
        }
    }
}