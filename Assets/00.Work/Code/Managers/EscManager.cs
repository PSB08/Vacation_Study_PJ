using UnityEngine;

namespace Work.Code.Managers
{
    public class EscManager : MonoBehaviour
    {
        public void Exit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
    }
}