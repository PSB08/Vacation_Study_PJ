using TMPro;
using UnityEngine;

namespace Work.Code
{
    public class OperatorPanelController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private CameraPresetSwitcher camPresetController;
        
        [Header("Light")]
        [SerializeField] private LightPresetController lightPresetController;
        
        [Header("Timeline")]
        [SerializeField] private ShowTimelineController timelineController;
        
        [Header("Status UI")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI currentPresetText;
        
        private string _currentCamera = "Wide";
        private string _currentLight = "Normal";
        private string _currentStatus = "Ready";
        
        private void Start()
        {
            Apply_CameraWide();
            Apply_Normal();
        }
        
        public void Apply_CameraWide()
        {
            SetCamera(0, "Wide");
        }
        
        public void Apply_CameraCloseUp()
        {
            SetCamera(1, "CloseUp");
        }
        
        public void Apply_CameraSide()
        {
            SetCamera(2, "Side");
        }
        
        public void Apply_Normal()
        {
            if (lightPresetController != null)
            {
                lightPresetController.ApplyNormal();
            }
            
            _currentLight = "Normal";
            _currentStatus = "Ready";
            UpdateStatusText();
        }
        
        public void Apply_Live()
        {
            if (lightPresetController != null)
            {
                lightPresetController.ApplyLive();
            }
            
            _currentLight = "Live";
            _currentStatus = "Live";
            UpdateStatusText();
        }
        
        public void Apply_Emergency()
        {
            if (lightPresetController != null)
            {
                lightPresetController.ApplyEmergency();
            }
            
            _currentLight = "Emergency";
            _currentStatus = "Emergency";
            UpdateStatusText();
        }

        public void Apply_TimelineStart()
        {
            if (timelineController != null)
                timelineController.PlayFromStart();
            _currentStatus = "Timeline";
            UpdateStatusText();
        }

        public void Apply_TimelineStop()
        {
            if (timelineController != null)
            {
                timelineController.StopTimeline();
            }

            Apply_CameraWide();
            Apply_Normal();

            _currentStatus = "Ready";
            UpdateStatusText();
        }
        
        private void SetCamera(int index, string cameraName)
        {
            if (camPresetController == null)
            {
                _currentStatus = "Camera Error";
                UpdateStatusText();
                Debug.LogWarning("[Operator] Camera preset controller is missing.");
                return;
            }

            if (!camPresetController.TrySwitchTo(index))
            {
                _currentStatus = "Camera Error";
                UpdateStatusText();
                return;
            }
            
            _currentCamera = cameraName;
            UpdateStatusText();
        }
        
        private void UpdateStatusText()
        {
            if (statusText != null)
            {
                statusText.text = $"Status : {_currentStatus}";
            }
            
            if (currentPresetText != null)
            {
                currentPresetText.text = $"Camera : {_currentCamera} | Light : {_currentLight}";
            }
        }
        
    }
}
