using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Work.Code.Routers;

namespace Work.Code.UI
{
    public class OperatorPanelController : MonoBehaviour
    {
        [Header("Router")]
        [SerializeField] private ShowCueRouter showCueRouter;
        
        [Header("Status UI")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI currentPresetText;
        
        [SerializeField] private TextMeshProUGUI lastCommandText;
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private TextMeshProUGUI warningText;
        
        private readonly Queue<string> _logs = new Queue<string>();
        private const int MaxLogCount = 10;

        private void Awake()
        {
            statusText.text = string.Empty;
            currentPresetText.text = string.Empty;
            lastCommandText.text = string.Empty;
            logText.text = string.Empty;
            warningText.text = string.Empty;
        }

        private void OnEnable()
        {
            if (showCueRouter == null)
            {
                return;
            }
            
            showCueRouter.OnStatusChanged += UpdateStatusText;
            showCueRouter.OnPresetChanged += UpdatePresetText;
            showCueRouter.OnLastCommandChanged += UpdateLastCommandText;
            showCueRouter.OnLogAdded += AddLogText;
            showCueRouter.OnWarningLog += UpdateWarningText;
        }

        private void OnDisable()
        {
            if (showCueRouter == null)
            {
                return;
            }
            
            showCueRouter.OnStatusChanged -= UpdateStatusText;
            showCueRouter.OnPresetChanged -= UpdatePresetText;
            showCueRouter.OnLastCommandChanged -= UpdateLastCommandText;
            showCueRouter.OnLogAdded -= AddLogText;
            showCueRouter.OnWarningLog -= UpdateWarningText;
        }
        
        #region Camera
        public void Apply_CameraWide()
        {
            showCueRouter?.CueCameraWide();
        }
        
        public void Apply_CameraCloseUp()
        {
            showCueRouter?.CueCameraCloseUp();
        }
        
        public void Apply_CameraSide()
        {
            showCueRouter?.CueCameraSide();
        }
        #endregion
        
        #region Light
        public void Apply_LightNormal()
        {
            showCueRouter?.CueLightNormal();
        }
        
        public void Apply_LightLive()
        {
            showCueRouter?.CueLightLive();
        }
        
        public void Apply_LightEmergency()
        {
            showCueRouter?.CueLightEmergency();
        }
        #endregion
        
        #region Express

        public void Apply_ExpressNeutral()
        {
            showCueRouter?.CueExpressNeutral();
        }

        public void Apply_ExpressHappy()
        {
            showCueRouter?.CueExpressHappy();
        }

        public void Apply_ExpressSurprised()
        {
            showCueRouter?.CueExpressSurprised();
        }

        public void Apply_Fallback()
        {
            showCueRouter?.CueFallback();
        }
        
        #endregion
        
        #region Vfx
        public void Apply_VfxEntrance()
        {
            showCueRouter?.CueVfxEntrance();
        }
        
        public void Apply_VfxAccent()
        {
            showCueRouter?.CueVfxAccent();
        }
        #endregion
        
        #region Timeline
        public void Apply_TimelineStart()
        {
            showCueRouter?.CueTimelineStart();
        }
        
        public void Apply_TimelineStop()
        {
            showCueRouter?.CueTimelineStop();
        }
        #endregion
        
        private void UpdateStatusText(string status)
        {
            if (statusText != null)
            {
                statusText.text = $"Status : {status}";
            }
        }
        
        private void UpdatePresetText(string preset)
        {
            if (currentPresetText != null)
            {
                currentPresetText.text = preset;
            }
        }
        
        private void UpdateLastCommandText(string command)
        {
            if (lastCommandText != null)
            {
                lastCommandText.text = $"Last Command : {command}";
            }
        }

        private void AddLogText(string log)
        {
            _logs.Enqueue(log);

            while (_logs.Count > MaxLogCount)
            {
                _logs.Dequeue();
            }

            if (logText != null)
            {
                logText.text = string.Join("\n", _logs);
            }
        }

        private void UpdateWarningText(string log)
        {
            StartCoroutine(UpdateWarningTextCoroutine(log));
        }

        private IEnumerator UpdateWarningTextCoroutine(string log)
        {
            if (warningText != null)
            {
                warningText.text = log;
            }

            yield return new WaitForSeconds(1f);
            
            warningText.text = string.Empty;
        }
        
    }
}
