using System.Collections.Generic;
using System;
using OscJack;
using UnityEngine;
using Work.Code.Routers;

namespace Work.Code.OSC
{
    public class OscReceiverController : MonoBehaviour
    {
        [Header("OSC")]
        [SerializeField] private int port = 9000;
        [SerializeField] private float activeSignalSeconds = 3f;
        
        [Header("Router")]
        [SerializeField] private ShowCueRouter showCueRouter;

        public event Action<string> OnReceiveStatusChanged;
        public event Action<string> OnSignalStatusChanged;
        
        private OscServer _server;
        private readonly Queue<string> _receivedCommands = new Queue<string>();
        private readonly object _lock = new object();
        private float _lastReceivedTime = -1f;
        private string _receiveStatus = "Stopped";
        private string _signalStatus = "Idle";

        public string ReceiveStatus => _receiveStatus;
        public string SignalStatus => _signalStatus;
        
        private void OnEnable()
        {
            try
            {
                _server = new OscServer(port);
                _server.MessageDispatcher.AddCallback(string.Empty, OnOscReceived);
                _lastReceivedTime = -1f;
                SetReceiveStatus($"Listening : {port}");
                SetSignalStatus("Idle");
                Debug.Log($"[OSC] Receiver started. Port : {port}");
            }
            catch (Exception exception)
            {
                _server = null;
                SetReceiveStatus($"Port Error : {port}");
                SetSignalStatus("Error");
                Debug.LogWarning($"[OSC] Receiver failed. Port: {port}, Error: {exception.Message}");
            }
        }
        
        private void OnDisable()
        {
            if (_server == null)
            {
                SetReceiveStatus("Stopped");
                SetSignalStatus("Idle");
                return;
            }
            
            _server.Dispose();
            _server = null;
            
            SetReceiveStatus("Stopped");
            SetSignalStatus("Idle");
            Debug.Log("[OSC] Receiver stopped.");
        }
        
        private void Update()
        {
            while (TryDequeueCommand(out string address))
            {
                _lastReceivedTime = Time.realtimeSinceStartup;
                SetSignalStatus("Active");
                ExecuteCommand(address);
            }

            UpdateSignalStatus();
        }
        
        private void OnOscReceived(string address, OscDataHandle data)
        {
            lock (_lock)
            {
                _receivedCommands.Enqueue(address);
            }
        }
        
        private bool TryDequeueCommand(out string address)
        {
            lock (_lock)
            {
                if (_receivedCommands.Count > 0)
                {
                    address = _receivedCommands.Dequeue();
                    return true;
                }
            }
            
            address = null;
            return false;
        }
        
        private void ExecuteCommand(string address)
        {
            Debug.Log($"[OSC] Received: {address}");
            
            if (showCueRouter == null)
            {
                Debug.LogWarning("[OSC] ShowCueRouter is missing.");
                return;
            }
            
            switch (address)
            {
                case "/show/start":
                    showCueRouter.CueTimelineStart();
                    break;
                case "/show/stop":
                    showCueRouter.CueTimelineStop();
                    break;
                    
                case "/camera/wide":
                    showCueRouter.CueCameraWide();
                    break;
                case "/camera/closeup":
                    showCueRouter.CueCameraCloseUp();
                    break;
                case "/camera/side":
                    showCueRouter.CueCameraSide();
                    break;
                
                case "/anim/idle":
                    showCueRouter.CueAnimIdle();
                    break;
                case "/anim/walk":
                    showCueRouter.CueAnimWalk();
                    break;
                
                case "/light/normal":
                    showCueRouter.CueLightNormal();
                    break;
                case "/light/live":
                    showCueRouter.CueLightLive();
                    break;
                case "/light/emergency":
                    showCueRouter.CueLightEmergency();
                    break;
                
                case "/vfx/entrance":
                    showCueRouter.CueVfxEntrance();
                    break;
                case "/vfx/accent":
                    showCueRouter.CueVfxAccent();
                    break;
                case "/fallback":
                    showCueRouter.CueFallback();
                    break;
                
                default:
                    Debug.LogWarning($"[OSC] Unknown address: {address}");
                    break;
            }
        }

        private void UpdateSignalStatus()
        {
            if (_server == null)
            {
                SetSignalStatus("Idle");
                return;
            }

            if (_lastReceivedTime < 0f)
            {
                SetSignalStatus("Idle");
                return;
            }

            float elapsed = Time.realtimeSinceStartup - _lastReceivedTime;
            SetSignalStatus(elapsed <= activeSignalSeconds ? "Active" : "No Signal");
        }

        private void SetReceiveStatus(string status)
        {
            if (_receiveStatus == status)
            {
                return;
            }

            _receiveStatus = status;
            OnReceiveStatusChanged?.Invoke(status);
        }

        private void SetSignalStatus(string status)
        {
            if (_signalStatus == status)
            {
                return;
            }

            _signalStatus = status;
            OnSignalStatusChanged?.Invoke(status);
        }
        
    }
}
