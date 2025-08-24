using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

[Preserve]
public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject connectedMenu;
    [SerializeField] private GameObject joinRoomMenu;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject menuBackground;




    [SerializeField] private Button buttonSetting;
    [SerializeField] private Button buttonCancelSetting;
    [SerializeField] private Button buttonSaveSetting;
    [SerializeField] private Button buttonStartAndConnect;
    [SerializeField] private Button buttonDisconnect;
    [SerializeField] private Button buttonCreateRoom;
    [SerializeField] private Button buttonOpenJoinRoomMenu;
    [SerializeField] private Button buttonJoinRoom;
    [SerializeField] private Button buttonCancelJoinRoom;
    [SerializeField] private Button buttonLeaveRoom;
    [SerializeField] private Button buttonReady;
    [SerializeField] private Button buttonHostStart;
    [SerializeField] private Button buttonChangeName;


    [SerializeField] private TMP_InputField urlInput;
    [SerializeField] private TMP_InputField roomIdInput;
    [SerializeField] private TMP_InputField changeNameInput;

    [SerializeField] private TMP_Text textConnected;
    [SerializeField] private TMP_Text textRoomId;

    [SerializeField] private GameObject lobbyScrollViewContent;
    [SerializeField] private GameObject lobbyListItemBase;

    private Action<Room> onRoomRefreshedCallback = null;


    public void OpenSettingMenu()
    {
        mainMenu.SetActive(false);
        settingMenu.SetActive(true);
        urlInput.text = WebSocketClient.Instance.url;
        Debug.Log("URL: " + WebSocketClient.Instance.url);
    }

    public void CloseSettingMenuAndCancel()
    {
        settingMenu.SetActive(false);
        mainMenu.SetActive(true);
        Debug.Log("URL: " + WebSocketClient.Instance.url);
    }

    public void CloseSettingMenuAndSave()
    {
        string url = urlInput.text;
        WebSocketClient.Instance.url = url;
        settingMenu.SetActive(false);
        mainMenu.SetActive(true);
        Debug.Log("URL: " + WebSocketClient.Instance.url);
    }

    public IEnumerator HandleStartButton()
    {
        Action callback = null;
        callback = () =>
        {
            mainMenu.SetActive(false);
            connectedMenu.SetActive(true);
            textConnected.text = "Connected to\n" + WebSocketClient.Instance.url;

            WebSocketClient.Instance.OnConnected -= callback;
            Debug.Log("Connected to WebSocket server: " + WebSocketClient.Instance.url);
        };
        WebSocketClient.Instance.OnConnected += callback;
        WebSocketClient.Instance.Connect();
        yield break;
    }

    public IEnumerator HandleDisconnectButton()
    {
        WebSocketClient.Instance.Close();
        connectedMenu.SetActive(false);
        mainMenu.SetActive(true);
        yield break;
    }

    public void OpenJoinRoomMenu()
    {
        connectedMenu.SetActive(false);
        joinRoomMenu.SetActive(true);
        roomIdInput.text = "";
        Debug.Log("Join Room Menu Opened");
    }

    public void CloseJoinRoomMenu()
    {
        joinRoomMenu.SetActive(false);
        connectedMenu.SetActive(true);
        Debug.Log("Join Room Menu Closed");
    }

    public IEnumerator HandleCreateRoomButton()
    {
        WebSocketClient.Instance.OnMessageReceived += RoomManager.Instance.RefreshRoom;
        if (onRoomRefreshedCallback != null)
        {
            RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
        }
        onRoomRefreshedCallback = (Room room) =>
        {
            connectedMenu.SetActive(false);
            lobby.SetActive(true);
            StartCoroutine(RefreshLobbyList(room));
        };
        RoomManager.Instance.OnRoomRefreshed += onRoomRefreshedCallback;
        yield return RoomManager.Instance.CreateRoom();
    }

    public IEnumerator HandleJoinRoomButton()
    {
        WebSocketClient.Instance.OnMessageReceived += RoomManager.Instance.RefreshRoom;
        Debug.Log("Join Room Button Pressed");
        string roomId = roomIdInput.text;
        if (onRoomRefreshedCallback != null)
        {
            RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
        }
        onRoomRefreshedCallback = (Room room) =>
        {
            joinRoomMenu.SetActive(false);
            lobby.SetActive(true);
            StartCoroutine(RefreshLobbyList(room));
        };
        RoomManager.Instance.OnRoomRefreshed += onRoomRefreshedCallback;
        yield return RoomManager.Instance.JoinRoom(roomId);
    }

    public IEnumerator HandleLeaveRoomButton1()
    {
        Debug.Log("Leave Room Button Pressed");
        if (onRoomRefreshedCallback != null)
        {
            RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
        }
        void callback(string text)
        {
            WebSocketClient.Instance.OnMessageReceived -= callback;
            try
            {
                Message<string> message = JsonUtility.FromJson<Message<string>>(text);
                if (message.msgType == Message<string>.MsgType.ROOM_LEFT)
                {
                    if (onRoomRefreshedCallback != null)
                    {
                        RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
                    }
                    // tcs.SetResult(true);
                    lobby.SetActive(false);
                    connectedMenu.SetActive(true);
                    textRoomId.text = "";
                    Debug.Log("Left Room");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing leave room message: {ex.Message}");
            }

        }
        WebSocketClient.Instance.OnMessageReceived += callback;
        yield return RoomManager.Instance.LeaveRoom();
        
        Debug.Log("Leave Room Button Finished");

    }

    public IEnumerator HandleLeaveRoomButton()
    {
        lobby.SetActive(false);
        connectedMenu.SetActive(true);
        textRoomId.text = "";
        WebSocketClient.Instance.OnMessageReceived -= RoomManager.Instance.RefreshRoom;
        if (onRoomRefreshedCallback != null)
        {
            RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
        }
        yield return RoomManager.Instance.LeaveRoom();
        Debug.Log("Left Room");
    }

    public IEnumerator RefreshLobbyList(Room room)
    {
        if (room == null) yield break;

        if (room.started)
        {
            HandleClientStart();
            yield break;
        }

        textRoomId.text = "Room ID:\n" + room.id;
        foreach (Transform child in lobbyScrollViewContent.transform)
        {
            if (child != lobbyListItemBase.transform)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < room.playerList.Count; i++)
        {
            Player player = room.playerList[i];
            GameObject lobbyListItem = Instantiate(lobbyListItemBase, lobbyScrollViewContent.transform);
            lobbyListItem.SetActive(true);
            TMP_Text[] texts = lobbyListItem.GetComponentsInChildren<TMP_Text>();
            Debug.Log(player.ToString());
            texts[0].text = (i + 1).ToString();
            texts[1].text = player.name;
            texts[2].text = player.host ? "Host" : "Player";
            texts[3].text = player.ready ? "Ready" : "Not Ready";
        }
        buttonReady.GetComponentInChildren<TMP_Text>().text = RoomManager.Instance.player.ready ? "Ready: Yes" : "Ready: No";
        buttonHostStart.gameObject.SetActive(
            RoomManager.Instance.player.host
            && room.playerList.TrueForAll(p => p.ready)
            && !room.started
        );
        changeNameInput.text = RoomManager.Instance.player.name;
        if (room.started)
        {
            Debug.Log($"Game ({room.id}) has started");
        }
    }

    public IEnumerator HandleReadyButton()
    {
        if (RoomManager.Instance.room == null || RoomManager.Instance.player == null)
        {
            Debug.LogError("Cannot ready, not in a room or player not found.");
            yield break;
        }
        bool ready = !RoomManager.Instance.player.ready;
        yield return RoomManager.Instance.Ready(ready);
    }

    public IEnumerator HandleChangeNameButton()
    {
        if (RoomManager.Instance.room == null || RoomManager.Instance.player == null)
        {
            Debug.LogError("Cannot ready, not in a room or player not found.");
            yield break;
        }
        string name = changeNameInput.text;
        yield return RoomManager.Instance.ChangeName(name);
    }

    public IEnumerator HandleHostStartButton()
    {
        if (RoomManager.Instance.room == null || RoomManager.Instance.player == null)
        {
            Debug.LogError("Cannot start game, not in a room or player not found.");
            yield break;
        }
        if (!RoomManager.Instance.player.host)
        {
            Debug.LogError("Only the host can start the game.");
            yield break;
        }
        if (RoomManager.Instance.room.started)
        {
            Debug.LogError("Game already started.");
            yield break;
        }
        if (!RoomManager.Instance.room.playerList.TrueForAll(p => p.ready))
        {
            Debug.LogError("Not all players are ready.");
            yield break;
        }

        RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
        onRoomRefreshedCallback = (Room room) =>
        {
            Debug.Log("New room: " + room.ToString());
            Debug.Log("Stored room: " + RoomManager.Instance.room.ToString());
            Debug.Log("Stored player: " + RoomManager.Instance.player.ToString());
            RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
            lobby.SetActive(false);
            menuBackground.SetActive(false);
            gameManager.StartGame();
        };
        RoomManager.Instance.OnRoomRefreshed += onRoomRefreshedCallback;
        yield return RoomManager.Instance.StartGame();
    }

    public IEnumerator HandleClientStart()
    {
        RoomManager.Instance.OnRoomRefreshed -= onRoomRefreshedCallback;
        lobby.SetActive(false);
        menuBackground.SetActive(false);
        gameManager.StartGame();
        yield break;
    }

    public void ActivateInput(TMP_InputField field)
    {
        field.Select();
        field.ActivateInputField();
    }


    // Start is called before the first frame update
    void Start()
    {
        menuBackground.SetActive(true);
        mainMenu.SetActive(true);

        foreach (GameObject menu in new GameObject[] { settingMenu, connectedMenu, joinRoomMenu, lobby })
        {
            menu.SetActive(false);
        }
        urlInput.onSelect.AddListener((string text) =>
        {
            ActivateInput(urlInput);
        });
        roomIdInput.onSelect.AddListener((string text) =>
        {
            ActivateInput(roomIdInput);
        });
        changeNameInput.onSelect.AddListener((string text) =>
        {
            ActivateInput(changeNameInput);
        });

        buttonSetting.onClick.AddListener(OpenSettingMenu);
        buttonCancelSetting.onClick.AddListener(CloseSettingMenuAndCancel);
        buttonSaveSetting.onClick.AddListener(CloseSettingMenuAndSave);
        buttonOpenJoinRoomMenu.onClick.AddListener(OpenJoinRoomMenu);
        buttonCancelJoinRoom.onClick.AddListener(CloseJoinRoomMenu);

        buttonStartAndConnect.onClick.AddListener(() =>
        {
            StartCoroutine(HandleStartButton());
        });
        buttonDisconnect.onClick.AddListener(() =>
        {
            StartCoroutine(HandleDisconnectButton());
        });
        buttonCreateRoom.onClick.AddListener(() =>
        {
            StartCoroutine(HandleCreateRoomButton());
        });
        buttonJoinRoom.onClick.AddListener(() =>
        {
            StartCoroutine(HandleJoinRoomButton());
        });
        buttonLeaveRoom.onClick.AddListener(() =>
        {
            StartCoroutine(HandleLeaveRoomButton());
        });
        buttonReady.onClick.AddListener(() =>
        {
            StartCoroutine(HandleReadyButton());
        });
        buttonChangeName.onClick.AddListener(() =>
        {
            StartCoroutine(HandleChangeNameButton());
        });
        buttonHostStart.onClick.AddListener(() =>
        {
            StartCoroutine(HandleHostStartButton());
        });

    }


}
