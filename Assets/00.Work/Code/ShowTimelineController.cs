using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Work.Code
{
    public class ShowTimelineController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || director == null)
            {
                return;
            }

            if (keyboard.pKey.wasPressedThisFrame)
            {
                PlayFromStart();
            }

            if (keyboard.sKey.wasPressedThisFrame)
            {
                director.Stop();
            }
        }

        [ContextMenu("Play From Start")]
        public void PlayFromStart()
        {
            director.time = 0;
            director.Play();
        }
        
    }
}