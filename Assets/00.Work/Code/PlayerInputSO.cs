using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.Code
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/Player/PlayerInput", order = 10)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action OnCameraWideClick;
        public event Action OnCameraCloseUpClick;
        public event Action OnCameraSideClick;
        
        public event Action OnLightNormalClick;
        public event Action OnLightLiveClick;
        public event Action OnLightEmergencyClick;
        
        public event Action OnShowToggleClick;
        public event Action OnFallbackClick;
        public event Action OnVfxAccentClick;
        
        private Controls _controls;
        
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }
        
        private void OnDisable()
        {
            _controls.Player.Disable();
        }
        
        public void OnCameraWide(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCameraWideClick?.Invoke();
        }
        
        public void OnCameraCloseUp(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCameraCloseUpClick?.Invoke();
        }
        
        public void OnCameraSide(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCameraSideClick?.Invoke();
        }
        
        public void OnLightNormal(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnLightNormalClick?.Invoke();
        }
        
        public void OnLightLive(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnLightLiveClick?.Invoke();
        }
        
        public void OnLightEmergency(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnLightEmergencyClick?.Invoke();
        }
        
        public void OnShowToggle(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnShowToggleClick?.Invoke();
        }
        
        public void OnFallback(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnFallbackClick?.Invoke();
        }
        
        public void OnVfxAccent(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnVfxAccentClick?.Invoke();
        }
        
    }
}