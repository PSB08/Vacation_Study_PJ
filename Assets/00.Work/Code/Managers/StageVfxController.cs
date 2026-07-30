using UnityEngine;

namespace Work.Code.Managers
{
    public class StageVfxController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem entranceEffect;
        [SerializeField] private ParticleSystem accentEffect;
        
        public void PlayEntrance()
        {
            PlayEffect(entranceEffect, "Entrance");
        }
        
        public void PlayAccent()
        {
            PlayEffect(accentEffect, "Accent");
        }
        
        public void StopAll()
        {
            StopEffect(entranceEffect);
            StopEffect(accentEffect);
        }
        
        private void PlayEffect(ParticleSystem effect, string label)
        {
            if (effect == null)
            {
                Debug.LogWarning($"[VFX] {label} effect is missing.");
                return;
            }
            
            ParticleSystem.EmissionModule emission = effect.emission;
            emission.enabled = true;

            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.Clear(true);
            effect.Play(true);
        }
        
        private void StopEffect(ParticleSystem effect)
        {
            if (effect == null)
            {
                return;
            }
            
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        
    }
}