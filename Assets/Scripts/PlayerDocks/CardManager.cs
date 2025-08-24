using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class CardManager
{
    public List<Card> cardSequence;
    public Dictionary<Card, GameObject> cardObjects;
    public Transform parent;
    public Vector3 localScale;
    public GameObject cardPrefab;
    public float relativeY;
    public float offset;
    public CardManager(List<Card> cardSequence, Dictionary<Card, GameObject> cardObjects, Transform parent, GameObject cardPrefab, Vector3 localScale, float relativeY, float offset)
    {
        this.cardSequence = cardSequence;
        this.cardObjects = cardObjects;
        this.parent = parent;
        this.localScale = localScale;
        this.cardPrefab = cardPrefab;
        this.relativeY = relativeY;
        this.offset = offset;
    }

    public void AddOrDeleteCard(Card card)
    {
        if (cardObjects.ContainsKey(card))
        {
            GameObject cardObject = cardObjects[card];
            cardSequence.Remove(card);
            cardObjects.Remove(card);
            GameObject.Destroy(cardObject);
        }
        else
        {
            GameObject cardObject = Card.CreateCardObject
            (
                card,
                cardPrefab,
                parent,
                new Vector3(0f, 0f, 0f),
                localScale
            );
            cardSequence.Add(card);
            cardObjects.Add(card, cardObject);
        }
    }

    public void SortCards()
    {
        cardSequence.Sort();
        int cardCount = cardSequence.Count;
        float x0 = (1 - cardCount) / 2f * offset;
        for (int i = 0; i < cardCount; i++)
        {
            Card card = cardSequence[i];
            GameObject cardObject = cardObjects[card];
            cardObject.transform.position = parent.position + new Vector3(x0 + i * offset, 0f, 10 - 0.1f * i);
        }
    }
    
    public void RefreshCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            AddOrDeleteCard(card);
        }
        SortCards();
    }
}
