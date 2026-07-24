using UnityEngine;
using Work.Code.Routers;

namespace Work.Code.UI
{
    public class OperatorInputController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private PlayerInputSO inputSO;
        
        [Header("Router")]
        [SerializeField] private ShowCueRouter showCueRouter;
        
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
            showCueRouter?.CueCameraWide();
        }
        
        private void HandleCameraCloseUp()
        {
            showCueRouter?.CueCameraCloseUp();
        }
        
        private void HandleCameraSide()
        {
            showCueRouter?.CueCameraSide();
        }
        
        private void HandleLightNormal()
        {
            showCueRouter?.CueLightNormal();
        }
        
        private void HandleLightLive()
        {
            showCueRouter?.CueLightLive();
        }
        
        private void HandleLightEmergency()
        {
            showCueRouter?.CueLightEmergency();
        }
        
        private void HandleShowToggle()
        {
            if (showCueRouter == null)
            {
                return;
            }
            
            if (_isShowPlaying)
            {
                showCueRouter.CueTimelineStop();
                _isShowPlaying = false;
            }
            else
            {
                showCueRouter.CueTimelineStart();
                _isShowPlaying = true;
            }
        }
        
        private void HandleFallback()
        {
            showCueRouter?.CueTimelineStop();
            _isShowPlaying = false;
        }
        
        private void HandleVfxAccent()
        {
            showCueRouter?.CueVfxAccent();
        }
        
    }
}