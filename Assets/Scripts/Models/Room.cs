using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[Serializable]
public class Room
{
    public string id;
    public List<Player> playerList;
    public int maxPlayers;
    public bool started;
    public bool ended;

    public List<Card> cardDeck;
    public int currentPlayerIndex;
    public int roundBeginnerIndex;
    public Card.Rank currentClaimRank;
    public Player winner;

    public Room(string id, int maxPlayers)
    {
        this.id = id;
        this.maxPlayers = maxPlayers;
        playerList = new List<Player>();
        started = false;
        ended = false;
        cardDeck = new List<Card>();
        currentPlayerIndex = 0;
        roundBeginnerIndex = 0;
        currentClaimRank = Card.Rank.NULL;
        winner = null;
    }
    public Room() { }
    public override string ToString()
    {
        // StringBuilder 在迴圈中比字串串接更有效率。
        var sb = new StringBuilder();
        
        sb.AppendLine("--------------------------------------------------------------------");
        sb.AppendLine($"Room {id}");
        sb.AppendLine("--------------------------------------------------------------------");
        sb.AppendLine("ID\tName       \tActive\tReady\tHost\tHand\tPlayed");
        sb.AppendLine("--------------------------------------------------------------------");

        // 遍歷玩家列表並附加每個玩家的字串表示。
        foreach (var player in playerList)
        {
            sb.Append(player.ToString());
        }

        sb.AppendLine("---------------------------------------------------------------------");
        sb.AppendLine($"Max players: {maxPlayers}");
        sb.AppendLine($"Started: {started}");
        sb.AppendLine($"Ended: {ended}");
        
        // 使用 null 條件運算子 (?.) 來處理 CardDeck 可能為 null 的情況。
        sb.AppendLine($"Card deck: {(cardDeck == null ? "null" : cardDeck.Count.ToString())}");
        
        sb.AppendLine($"Current player: {currentPlayerIndex}");
        sb.AppendLine($"Round beginner: {roundBeginnerIndex}");
        sb.AppendLine($"Current claim rank: {currentClaimRank}");
        
        // 同樣地，處理 Winner 可能為 null 的情況。
        sb.AppendLine($"Winner: {(winner == null ? "null" : winner.name)}");
        
        sb.AppendLine("---------------------------------------------------------------------");

        return sb.ToString();
    }
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public bool IsMyTurn(int myIndex)
    {
        return !ended && currentPlayerIndex == myIndex;
    }

    public bool AmIRoundBeginner(int myIndex)
    {
        return currentPlayerIndex == roundBeginnerIndex;
    }
    public bool CanPlayCards(int myIndex)
    {
        return !ended && IsMyTurn(myIndex) && ((AmIRoundBeginner(myIndex) && currentClaimRank == Card.Rank.NULL) || !AmIRoundBeginner(myIndex));
    }

    public bool CanChallengeOrSkip(int myIndex)
    {
        return !ended && IsMyTurn(myIndex) && currentClaimRank!= Card.Rank.NULL;
    }
}
