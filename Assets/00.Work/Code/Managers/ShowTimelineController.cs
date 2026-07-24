using UnityEngine;
using UnityEngine.Playables;

namespace Work.Code.Managers
{
    public class ShowTimelineController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        
        public void PlayFromStart()
        {
            if (director == null)
            {
                return;
            }
            
            director.time = 0;
            director.Play();
        }
        
        public void StopTimeline()
        {
            if (director == null)
            {
                return;
            }
            
            director.Pause();
            director.time = 30;
            director.Evaluate();
            director.Stop();
        }
        
    }
}