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
        
        public int CurrentIndex { get; private set; } = -1;
        
        private void Start()
        {
            SwitchTo(defaultIndex);
        }
        
        public void SwitchTo(int index)
        {
            TrySwitchTo(index);
        }
        
        public bool TrySwitchTo(int index)
        {
            if (cameras == null || index < 0 || index >= cameras.Length)
            {
                Debug.LogWarning($"[CameraPresetSwitcher] Invalid camera index: {index}");
                return false;
            }
            
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] == null)
                {
                    continue;
                }
                
                cameras[i].Priority = i == index ? activePriority : inactivePriority;
            }
            
            CurrentIndex = index;
            return true;
        }
        
    }
}
