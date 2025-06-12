using System.Collections.Generic;

public static class PlayerDataManager
{
    public static Dictionary<int, PlayerData> PlayerDatas = new Dictionary<int, PlayerData>();

    public static void Clear() => PlayerDatas.Clear();

    public static void AddOrUpdatePlayer(PlayerData playerData)
    {
        PlayerDatas[playerData.playerId] = playerData;
    }

    public static bool TryGetPlayer(int playerId, out PlayerData playerData)
    {
        return PlayerDatas.TryGetValue(playerId, out playerData);
    }
}

[System.Serializable]
public struct PlayerData
{
    public int playerId;
    public string playerName;
    public int totalScore;
    public int totalCoins;
    public int totalBlitzCardsPlayed;
    public int totalActionCardsPlayed;
    public int totalSpecialCardsPlayed;
    public float overallScore;

    public PlayerData(int id, string name)
    {
        playerId = id;
        playerName = name;
        totalScore = 0;
        totalCoins = 0;
        totalBlitzCardsPlayed = 0;
        totalActionCardsPlayed = 0;
        totalSpecialCardsPlayed = 0;
        overallScore = 0f;
    }
}