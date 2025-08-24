using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;


[Preserve]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerRightPrefab;
    [SerializeField] private GameObject playerLeftPrefab;
    [SerializeField] private GameObject playerTopPrefab;
    [SerializeField] private GameObject playerSelf;

    private GameObject[] docks; // Index follows playerList order
    private int selfIndex;
    private int playerCount;
    private Vector2[] positions;
    private Room room;
    private Player player;

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
        room = RoomManager.Instance.room;
        player = RoomManager.Instance.player;


        Debug.Log("Player Count: " + playerCount);
        Debug.Log("Self Index: " + selfIndex);

        docks = new GameObject[playerCount];
        docks[selfIndex] = playerSelf;
        docks[selfIndex].GetComponent<IPlayerDock>().Refresh(player);


        for (int i = 1; i < playerCount; i++) // Seat index, just a local variable, useless
        {
            CreateDockBySeatIndex(i);
            Debug.Log(docks[i].name + " " + docks[i].transform.position);
        }


        RoomManager.Instance.OnRoomRefreshed += RefreshGame;
    }

    public void RefreshGame(Room room)
    {
        for (int i = 0; i < playerCount; i++)
        {
            RefreshPlayer(i);
        }
    }

    private void RefreshPlayer(int playerIndex)
    {
        docks[playerIndex].GetComponent<IPlayerDock>().Refresh(room.playerList[playerIndex]);
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
        RefreshPlayer(index);
    }



}
