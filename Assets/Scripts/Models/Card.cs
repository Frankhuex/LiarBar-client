using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[Serializable]
public class Card : IComparable<Card>, IEquatable<Card>
{
    public enum Suit
    {
        HEARTS, DIAMONDS, CLUBS, SPADES, UNKNOWN
        // 红桃，方块，梅花，黑桃
    }

    public enum Rank
    {
        NULL, ACE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING
    }

    public Suit suit;
    public Rank rank;

    public Card() { }
    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public override string ToString()
    {
        string suitName = suit.ToString()[0..1] + suit.ToString()[1..].ToLower();
        string rankName = rank switch
        {
            Rank.NULL => "Null",
            Rank.ACE => "A",
            Rank.TWO => "2",
            Rank.THREE => "3",
            Rank.FOUR => "4",
            Rank.FIVE => "5",
            Rank.SIX => "6",
            Rank.SEVEN => "7",
            Rank.EIGHT => "8",
            Rank.NINE => "9",
            Rank.TEN => "10",
            Rank.JACK => "J",
            Rank.QUEEN => "Q",
            Rank.KING => "K",
            _ => ""
        };
        return suitName + "_" + rankName;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Card);
    }

    public bool Equals(Card other)
    {
        if (other == null) return false;
        return this.suit == other.suit && this.rank == other.rank;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(suit, rank);
    }

    public static bool MoveCards(List<Card> from, List<Card> to, List<Card> cards)
    {
        if (from == null || to == null || cards == null || cards.Count == 0)
            return false;

        // 检查所有 card 是否都在 from 中
        if (!cards.All(c => from.Contains(c)))
            return false;

        try
        {
            foreach (var card in cards)
            {
                from.Remove(card);
                to.Add(card);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool MoveAllCards(List<Card> from, List<Card> to)
    {
        if (from == null || to == null) return false;

        to.AddRange(from);
        from.Clear();
        return true;
    }

    public int CompareTo(Card other)
    {
        if (other == null)
        {
            return 1;
        }
        int rankCompare = this.rank.CompareTo(other.rank);
        if (rankCompare != 0)
        {
            return rankCompare;
        }
        return this.suit.CompareTo(other.suit);
    }

    public static GameObject CreateCardObject(Card card, GameObject cardPrefab, Transform parent, Vector3 relativePosition, Vector3 localScale)
    {
        Sprite sprite = PokerSpriteManager.Instance.GetPokerSprite(card);
        Debug.Log("Get card sprite: " + sprite.name);
        GameObject cardObject = GameObject.Instantiate(cardPrefab, parent);
        cardObject.transform.position = parent.position + relativePosition;
        cardObject.transform.localScale = localScale;
        cardObject.GetComponent<SpriteRenderer>().sprite = sprite;
        cardObject.name = card.ToString();
        CardMouseDetector detector = cardObject.GetComponent<CardMouseDetector>();
        if (detector != null)
        {
            detector.card = card;
        }
        Debug.Log("Create card object: " + card.ToString() + " - " + cardObject.name);
        return cardObject;
    }
}
