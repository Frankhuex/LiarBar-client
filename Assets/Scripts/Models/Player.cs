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
        return JsonConvert.SerializeObject(this);
    }
}
