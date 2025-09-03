using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

[Preserve]
[Serializable]
public class Message<T>
{
    public enum MsgType
    {
        // Request
        CREATE_ROOM, // null
        JOIN_ROOM, // String roomId
        LEAVE_ROOM, // null
        CHANGE_NAME, // String newName
        GET_ROOM_PLAYERS, // null
        PREPARE, // boolean isReady
        CANCEL_PREPARE, 
        START_GAME, // boolean
        PLAY_CARDS, // String cardId
        SKIP,
        CHALLENGE,
        RESTART,

        // Response
        WELCOME,
        ROOM_CREATED,
        ROOM_JOINED,
        ROOM_LEFT,
        NAME_CHANGED,
        ROOM_PLAYERS_LIST,
        PREPARED,
        CANCELLED_PREPARE,
        GAME_STARTED,

        // Error
        ERROR,
        INVALID_REQUEST,
        ROOM_NOT_FOUND,
        ALREADY_IN_ROOM,
        NAME_ALREADY_EXISTS,
        GAME_ALREADY_STARTED,
        NOT_PREPARED,
        NOT_IN_ROOM,
        ALREADY_PREPARED,
        ALREADY_STARTED,
        GAME_NOT_STARTED,
        GAME_ALREADY_FINISHED,
        GAME_NOT_FOUND,
        PLAYER_NOT_FOUND,
    }

    public string msgId;
    public MsgType msgType;
    public T data;

    public Message(MsgType msgType, T data)
    {
        this.msgId = Guid.NewGuid().ToString();
        this.msgType = msgType;
        this.data = data;
    }
    public Message() { }

    // 添加一个静态方法用于反序列化
    public static Message<T> FromJson<C>(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<Message<T>>(json);
        }
        catch (Exception ex)
        {
            Debug.Log($"Failed to deserialize message: {ex.Message}");
            Debug.Log(ex.StackTrace);
            return null;
        }
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }   
}