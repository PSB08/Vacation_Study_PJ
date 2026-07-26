using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Work.Code.OBS
{
    public class ObsWebSocketClient : MonoBehaviour
    { 
        [Header("Connection")]
        [SerializeField] private string host = "127.0.0.1";
        [SerializeField] private int port = 9000;
        [SerializeField] private bool useAuthentication = true;
        [SerializeField] private string password = "123456789";
        
        [Header("OBS Protocol")]
        [SerializeField] private int rpcVersion = 1;
        [SerializeField] private int eventSubscriptions = 0;
        
        [Header("Scene Names")]
        [SerializeField] private string standbySceneName = "Standby";
        [SerializeField] private string liveSceneName = "Live";
        [SerializeField] private string emergencySceneName = "Emergency";
        
        public event Action<string> OnStatusChanged;
        public event Action<string> OnLastMessageChanged;
        
        public string StatusText { get; private set; } = "Disconnected";
        public string LastMessage { get; private set; } = "-";
        
        private WebSocket _webSocket;
        private bool _identified;
        private readonly Dictionary<string, string> _pendingRequests = new Dictionary<string, string>();
        
        public bool IsConnected => _webSocket != null &&
                                   _webSocket.State == WebSocketState.Open &&
                                   _identified;
        
        public void ConnectToObs()
        {
            if (_webSocket != null && (_webSocket.State == WebSocketState.Open 
                                       || _webSocket.State == WebSocketState.Connecting))
            {
                SetLastMessage("Already connecting or connected.");
                return;
            }
            
            string url = $"ws://{host}:{port}";
            
            _identified = false;
            _pendingRequests.Clear();
            
            SetStatus("Connecting");
            SetLastMessage($"Connecting to OBS: {url}");
            
            _webSocket = new WebSocket(url, "obswebsocket.json");
            
            _webSocket.OnOpen += HandleOpen;
            _webSocket.OnMessage += HandleMessage;
            _webSocket.OnError += HandleError;
            _webSocket.OnClose += HandleClose;
            
            _ = _webSocket.Connect();
        }
        
        public async void DisconnectFromObs()
        {
            await CloseSocket();
            
            _identified = false;
            _pendingRequests.Clear();
            _webSocket = null;
            
            SetStatus("Disconnected");
            SetLastMessage("Disconnected from OBS.");
        }
        
        public async void ReconnectToObs()
        {
            SetStatus("Reconnecting");
            SetLastMessage("Reconnecting to OBS...");
            
            await CloseSocket();
            
            _identified = false;
            _pendingRequests.Clear();
            _webSocket = null;
            
            await Task.Delay(500);
            
            ConnectToObs();
        }
        
        public void SwitchSceneStandby()
        {
            SwitchScene(standbySceneName);
        }
        
        public void SwitchSceneLive()
        {
            SwitchScene(liveSceneName);
        }
        
        public void SwitchSceneEmergency()
        {
            SwitchScene(emergencySceneName);
        }
        
        public void SwitchScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                SetWarning("Scene switch failed. Scene name is empty.");
                return;
            }
            
            JObject requestData = new JObject
            {
                ["sceneName"] = sceneName
            };
            
            SendRequest("SetCurrentProgramScene", requestData, $"Switch Scene -> {sceneName}");
        }
        
        private void HandleOpen()
        {
            SetStatus("Waiting Hello");
            SetLastMessage("Socket opened. Waiting OBS Hello.");
        }
        
        private void HandleMessage(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            SetLastMessage(Shorten(json, 180));
            
            JObject packet;
            
            try
            {
                packet = JObject.Parse(json);
            }
            catch (Exception exception)
            {
                SetWarning($"Invalid OBS JSON: {exception.Message}");
                return;
            }
            
            int op = packet.Value<int?>("op") ?? -1;
            JObject data = packet["d"] as JObject ?? new JObject();
            
            switch (op)
            {
                case 0:
                    HandleHello(data);
                    break;
                case 2:
                    HandleIdentified(data);
                    break;
                case 5:
                    HandleEvent(data);
                    break;
                case 7:
                    HandleRequestResponse(data);
                    break;
                default:
                    SetLastMessage($"Unhandled OBS opcode: {op}");
                    break;
            }
        }
        
        private void HandleHello(JObject data)
        {
            SetStatus("Identifying");
            
            int serverRpcVersion = data.Value<int?>("rpcVersion") ?? rpcVersion;
            int selectedRpcVersion = Mathf.Min(rpcVersion, serverRpcVersion);
            
            JObject identifyData = new JObject
            {
                ["rpcVersion"] = selectedRpcVersion,
                ["eventSubscriptions"] = eventSubscriptions
            };
            
            JObject authentication = data["authentication"] as JObject;
            
            if (authentication != null)
            {
                if (!useAuthentication || string.IsNullOrWhiteSpace(password))
                {
                    SetWarning("OBS requires password, but password is empty.");
                    DisconnectFromObs();
                    return;
                }
                
                string salt = authentication.Value<string>("salt");
                string challenge = authentication.Value<string>("challenge");
                
                identifyData["authentication"] =
                    CreateAuthenticationString(password, salt, challenge);
            }
            
            SendIdentify(identifyData);
        }
        
        private async void SendIdentify(JObject identifyData)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                SetWarning("Cannot identify. WebSocket is not open.");
                return;
            }
            
            JObject identifyPacket = new JObject
            {
                ["op"] = 1,
                ["d"] = identifyData
            };
            
            string json = identifyPacket.ToString(Formatting.None);
            await _webSocket.SendText(json);
            
            SetLastMessage("Identify sent.");
        }
        
        private void HandleIdentified(JObject data)
        {
            _identified = true;
            SetStatus("Connected");
            
            int negotiatedRpcVersion = data.Value<int?>("negotiatedRpcVersion") ?? rpcVersion;
            SetLastMessage($"OBS identified. RPC Version: {negotiatedRpcVersion}");
        }
        
        private void HandleEvent(JObject data)
        {
            string eventType = data.Value<string>("eventType") ?? "UnknownEvent";
            SetLastMessage($"OBS Event: {eventType}");
        }
        
        private void HandleRequestResponse(JObject data)
        {
            string requestType = data.Value<string>("requestType") ?? "UnknownRequest";
            string requestId = data.Value<string>("requestId") ?? "UnknownId";
            
            JObject requestStatus = data["requestStatus"] as JObject ?? new JObject();
            
            bool result = requestStatus.Value<bool?>("result") ?? false;
            int code = requestStatus.Value<int?>("code") ?? -1;
            string comment = requestStatus.Value<string>("comment") ?? "";
            
            string requestLabel = requestType;
            
            if (_pendingRequests.TryGetValue(requestId, out string savedLabel))
            {
                requestLabel = savedLabel;
                _pendingRequests.Remove(requestId);
            }
            
            if (result)
            {
                SetStatus("Connected");
                SetLastMessage($"OBS Request Success: {requestLabel}");
                return;
            }
            
            SetWarning($"OBS Request Failed: {requestLabel} / Code: {code} / {comment}");
        }
        
        private async void SendRequest(string requestType, JObject requestData, string label)
        {
            if (!IsConnected)
            {
                SetWarning($"Cannot send request. OBS is not connected: {label}");
                return;
            }
            
            string requestId = $"{requestType}_{Guid.NewGuid():N}";
            _pendingRequests[requestId] = label;
            
            JObject requestPacket = new JObject
            {
                ["op"] = 6,
                ["d"] = new JObject
                {
                    ["requestType"] = requestType,
                    ["requestId"] = requestId,
                    ["requestData"] = requestData ?? new JObject()
                }
            };
            
            string json = requestPacket.ToString(Formatting.None);
            
            try
            {
                await _webSocket.SendText(json);
                SetLastMessage($"OBS Request Sent: {label}");
            }
            catch (Exception exception)
            {
                _pendingRequests.Remove(requestId);
                SetWarning($"OBS Request Send Failed: {label} / {exception.Message}");
            }
        }
        
        private void HandleError(string errorMessage)
        {
            _identified = false;
            SetStatus("Failed");
            SetWarning($"OBS Error: {errorMessage}");
        }
        
        private void HandleClose(WebSocketCloseCode closeCode)
        {
            _identified = false;
            SetStatus("Disconnected");
            SetLastMessage($"OBS closed: {closeCode}");
        }
        
        private async Task CloseSocket()
        {
            if (_webSocket == null)
            {
                return;
            }
            
            if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.Connecting)
            {
                try
                {
                    await _webSocket.Close();
                }
                catch (Exception exception)
                {
                    SetWarning($"OBS close failed: {exception.Message}");
                }
            }
        }
        
        private static string CreateAuthenticationString(string password, string salt, string challenge)
        {
            string secret = Sha256Base64(password + salt);
            return Sha256Base64(secret + challenge);
        }
        
        private static string Sha256Base64(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        
        private void SetStatus(string status)
        {
            StatusText = status;
            OnStatusChanged?.Invoke(status);
            Debug.Log($"[OBS] Status: {status}");
        }
        
        private void SetLastMessage(string message)
        {
            LastMessage = message;
            OnLastMessageChanged?.Invoke(message);
            Debug.Log($"[OBS] {message}");
        }
        
        private void SetWarning(string message)
        {
            LastMessage = message;
            OnLastMessageChanged?.Invoke(message);
            Debug.LogWarning($"[OBS] {message}");
        }
        
        private static string Shorten(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }
            return value.Substring(0, maxLength) + "...";
        }
        
        private async void OnDestroy()
        {
            await CloseSocket();
        }
        
    }
}