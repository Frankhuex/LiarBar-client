using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

[Preserve]
public class PlayerSelf : IPlayerDock
{
    [SerializeField] private GameObject playedCards;
    [SerializeField] private GameObject handCards;
    [SerializeField] private TMP_Text remainCardsNumLabel;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private Button buttonChallenge;
    [SerializeField] private Button buttonSkipChallenge;
    [SerializeField] private Button buttonPlayCards;

    [SerializeField] private RankSelector rankSelector;

    private CardManager handCardManager;
    private CardManager playedCardManager;



    public void Start()
    {
        buttonChallenge.onClick.AddListener(() => StartCoroutine(RoomManager.Instance.Challenge()));
        buttonSkipChallenge.onClick.AddListener(() => StartCoroutine(RoomManager.Instance.Skip()));
        buttonPlayCards.onClick.AddListener(() => StartCoroutine(OnPlayCardsButtonClick()));
    }

    public override void Init(Player player)
    {
        handCardManager = new CardManager(new List<Card>(), new Dictionary<Card, GameObject>(), handCards.transform, cardPrefab, Consts.HANDCARDS_SCALE * Vector3.one, 0f, Consts.HANDCARDS_OFFSET, CardManager.Alignment.CENTER, false);
        playedCardManager = new CardManager(new List<Card>(), new Dictionary<Card, GameObject>(), playedCards.transform, cardPrefab, Consts.PLAYEDCARDS_SCALE * Vector3.one, 0f, Consts.PLAYEDCARDS_OFFSET, CardManager.Alignment.CENTER, true);
        Refresh(player);
        Debug.Log("已初始化玩家信息：" + player.ToString());
    }
    public override void Refresh(Player player)
    {
        
        playerName.text = player.name;
        remainCardsNumLabel.text = player.handCards.Count.ToString();
        handCardManager.RefreshCards(player.handCards);
        playedCardManager.RefreshCards(player.playedCards);
        Debug.Log("已刷新玩家信息：" + player.ToString());
    }

    private IEnumerator OnPlayCardsButtonClick()
    {
        if (handCardManager.cardSequence.FindAll(card => handCardManager.IsPicked(card)).Count == 0)
        {
            Debug.Log("请选择牌");
            yield break;
        }

        Card.Rank toClaim = RoomManager.Instance.room.currentClaimRank;
        if (RoomManager.Instance.room.MustClaim(RoomManager.Instance.room.currentPlayerIndex))
        {
            toClaim = rankSelector.Rank;
        }
        
        yield return RoomManager.Instance.PlayCards(
            new PlayCards(
                handCardManager.cardSequence.FindAll(card => handCardManager.IsPicked(card)),
                toClaim
            )
        );
    }

    /////////////////////////////////////////////
    // 以下为出牌功能相关代码
    // --- 状态变量 ---
    private bool isDragging = false;          // 标记当前是否正在进行有效的拖拽操作
    private CardMouseDetector headCard = null;  // 记录拖拽的“头部”卡牌
    private CardMouseDetector tailCard = null;  // 记录拖拽的“尾部”卡牌

    void Update()
    {
        if (RoomManager.Instance.room!=null && RoomManager.Instance.room.started)
        {
            // 1. 当鼠标左键按下的瞬间
            if (Input.GetMouseButtonDown(0))
            {
                CardMouseDetector cardUnderMouse = GetCardUnderMouse();

                // 检查鼠标下方是否确实有一张牌
                if (cardUnderMouse != null)
                {
                    // 开始拖拽流程
                    isDragging = true;

                    // 将这张牌记为“头部”
                    headCard = cardUnderMouse;

                    // 在拖拽开始时，头部和尾部是同一张牌
                    tailCard = cardUnderMouse;

                    // Debug.Log("拖拽开始！头部: " + headCard.gameObject.name);

                    // (可选) 你可以在这里调用卡牌的方法，让它高亮显示为头部
                    ChangeColor(headCard, tailCard);
                }
            }
            // 2. 当鼠标左键持续按住，并且拖拽已经开始
            else if (Input.GetMouseButton(0) && isDragging)
            {
                CardMouseDetector cardUnderMouse = GetCardUnderMouse();

                // 检查鼠标下方是否有牌，并且这张牌不是当前的尾部牌
                if (cardUnderMouse != null && cardUnderMouse != tailCard)
                {
                    // (可选) 如果之前的尾部牌有特殊高亮，先取消它
                    // if (tailCard != null) tailCard.SetAsNormalVisual();

                    // 更新尾部牌为鼠标当前进入的这张新牌
                    tailCard = cardUnderMouse;
                    // Debug.Log("更新尾部: " + tailCard.gameObject.name);

                    // (可选) 让新的尾部牌高亮显示
                    ChangeColor(headCard, tailCard);

                }
            }
            // 3. 当鼠标左键松开的瞬间，并且之前正在拖拽
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                // Debug.Log("拖拽结束。头部: " + headCard.gameObject.name + ", 尾部: " + tailCard.gameObject.name);

                // --- 在这里调用你需要的最终函数 ---
                ProcessDragSelection(headCard, tailCard);

                // --- 重置所有状态，为下一次拖拽做准备 ---
                isDragging = false;

                // (可选) 取消头部和尾部牌的所有高亮效果
                // if (headCard != null) headCard.SetAsNormalVisual();
                // if (tailCard != null) tailCard.SetAsNormalVisual();
                ResumeColor();

                headCard = null;
                tailCard = null;
            }
        }
    }

    private List<Card> GetSelectedCards(CardMouseDetector startCard, CardMouseDetector endCard)
    {
        int startIndex = handCardManager.cardSequence.IndexOf(startCard.card);
        int endIndex = handCardManager.cardSequence.IndexOf(endCard.card);
        if (startIndex > endIndex)
        {
            (endIndex, startIndex) = (startIndex, endIndex);
        }

        List<Card> selectedCards = handCardManager.cardSequence.GetRange(startIndex, endIndex - startIndex + 1);
        return selectedCards;
    }

    private void ResumeColor()
    {
        foreach ((Card card, GameObject obj) in handCardManager.cardObjects)
        {
            obj.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void ChangeColor(CardMouseDetector startCard, CardMouseDetector endCard)
    {   
        ResumeColor();
        List<Card> selectedCards = GetSelectedCards(startCard, endCard);
        foreach (Card card in selectedCards)
        {
            if (handCardManager.cardObjects.TryGetValue(card, out GameObject obj))
            {
                obj.GetComponent<SpriteRenderer>().color = Consts.GREY;
            }
        }

    }

    /// <summary>
    /// 当拖拽结束时调用的函数
    /// </summary>
    /// <param name="startCard">拖拽开始的头部卡牌</param>
    /// <param name="endCard">拖拽结束的尾部卡牌</param>
    private void ProcessDragSelection(CardMouseDetector startCard, CardMouseDetector endCard)
    {
        // 你可以在这里实现你的游戏逻辑
        // 例如，选中从 headCard 到 tailCard 之间的所有牌
        // 或者执行一个从 headCard 指向 tailCard 的技能

        // Debug.Log("函数 ProcessDragSelection 已被调用！");
        List<Card> selectedCards = GetSelectedCards(startCard, endCard);
        foreach (Card card in selectedCards)
        {
            if (handCardManager.cardObjects[card].TryGetComponent<CardMouseDetector>(out var detector))
            {
                detector.TogglePick();
            }
        }

    }
    

    /// <summary>
    /// 使用射线投射，获取鼠标指针下最顶层的卡牌
    /// </summary>
    /// <returns>返回卡牌的脚本，如果没有则返回 null</returns>
    private CardMouseDetector GetCardUnderMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.transform.CompareTag("HandCard"))
        {
            return hit.collider.GetComponent<CardMouseDetector>();
        }
        return null;
    }




}
