using UnityEngine;

namespace Work.Code
{
    public class OperatorInputController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private PlayerInputSO inputSO;
        
        [Header("Controllers")]
        [SerializeField] private OperatorPanelController operatorPanelController;
        [SerializeField] private StageVfxController stageVfxController;
        
        private bool _isShowPlaying;
        
        private void OnEnable()
        {
            if (inputSO == null)
            {
                Debug.LogWarning("[OperatorInputController] PlayerInputSO is missing.");
                return;
            }
            
            inputSO.OnCameraWideClick += HandleCameraWide;
            inputSO.OnCameraCloseUpClick += HandleCameraCloseUp;
            inputSO.OnCameraSideClick += HandleCameraSide;
            
            inputSO.OnLightNormalClick += HandleLightNormal;
            inputSO.OnLightLiveClick += HandleLightLive;
            inputSO.OnLightEmergencyClick += HandleLightEmergency;
            
            inputSO.OnShowToggleClick += HandleShowToggle;
            inputSO.OnFallbackClick += HandleFallback;
            inputSO.OnVfxAccentClick += HandleVfxAccent;
        }
        
        private void OnDisable()
        {
            if (inputSO == null)
            {
                return;
            }
            
            inputSO.OnCameraWideClick -= HandleCameraWide;
            inputSO.OnCameraCloseUpClick -= HandleCameraCloseUp;
            inputSO.OnCameraSideClick -= HandleCameraSide;
            
            inputSO.OnLightNormalClick -= HandleLightNormal;
            inputSO.OnLightLiveClick -= HandleLightLive;
            inputSO.OnLightEmergencyClick -= HandleLightEmergency;
            
            inputSO.OnShowToggleClick -= HandleShowToggle;
            inputSO.OnFallbackClick -= HandleFallback;
            inputSO.OnVfxAccentClick -= HandleVfxAccent;
        }
        
        private void HandleCameraWide()
        {
            if (operatorPanelController == null)
            {
                return;
            }
            
            operatorPanelController.Apply_CameraWide();
        }
        
        private void HandleCameraCloseUp()
        {
            if (operatorPanelController == null)
            {
                return;
            }
            
            operatorPanelController.Apply_CameraCloseUp();
        }
        
        private void HandleCameraSide()
        {
            if (operatorPanelController == null)
            {
                return;
            }
            
            operatorPanelController.Apply_CameraSide();
        }
        
        private void HandleLightNormal()
        {
            if (operatorPanelController == null)
            {
                return;
            }
            
            operatorPanelController.Apply_Normal();
        }
        
        private void HandleLightLive()
        {
            if (operatorPanelController == null)
            {
                return;
            }
            
            operatorPanelController.Apply_Live();
        }
        
        private void HandleLightEmergency()
        {
            if (operatorPanelController == null)
            {
                return;
            }
            
            operatorPanelController.Apply_Emergency();
        }
        
        private void HandleShowToggle()
        {
            if (operatorPanelController == null)
            {
                return;
            }
            
            if (_isShowPlaying)
            {
                operatorPanelController.Apply_TimelineStop();
                _isShowPlaying = false;
            }
            else
            {
                operatorPanelController.Apply_TimelineStart();
                _isShowPlaying = true;
            }
        }
        
        private void HandleFallback()
        {
            if (operatorPanelController != null)
            {
                operatorPanelController.Apply_TimelineStop();
                operatorPanelController.Apply_CameraWide();
                operatorPanelController.Apply_Normal();
            }
            
            if (stageVfxController != null)
            {
                stageVfxController.StopAll();
            }
            
            _isShowPlaying = false;
        }
        
        private void HandleVfxAccent()
        {
            if (stageVfxController == null)
            {
                return;
            }
            
            stageVfxController.PlayAccent();
        }
        
    }
}