using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.Code
{
    public class CameraPresetSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera[] cameras;
        [SerializeField] private int activePriority = 20;
        [SerializeField] private int inactivePriority = 0;
        [SerializeField] private int defaultIndex = 0;
        
        private void Start()
        {
            SwitchTo(defaultIndex);
        }
        
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }
            
            if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
            {
                SwitchTo(0);
            }
            
            if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
            {
                SwitchTo(1);
            }
            
            if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame)
            {
                SwitchTo(2);
            }
        }
        
        public void SwitchTo(int index)
        {
            if (cameras == null || index < 0 || index >= cameras.Length)
            {
                return;
            }
            
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] == null)
                {
                    continue;
                }

                cameras[i].Priority = i == index ? activePriority : inactivePriority;
            }
        }
        
    }
}