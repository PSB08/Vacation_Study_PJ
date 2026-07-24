using System;
using UnityEngine;
using Work.Code.Managers;
using Object = UnityEngine.Object;

namespace Work.Code.Routers
{
    public class ShowCueRouter : MonoBehaviour
    { 
        [Header("Managers")]
        [SerializeField] private CameraPresetSwitcher cameraManager;
        [SerializeField] private LightPresetController lightManager;
        [SerializeField] private CharacterExpressionController expressionManager;
        [SerializeField] private StageVfxController vfxManager;
        [SerializeField] private ShowTimelineController timelineManager;
        
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
        public void CueExpressNeutral()
        {
            if (!CheckManager(expressionManager, "CharacterExpressionController"))
            {
                return;
            }
            
            expressionManager.SetNeutral();
            SetStatus("Ready");
            LogCommand("Express", "Neutral");
        }

        public void CueExpressHappy()
        {
            if (!CheckManager(expressionManager, "CharacterExpressionController"))
            {
                return;
            }
            
            expressionManager.SetHappy();
            SetStatus("Happy");
            LogCommand("Express", "Happy");
        }

        public void CueExpressSurprised()
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