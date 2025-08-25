using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;


[Preserve]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerRightPrefab;
    [SerializeField] private GameObject playerLeftPrefab;
    [SerializeField] private GameObject playerTopPrefab;
    [SerializeField] private GameObject playerSelf;

    [SerializeField] private GameObject inGameActionButtons;
    [SerializeField] private Button buttonChallenge;
    [SerializeField] private Button buttonSkipChallenge;
    [SerializeField] private Button buttonPlayCards;

    private GameObject[] docks; // Index follows playerList order
    private int selfIndex;
    private int playerCount;
    private Vector2[] positions;
    private Room savedRoom;
    private Player savedPlayer;

    private int SeatIndex2PlayerIndex(int seatIndex)
    {
        return (selfIndex + seatIndex) % playerCount;
    }

    private int PlayerIndex2SeatIndex(int playerIndex)
    {
        return (playerIndex - selfIndex + playerCount) % playerCount;
    }

    public void StartGame()
    {
        Debug.Log("Start Game");
        selfIndex = RoomManager.Instance.GetSelfIndex();
        playerCount = RoomManager.Instance.room.playerList.Count;
        positions = Consts.GetPositionsByPlayerCount(playerCount);
        savedRoom = RoomManager.Instance.room;
        savedPlayer = RoomManager.Instance.player;


        Debug.Log("Player Count: " + playerCount);
        Debug.Log("Self Index: " + selfIndex);

        docks = new GameObject[playerCount];
        docks[selfIndex] = playerSelf;
        docks[selfIndex].GetComponent<IPlayerDock>().Init(savedPlayer);


        for (int i = 1; i < playerCount; i++) // Seat index, just a local variable, useless
        {
            CreateDockBySeatIndex(i);
        }

        Debug.Log("Docks created. Check my turn:");
        CheckMyTurn();


        RoomManager.Instance.OnRoomRefreshed += RefreshGame;

    }

    public void CheckMyTurn()
    {
        Debug.Log("Check my turn");
        if (savedRoom.IsMyTurn(selfIndex))
        {
            Debug.Log("It's my turn");
            inGameActionButtons.SetActive(true);
            if (savedRoom.AmIRoundBeginner(selfIndex))
            {
                Debug.Log("It's my turn and I am the round beginner");
                buttonChallenge.gameObject.SetActive(false);
                buttonSkipChallenge.gameObject.SetActive(false);
                buttonPlayCards.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("It's my turn but I am not the round beginner");
                buttonChallenge.gameObject.SetActive(true);
                buttonSkipChallenge.gameObject.SetActive(true);
                buttonPlayCards.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("It's not my turn");
            inGameActionButtons.SetActive(false);
        }
    }

    private void RefreshPlayer(int playerIndex)
    {
        docks[playerIndex].GetComponent<IPlayerDock>().Refresh(savedRoom.playerList[playerIndex]);
    }

    public void RefreshGame(Room room)
    {
        Debug.Log("Refresh Game");
        savedRoom = room;
        for (int i = 0; i < playerCount; i++)
        {
            RefreshPlayer(i);
        }
        CheckMyTurn();
    }

    private void InitPlayer(int playerIndex)
    {
        docks[playerIndex].GetComponent<IPlayerDock>().Init(savedRoom.playerList[playerIndex]);
    }

    private void CreateDockBySeatIndex(int seatIndex)
    {
        int index = SeatIndex2PlayerIndex(seatIndex);
        GameObject prefab = playerTopPrefab;
        if (playerCount > 3)
        {
            if (seatIndex == 1)
            {
                prefab = playerRightPrefab;
            }
            else if (seatIndex == playerCount - 1)
            {
                prefab = playerLeftPrefab;
            }
        }
        docks[index] = Instantiate(prefab, positions[seatIndex], Quaternion.identity);
        docks[index].transform.parent = transform;
        InitPlayer(index);
    }



}
