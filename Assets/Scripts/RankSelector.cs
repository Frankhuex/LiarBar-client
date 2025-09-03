using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankSelector : MonoBehaviour
{
    public Card.Rank Rank { get; private set; }
    [SerializeField] Button select2;
    [SerializeField] Button select3;
    [SerializeField] Button select4;
    [SerializeField] Button select5;
    [SerializeField] Button select6;
    [SerializeField] Button select7;
    [SerializeField] Button select8;
    [SerializeField] Button select9;
    [SerializeField] Button select10;
    [SerializeField] Button selectJ;
    [SerializeField] Button selectQ;
    [SerializeField] Button selectK;
    [SerializeField] Button selectA;

    private Dictionary<Card.Rank, Button> buttons;

    void Start()
    {
        Rank = Card.Rank.NULL;
        buttons = new Dictionary<Card.Rank, Button>
        {
            { Card.Rank.TWO, select2 },
            { Card.Rank.THREE, select3 },
            { Card.Rank.FOUR, select4 },
            { Card.Rank.FIVE, select5 },
            { Card.Rank.SIX, select6 },
            { Card.Rank.SEVEN, select7 },
            { Card.Rank.EIGHT, select8 },
            { Card.Rank.NINE, select9 },
            { Card.Rank.TEN, select10 },
            { Card.Rank.JACK, selectJ },
            { Card.Rank.QUEEN, selectQ },
            { Card.Rank.KING, selectK },
            { Card.Rank.ACE, selectA }
        };
        foreach (Card.Rank r in buttons.Keys)
        {
            buttons[r].onClick.AddListener(() => ButtonOnClick(r));
            buttons[r].GetComponentInChildren<TMP_Text>().color = Color.black;
        }
    }

    public void Refresh()
    {
        
        Debug.Log("Refreshing RankSelector");
        Rank = Card.Rank.NULL;
        if (buttons == null)
        {
            buttons = new Dictionary<Card.Rank, Button>
            {
                { Card.Rank.TWO, select2 },
                { Card.Rank.THREE, select3 },
                { Card.Rank.FOUR, select4 },
                { Card.Rank.FIVE, select5 },
                { Card.Rank.SIX, select6 },
                { Card.Rank.SEVEN, select7 },
                { Card.Rank.EIGHT, select8 },
                { Card.Rank.NINE, select9 },
                { Card.Rank.TEN, select10 },
                { Card.Rank.JACK, selectJ },
                { Card.Rank.QUEEN, selectQ },
                { Card.Rank.KING, selectK },
                { Card.Rank.ACE, selectA }
            };
        }
        Debug.Log("Buttons: " + buttons);
        foreach (Button b in buttons.Values)
        {
            Debug.Log("Refreshing button: " + b.name);
            Debug.Log("Found TMP_Text: " + b.GetComponentInChildren<TMP_Text>());
            b.GetComponentInChildren<TMP_Text>().color = Color.black;
        }
    }

    private void ButtonOnClick(Card.Rank r)
    {
        Rank = r;
        foreach (Button b in buttons.Values)
        {
            b.GetComponentInChildren<TMP_Text>().color = Color.black;
        }
        buttons[r].GetComponentInChildren<TMP_Text>().color = Color.red;
    }
}
