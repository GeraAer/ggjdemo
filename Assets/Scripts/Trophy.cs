using System.Collections.Generic;

public enum TrophyType
{
    Type1,  // 战利品类型1
    Type2,  // 战利品类型2
    Type3,  // 战利品类型3
    Type4,  // 战利品类型4
    Type5   // 战利品类型5
}

public class Trophy
{
    public TrophyType type;
    public string name;
    public string description;

    public Trophy(TrophyType type, string name, string description = "")
    {
        this.type = type;
        this.name = name;
        this.description = description;
    }
}

public class TrophyManager
{
    private Dictionary<int, Dictionary<TrophyType, int>> playerTrophyCounts = new Dictionary<int, Dictionary<TrophyType, int>>();

    public void AddTrophy(int playerId, TrophyType type)
    {
        if (!playerTrophyCounts.ContainsKey(playerId))
        {
            playerTrophyCounts[playerId] = new Dictionary<TrophyType, int>();
        }

        if (!playerTrophyCounts[playerId].ContainsKey(type))
        {
            playerTrophyCounts[playerId][type] = 0;
        }

        playerTrophyCounts[playerId][type]++;

        // 检查是否达到3个，触发效果
        if (playerTrophyCounts[playerId][type] == 3)
        {
            TriggerTrophyEffect(playerId, type);
        }
    }

    public int GetTrophyCount(int playerId, TrophyType type)
    {
        if (playerTrophyCounts.ContainsKey(playerId) && playerTrophyCounts[playerId].ContainsKey(type))
        {
            return playerTrophyCounts[playerId][type];
        }
        return 0;
    }

    public bool HasTrophyTriggered(int playerId, TrophyType type)
    {
        return GetTrophyCount(playerId, type) >= 3;
    }

    private void TriggerTrophyEffect(int playerId, TrophyType type)
    {
        switch (type)
        {
            case TrophyType.Type1:
                OnType1Triggered(playerId);
                break;
            case TrophyType.Type2:
                OnType2Triggered(playerId);
                break;
            case TrophyType.Type3:
                OnType3Triggered(playerId);
                break;
            case TrophyType.Type4:
                OnType4Triggered(playerId);
                break;
            case TrophyType.Type5:
                OnType5Triggered(playerId);
                break;
        }
    }

    // 五种战利品触发效果的空方法
    private void OnType1Triggered(int playerId)
    {
        // 战利品类型1达到3个时的效果
    }

    private void OnType2Triggered(int playerId)
    {
        // 战利品类型2达到3个时的效果
    }

    private void OnType3Triggered(int playerId)
    {
        // 战利品类型3达到3个时的效果
    }

    private void OnType4Triggered(int playerId)
    {
        // 战利品类型4达到3个时的效果
    }

    private void OnType5Triggered(int playerId)
    {
        // 战利品类型5达到3个时的效果
    }
}
