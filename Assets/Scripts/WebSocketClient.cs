using System;
using System.Text;
using System.Collections;
using TMPro;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.Scripting;

[Preserve]
public class WebSocketClient : MonoBehaviour
{
    public string url = "localhost:8080";
    public string userId = Guid.NewGuid().ToString();

    private static WebSocketClient _instance;
    private NativeWebSocket.WebSocket _webSocket;

    public event Action<string> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string> OnError;

    public static WebSocketClient Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("WebSocketClient");
                _instance = go.AddComponent<WebSocketClient>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void Connect()
    {
        StartCoroutine(ConnectCoroutine());
    }

    private IEnumerator ConnectCoroutine()
    {
        string fullUrl = "ws://" + url + "/api/ws/" + userId;
        _webSocket = new WebSocket(fullUrl);

        _webSocket.OnOpen += () =>
        {
            Debug.Log("WebSocket connected: " + fullUrl);
            OnConnected?.Invoke();
        };

        _webSocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            OnMessageReceived?.Invoke(message);
            Debug.Log("Received message: " + message);
        };

        _webSocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket error: " + e);
            OnError?.Invoke(e);
        };

        _webSocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed");
            OnDisconnected?.Invoke();
        };

        // 开始连接
        var connectTask = _webSocket.Connect();
        
        // 等待连接完成
        yield return new WaitUntil(() => connectTask.IsCompleted);
        
        if (connectTask.IsFaulted && connectTask.Exception != null)
        {
            Debug.LogError("Connection failed: " + connectTask.Exception.Message);
            OnError?.Invoke(connectTask.Exception.Message);
        }
    }

    private void Update()
    {
#if !(UNITY_WEBGL && !UNITY_EDITOR)
        if (_webSocket != null)
        {
            _webSocket.DispatchMessageQueue();
        }
#endif
    }

    public void SendMessage<T>(Message<T> message)
    {
        StartCoroutine(SendMessageCoroutine(message));
    }

    public IEnumerator SendMessageCoroutine<T>(Message<T> message)
    {
        string json = null;
        
        // 序列化部分放在 try 块外
        try
        {
            json = JsonUtility.ToJson(message);
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON serialization failed: " + ex.Message);
            OnError?.Invoke(ex.Message);
            yield break; // 直接退出协程
        }
        
        // 发送部分使用单独的协程
        yield return StartCoroutine(SendCoroutine(json));
    }

    public IEnumerator SendCoroutine(string message)
    {
        if (_webSocket?.State == WebSocketState.Open)
        {
            var sendTask = _webSocket.SendText(message);
            
            // 等待发送完成
            yield return new WaitUntil(() => sendTask.IsCompleted);
            
            if (sendTask.IsFaulted && sendTask.Exception != null)
            {
                Debug.LogError("Send failed: " + sendTask.Exception.Message);
                OnError?.Invoke(sendTask.Exception.Message);
            }
            else
            {
                Debug.Log("Sent message: " + message);
            }
        }
        else
        {
            Debug.LogWarning("WebSocket not connected");
            OnError?.Invoke("WebSocket not connected");
        }
    }

    public void Close()
    {
        StartCoroutine(CloseCoroutine());
    }

    private IEnumerator CloseCoroutine()
    {
        if (_webSocket != null)
        {
            var closeTask = _webSocket.Close();
            
            // 等待关闭完成
            yield return new WaitUntil(() => closeTask.IsCompleted);
            
            _webSocket = null;
            OnDisconnected?.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        Close();
    }

    private void OnDestroy()
    {
        Close();
    }
}