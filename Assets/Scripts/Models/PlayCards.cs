using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class PlayCards
{
    List<Card> cards;
    Card.Rank claimRank;

    public PlayCards() { }

    public PlayCards(List<Card> cards, Card.Rank claimRank)
    {
        this.cards = cards;
        this.claimRank = claimRank;
    }
}
