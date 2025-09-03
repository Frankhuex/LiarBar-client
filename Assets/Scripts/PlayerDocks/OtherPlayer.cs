using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

[Preserve]
public class OtherPlayer : IPlayerDock
{
    [SerializeField] private GameObject playedCards;
    [SerializeField] private TMP_Text remainCardsNumLabel;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private CardManager.Alignment alignment;

    private CardManager playedCardManager;


    public override void Init(Player player)
    {
        playedCardManager = new CardManager(new List<Card>(), new Dictionary<Card, GameObject>(), playedCards.transform, cardPrefab, Consts.PLAYEDCARDS_SCALE * Vector3.one, 0f, Consts.PLAYEDCARDS_OFFSET, alignment, true);
        Refresh(player);
        Debug.Log("已初始化玩家信息：" + player.ToString());
    }
    public override void Refresh(Player player)
    {
        
        playerName.text = player.name;
        remainCardsNumLabel.text = player.handCards.Count.ToString();
        playedCardManager.RefreshCards(player.playedCards);
        Debug.Log("已刷新玩家信息：" + player.ToString());
    }
}
