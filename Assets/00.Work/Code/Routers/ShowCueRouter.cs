using System;
using UnityEngine;
using Work.Code.Managers;
using Work.Code.OBS;
using Object = UnityEngine.Object;

namespace Work.Code.Routers
{
    public class ShowCueRouter : MonoBehaviour
    { 
        [Header("Managers")]
        [SerializeField] private CameraPresetSwitcher cameraManager;
        [SerializeField] private LightPresetController lightManager;
        [SerializeField] private AnimPlayController animManager;
        [SerializeField] private CharacterExpressionController expressionManager;
        [SerializeField] private StageVfxController vfxManager;
        [SerializeField] private ShowTimelineController timelineManager;
        [SerializeField] private ObsWebSocketClient obsWebSocketClient;
        
        public event Action<string> OnStatusChanged;
        public event Action<string> OnPresetChanged;
        public event Action<string> OnLastCommandChanged;
        public event Action<string> OnLogAdded;
        public event Action<string> OnWarningLog;
        
        private string _currentCamera = "Wide";
        private string _currentLight = "Normal";

        private void Start()
        {
            CueCameraWide();
            CueLightNormal();
        }
        
        #region Camera
        public void CueCameraWide()
        {
            if (!CheckManager(cameraManager, "CameraPresetSwitcher"))
            {
                return;
            }
            
            cameraManager.SwitchTo(0);
            _currentCamera = "Wide";
            UpdatePreset();
            LogCommand("Camera", "Wide");
        }
        
        public void CueCameraCloseUp()
        {
            if (!CheckManager(cameraManager, "CameraPresetSwitcher"))
            {
                return;
            }
            
            cameraManager.SwitchTo(1);
            _currentCamera = "CloseUp";
            UpdatePreset();
            LogCommand("Camera", "CloseUp");
        }
        
        public void CueCameraSide()
        {
            if (!CheckManager(cameraManager, "CameraPresetSwitcher"))
            {
                return;
            }
            
            cameraManager.SwitchTo(2);
            _currentCamera = "Side";
            UpdatePreset();
            LogCommand("Camera", "Side");
        }
        #endregion
        
        #region Light
        public void CueLightNormal()
        {
            if (!CheckManager(lightManager, "LightPresetController"))
            {
                return;
            }
            
            lightManager.ApplyNormal();
            _currentLight = "Normal";
            UpdatePreset();
            SetStatus("Ready");
            LogCommand("Light", "Ready");
        }
        
        public void CueLightLive()
        {
            if (!CheckManager(lightManager, "LightPresetController"))
            {
                return;
            }
            
            lightManager.ApplyLive();
            _currentLight = "Live";
            UpdatePreset();
            SetStatus("Live");
            LogCommand("Light", "Live");
        }
        
        public void CueLightEmergency()
        {
            if (!CheckManager(lightManager, "LightPresetController"))
            {
                return;
            }
            
            lightManager.ApplyEmergency();
            _currentLight = "Emergency";
            UpdatePreset();
            SetStatus("Emergency");
            LogCommand("Light", "Emergency");
        }
        #endregion
        
        #region Express

        public void CueAnimIdle()
        {
            if (!CheckManager(animManager, "AnimPlayController"))
            {
                return;
            }
            
            animManager.Anim_Idle();
            SetStatus("Idle");
            LogCommand("Anim", "Idle");
        }
        
        public void CueAnimWalk()
        {
            if (!CheckManager(animManager, "AnimPlayController"))
            {
                return;
            }
            
            animManager.Anim_Walk();
            SetStatus("Walk");
            LogCommand("Anim", "Walk");
        }
        
        public void CueAnimDance(int index)
        {
            if (!CheckManager(animManager, "AnimPlayController"))
            {
                return;
            }
            
            animManager.Anim_Dance(index);
            SetStatus($"Dance{index}");
            LogCommand("Anim", $"Dance{index}");
        }
        
        public void CueExpressionNeutral()
        {
            if (!CheckManager(expressionManager, "CharacterExpressionController"))
            {
                return;
            }
            
            expressionManager.SetNeutral();
            SetStatus("Ready");
            LogCommand("Express", "Neutral");
        }

        public void CueExpressionHappy()
        {
            if (!CheckManager(expressionManager, "CharacterExpressionController"))
            {
                return;
            }
            
            expressionManager.SetHappy();
            SetStatus("Happy");
            LogCommand("Express", "Happy");
        }

        public void CueExpressionSurprised()
        {
            if (!CheckManager(expressionManager, "CharacterExpressionController"))
            {
                return;
            }
            
            expressionManager.SetSurprised();
            SetStatus("Surprised");
            LogCommand("Express", "Surprised");
        }

        public void CueFallback()
        {
            if (!CheckManager(expressionManager, "CharacterExpressionController"))
            {
                return;
            }
            
            expressionManager.FallbackToIdle();
            SetStatus("Ready");
            LogCommand("All", "Fallback");
        }
        #endregion
        
        #region Vfx
        public void CueVfxEntrance()
        {
            if (!CheckManager(vfxManager, "StageVfxController"))
            {
                return;
            }
            
            vfxManager.PlayEntrance();
            SetStatus("Vfx Entrance");
            LogCommand("Vfx", "Entrance");
        }
        
        public void CueVfxAccent()
        {
            if (!CheckManager(vfxManager, "StageVfxController"))
            {
                return;
            }
            
            vfxManager.PlayAccent();
            SetStatus("Vfx Accent");
            LogCommand("Vfx", "Accent");
        }
        #endregion
        
        #region Timeline
        public void CueTimelineStart()
        {
            if (!CheckManager(timelineManager, "ShowTimelineController"))
            {
                return;
            }
            
            timelineManager.PlayFromStart();
            SetStatus("Timeline Playing");
            LogCommand("Timeline", "Play");
        }
        
        public void CueTimelineStop()
        {
            if (!CheckManager(timelineManager, "ShowTimelineController"))
            {
                return;
            }
            timelineManager.StopTimeline();
            
            if (!CheckManager(vfxManager, "StageVfxController"))
            {
                return;
            }
            vfxManager.StopAll();

            CueCameraWide();
            CueLightNormal();

            SetStatus("Ready");
            LogCommand("Timeline", "Stop");
        }
        #endregion
        
        #region OBS
        public void CueObsConnect()
        {
            if (!CheckManager(obsWebSocketClient, "ObsWebSocketClient"))
            {
                return;
            }
            
            obsWebSocketClient.ConnectToObs();
            LogCommand("OBS", "Connect");
        }
        
        public void CueObsDisconnect()
        {
            if (!CheckManager(obsWebSocketClient, "ObsWebSocketClient"))
            {
                return;
            }
            
            obsWebSocketClient.DisconnectFromObs();
            LogCommand("OBS", "Disconnect");
        }
        
        public void CueObsReconnect()
        {
            if (!CheckManager(obsWebSocketClient, "ObsWebSocketClient"))
            {
                return;
            }
            
            obsWebSocketClient.ReconnectToObs();
            LogCommand("OBS", "Reconnect");
        }
        
        public void CueObsSceneStandby()
        {
            if (!CheckManager(obsWebSocketClient, "ObsWebSocketClient"))
            {
                return;
            }
            
            obsWebSocketClient.SwitchSceneStandby();
            LogCommand("OBS Scene", "Standby");
        }
        
        public void CueObsSceneLive()
        {
            if (!CheckManager(obsWebSocketClient, "ObsWebSocketClient"))
            {
                return;
            }
            
            obsWebSocketClient.SwitchSceneLive();
            LogCommand("OBS Scene", "Live");
        }
        
        public void CueObsSceneEmergency()
        {
            if (!CheckManager(obsWebSocketClient, "ObsWebSocketClient"))
            {
                return;
            }
            
            obsWebSocketClient.SwitchSceneEmergency();
            LogCommand("OBS Scene", "Emergency");
        }
        #endregion
        
        private void SetStatus(string status)
        {
            OnStatusChanged?.Invoke(status);
        }
        
        private void UpdatePreset()
        {
            OnPresetChanged?.Invoke($"Camera : {_currentCamera}\nLight : {_currentLight}");
        }
        
        private void LogCommand(string category, string message)
        {
            string command = $"{category}: {message}";
            OnLastCommandChanged?.Invoke(command);
            OnLogAdded?.Invoke($"[{DateTime.Now:HH:mm:ss}] {command}");
            Debug.Log($"[CueRouter] {command}");
        }
        
        private bool CheckManager(Object manager, string managerName)
        {
            if (manager != null)
            {
                return true;
            }
            
            string message = $"{managerName} is missing.";
            OnWarningLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] Warning: {message}");
            OnStatusChanged?.Invoke("Warning");
            return false;
        }
        
    }
}