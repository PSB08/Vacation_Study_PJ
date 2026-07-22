using UnityEngine;
using UnityEngine.Playables;

namespace Work.Code
{
    public class ShowTimelineController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        
        public void PlayFromStart()
        {
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