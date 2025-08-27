using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[Serializable]
public class Player
{
    public string userId;
    public string name;
    public bool active;
    public bool ready;
    public bool host;
    public string roomId;
    public List<Card> handCards;
    public List<Card> playedCards;

    public Player(string userId, string name, bool active, bool ready, bool host, string roomId)
    {
        this.userId = userId;
        this.name = name;
        this.active = active;
        this.ready = ready;
        this.host = host;
        this.roomId = roomId;
        handCards = new List<Card>();
        playedCards = new List<Card>();
    }
    public Player() { }
    public override string ToString()
    {
        // Math.Min 用於確保即使 userId 的長度小於 5 也不會丟擲例外。
        string truncatedUserId = userId.Substring(0, Math.Min(5, userId.Length));
        
        // 使用字串內插來格式化輸出，並透過三元運算子決定狀態字串。
        return $"{truncatedUserId}\t"
            + $"{name}\t"
            + $"{(active ? "Active" : "Inactive")}\t"
            + $"{(ready ? "Ready" : "NotReady")}\t"
            + $"{(host ? "Host" : "NotHost")}\t"
            + $"{handCards.Count}\t"
            + $"{playedCards.Count}\n";
    }
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
