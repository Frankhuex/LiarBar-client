using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;


[Preserve]
public class RoomManager
{
    private static RoomManager _instance;
    public static RoomManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RoomManager();
            }
            return _instance;
        }
    }

    public Room room;
    public Player player;

    public RoomManager()
    {
        room = null;
        // WebSocketClient.Instance.OnMessageReceived += text => RefreshRoom(text);
    }

    public event Action<Room> OnRoomRefreshed;
    public void RefreshRoom(string text)
    {
        Debug.Log("Refreshing room: " + text);
        Debug.Log("Parsing message");
        try
        {
            Message<Room> msg = Message<Room>.FromJson<Message<Room>>(text);
            Debug.Log("Message parsed: " + msg.ToString());
            if (msg.msgType == Message<Room>.MsgType.ROOM_PLAYERS_LIST)
            {
                room = msg.data;
                player = room.playerList.Find(x => x.userId == WebSocketClient.Instance.userId);
                Debug.Log("Room refreshed: " + room.ToString() + " Message ID: " + msg.msgId);
                OnRoomRefreshed?.Invoke(room);
                Debug.Log("Room refreshed event invoked." + " Message ID: " + msg.msgId);
            }
            else
            {
                Debug.Log("Wrong msg type: " + msg.msgType);
                Debug.Log("Not refreshing room.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing message: " + text);
            Debug.LogError("Caught exception: " + ex.Message);
            Debug.LogError(ex.StackTrace);
            // try
            // {
            //     Message<string> msg2 = Message<string>.FromJson<Message<string>>(text);
            //     Debug.Log("Message not REFRESH_ROOM: " + msg2.ToString());
            // }
            // catch (System.Exception ex2)
            // {
            //     Debug.Log("Error parsing message: " + text);
            //     Debug.Log("Caught exception: " + ex2.Message);
            //     Debug.Log("Stack trace: " + ex2.StackTrace);
            // }
        }
        finally
        {
            Debug.Log("Room refresh completed.");
        }
    }

    public IEnumerator CreateRoom()
    {
        Message<string> msg = new Message<string>(Message<string>.MsgType.CREATE_ROOM, "test");
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Created room request sent.");
    }

    public IEnumerator JoinRoom(string roomId)
    {
        Message<string> msg = new Message<string>(Message<string>.MsgType.JOIN_ROOM, roomId);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Joined room request sent: "+roomId);
    }

    public IEnumerator LeaveRoom()
    {
        if (room == null)
        {
            Debug.LogError("Cannot leave room, not in a room.");
            yield break;
        }
        Message<string> msg = new(Message<string>.MsgType.LEAVE_ROOM, room.id);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        room = null; // Clear the room after leaving
        player = null;
        Debug.Log("Left room request sent.");
    }

    public IEnumerator Ready(bool ready)
    {
        if (room == null || player == null)
        {
            Debug.LogError("Cannot change ready status, not in a room or player not found.");
            yield break;
        }
        Message<bool> msg = new(Message<bool>.MsgType.PREPARE, ready);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Ready request sent: " + ready);
    }

    public IEnumerator ChangeName(string name)
    {
        if (room == null || player == null)
        {
            Debug.LogError("Cannot change name, not in a room or player not found.");
            yield break;
        }
        Message<string> msg = new Message<string>(Message<string>.MsgType.CHANGE_NAME, name);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Name changed request sent: " + name);
    }

    public IEnumerator StartGame()
    {
        if (room == null || player == null)
        {
            Debug.LogError("Cannot start game, not in a room or player not found.");
            yield break;
        }
        if (!player.host)
        {
            Debug.LogError("Only the host can start the game.");
            yield break;
        }
        if (room.started)
        {
            Debug.LogError("Game already started.");
            yield break;
        }
        if (!room.playerList.TrueForAll(p => p.ready))
        {
            Debug.LogError("Not all players are ready.");
            yield break;
        }
        Message<string> msg = new Message<string>(Message<string>.MsgType.START_GAME, room.id);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Start Game request sent:");
    }

    public IEnumerator PlayCards(PlayCards playcards)
    {
        if (room == null || player == null)
        {
            Debug.LogError("Cannot play cards, not in a room or player not found.");
            yield break;
        }
        if (!room.started)
        {
            Debug.LogError("Game not started.");
            yield break;
        }
        if (!room.CanPlayCards(GetSelfIndex()))
        {
            Debug.LogError("It's not your turn, or there are claim to be challenged or skipped, even if you are the round beginner of a old round.");
            yield break;
        }

        Message<PlayCards> msg = new(Message<PlayCards>.MsgType.PLAY_CARDS, playcards);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Play Cards request sent: " + msg.ToString());
    }

    public IEnumerator Skip()
    {
        if (room == null || player == null)
        {
            Debug.LogError("Cannot play cards, not in a room or player not found.");
            yield break;
        }
        if (!room.started || room.ended)
        {
            Debug.LogError("Game not started or already ended.");
            yield break;
        }
        if (!room.CanChallengeOrSkip(GetSelfIndex()))
        {
            Debug.LogError("Not your turn, or you are the round beginner of a new round.");
            yield break;
        }
        Message<PlayCards> msg = new(Message<PlayCards>.MsgType.SKIP, null);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Skip request sent.");
    }

    public IEnumerator Challenge()
    {
        if (room == null || player == null)
        {
            Debug.LogError("Cannot play cards, not in a room or player not found.");
            yield break;
        }
        if (!room.started || room.ended)
        {
            Debug.LogError("Game not started or already ended.");
            yield break;
        }
        if (!room.CanChallengeOrSkip(GetSelfIndex()))
        {
            Debug.LogError("Not your turn, or you are the round beginner of a new round.");
            yield break;
        }
        Message<PlayCards> msg = new(Message<PlayCards>.MsgType.CHALLENGE, null);
        yield return WebSocketClient.Instance.SendMessageCoroutine(msg);
        Debug.Log("Challenge request sent.");
    }


    public int GetSelfIndex()
    {
        if (room == null || player == null)
        {
            Debug.LogError("Cannot get self index, not in a room or player not found.");
            return -1;
        }
        return room.playerList.FindIndex(x => x.userId == player.userId);
    }

}
