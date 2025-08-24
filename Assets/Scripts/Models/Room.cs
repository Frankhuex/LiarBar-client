using System;
using System.Collections;
using System.Collections.Generic;
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
        return JsonUtility.ToJson(this);
    }
}
