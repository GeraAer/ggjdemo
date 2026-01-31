using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 单例模式，保证全局只有一个游戏管理器
    public static GameManager Instance;

    [Header("设置")]
    // 在编辑器中拖入4个空物体，代表4个玩家的座位位置 (0:自己, 1:左, 2:上, 3:右)
    public Transform[] playerPositions;

    // 核心数据
    private Deck deck;
    private List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;

    private void Awake()
    {
        // 确保单例唯一性
        if (Instance == null)
        {
            Instance = this;
            // 切换场景时不销毁 (可选，根据你需求保留或删除)
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化游戏数据（创建牌库、创建玩家）
    /// 注意：这里不再发牌，发牌逻辑已移交 GameLoopController 的“摸牌阶段”
    /// </summary>
    public void StartGame()
    {
        // 1. 初始化新牌库
        deck = new Deck();

        // 2. 初始化玩家列表
        players.Clear();
        string[] defaultNames = { "Me", "LeftBot", "TopBot", "RightBot" };

        for (int i = 0; i < 4; i++)
        {
            // 创建纯数据玩家对象
            Player p = new Player(i, defaultNames[i]);
            players.Add(p);
        }

        // 3. 重置当前玩家指针
        currentPlayerIndex = 0;

        Debug.Log("GameManager: 游戏数据初始化完成 (等待 GameLoop 开始流程)");
    }

    // --- 数据获取与操作接口 ---

    public Player GetCurrentPlayer()
    {
        if (players.Count == 0) return null;
        return players[currentPlayerIndex];
    }

    public List<Player> GetAllPlayers()
    {
        return players;
    }

    public void NextPlayer()
    {
        if (players.Count == 0) return;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
    }

    public Deck GetDeck()
    {
        return deck;
    }

    /// <summary>
    /// 获取指定玩家ID的座位位置
    /// </summary>
    public Transform GetPlayerPosition(int playerId)
    {
        if (playerPositions != null && playerId >= 0 && playerId < playerPositions.Length)
        {
            return playerPositions[playerId];
        }
        return transform; // 如果没设置，默认返回Manager的位置，防止报错
    }
}