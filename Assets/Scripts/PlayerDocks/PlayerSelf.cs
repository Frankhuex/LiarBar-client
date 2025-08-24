using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class PlayerSelf : IPlayerDock
{
    [SerializeField] private GameObject playedCards;
    [SerializeField] private GameObject handCards;
    [SerializeField] private TMP_Text remainCardsNumLabel;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject cardPrefab;

    private CardManager handCardManager;
    private CardManager playedCardManager;

    Dictionary<Card, GameObject> handCardsObjects = new Dictionary<Card, GameObject>();
    Dictionary<Card, GameObject> playedCardsObjects = new Dictionary<Card, GameObject>();
    List<Card> handCardsSequence = new List<Card>();
    List<Card> playedCardsSequence = new List<Card>();

    public void Start()
    {
        handCardManager = new CardManager(handCardsSequence, handCardsObjects, handCards.transform, cardPrefab, Consts.HANDCARDS_SCALE * Vector3.one, 0f, Consts.HANDCARDS_OFFSET);
        playedCardManager = new CardManager(playedCardsSequence, playedCardsObjects, playedCards.transform, cardPrefab, Consts.PLAYEDCARDS_SCALE * Vector3.one, 0f, Consts.PLAYEDCARDS_OFFSET);
    }


    

    public override void Refresh(Player player)
    {
        playerName.text = player.name;
        remainCardsNumLabel.text = player.handCards.Count.ToString();
        handCardManager.RefreshCards(player.handCards);
        playedCardManager.RefreshCards(player.playedCards);
    }




}
