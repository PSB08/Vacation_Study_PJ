using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Work.Code.Managers
{
    public class QualityModeController : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] private Volume globalVolume;
        
        [Header("VFX")]
        [SerializeField] private ParticleSystem[] performanceParticles;
        
        [Header("Lights")]
        [SerializeField] private Light[] shadowLights;
        [SerializeField] private Light[] disabledInLowQualityLights;
        
        [Header("Renderer Features")]
        [SerializeField] private ScriptableRendererData rendererData;
        [SerializeField] private string ssaoFeatureName = "ScreenSpaceAmbientOcclusion";
        [SerializeField] private bool normalSsaoEnabled = true;
        [SerializeField] private bool lowSsaoEnabled = false;
        [SerializeField] private bool emergencySsaoEnabled = false;
        
        [Header("Normal Quality")]
        [SerializeField] private float normalRenderScale = 1.0f;
        [SerializeField] private float normalShadowDistance = 50f;
        [SerializeField] private bool normalBloomEnabled = true;
        [SerializeField] private float normalBloomIntensity = 0.8f;
        [SerializeField] private bool normalParticlesEnabled = true;
        [SerializeField] private bool normalOptionalLightsEnabled = true;
        
        [SerializeField] private float normalBloomThreshold = 1.1f;
        [SerializeField] private float normalBloomScatter = 0.55f;
        [SerializeField] private bool normalBloomHighQualityFiltering = false;
        
        [Header("Low Quality")]
        [SerializeField] private float lowRenderScale = 0.8f;
        [SerializeField] private float lowShadowDistance = 25f;
        [SerializeField] private bool lowBloomEnabled = true;
        [SerializeField] private float lowBloomIntensity = 0.2f;
        [SerializeField] private bool lowParticlesEnabled = false;
        [SerializeField] private bool lowOptionalLightsEnabled = false;
        
        [SerializeField] private float lowBloomThreshold = 1.4f;
        [SerializeField] private float lowBloomScatter = 0.35f;
        [SerializeField] private bool lowBloomHighQualityFiltering = false;
        
        [Header("Emergency Quality")]
        [SerializeField] private float emergencyRenderScale = 0.75f;
        [SerializeField] private float emergencyShadowDistance = 15f;
        [SerializeField] private bool emergencyBloomEnabled = false;
        [SerializeField] private float emergencyBloomIntensity = 0f;
        [SerializeField] private bool emergencyParticlesEnabled = false;
        [SerializeField] private bool emergencyOptionalLightsEnabled = false;
        
        [SerializeField] private float emergencyBloomThreshold = 1.6f;
        [SerializeField] private float emergencyBloomScatter = 0.2f;
        [SerializeField] private bool emergencyBloomHighQualityFiltering = false;
        
        private UniversalRenderPipelineAsset _urpAsset;
        private Bloom _bloom;
        
        public bool IsLowQuality { get; private set; }
        
        private void Awake()
        {
            _urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            
            if (globalVolume != null && globalVolume.profile != null)
            {
                globalVolume.profile.TryGet(out _bloom);
            }
        }
        
        public void ApplyNormalQuality()
        {
            IsLowQuality = false;
            
            ApplyRenderSettings(normalRenderScale, normalShadowDistance);
            ApplyBloom(normalBloomEnabled, normalBloomIntensity, 
                normalBloomThreshold, normalBloomScatter, normalBloomHighQualityFiltering);
            
            SetParticlesEnabled(normalParticlesEnabled);
            SetShadowLightsEnabled(true);
            SetOptionalLightsEnabled(normalOptionalLightsEnabled);
            SetRendererFeatureActive(ssaoFeatureName, normalSsaoEnabled);
            
            Debug.Log("[QualityMode] Normal Quality Applied");
        }
        
        public void ApplyLowQuality()
        {
            IsLowQuality = true;
            
            ApplyRenderSettings(lowRenderScale, lowShadowDistance);
            ApplyBloom(lowBloomEnabled, lowBloomIntensity,
                lowBloomThreshold, lowBloomScatter, lowBloomHighQualityFiltering);
            
            SetParticlesEnabled(lowParticlesEnabled);
            SetShadowLightsEnabled(false);
            SetOptionalLightsEnabled(lowOptionalLightsEnabled);
            SetRendererFeatureActive(ssaoFeatureName, lowSsaoEnabled);
            
            Debug.Log("[QualityMode] Low Quality Applied");
        }
        
        public void ApplyEmergencyQuality()
        {
            IsLowQuality = true;
            
            ApplyRenderSettings(emergencyRenderScale, emergencyShadowDistance);
            ApplyBloom(emergencyBloomEnabled, emergencyBloomIntensity, 
                emergencyBloomThreshold, emergencyBloomScatter, emergencyBloomHighQualityFiltering);
            
            SetParticlesEnabled(emergencyParticlesEnabled);
            SetShadowLightsEnabled(false);
            SetOptionalLightsEnabled(emergencyOptionalLightsEnabled);
            SetRendererFeatureActive(ssaoFeatureName, emergencySsaoEnabled);
            
            Debug.Log("[QualityMode] Emergency Quality Applied");
        }
        
        private void ApplyRenderSettings(float renderScale, float shadowDistance)
        {
            if (_urpAsset == null)
            {
                Debug.LogWarning("[QualityMode] URP Asset을 찾을 수 없습니다.");
                return;
            }
            
            _urpAsset.renderScale = renderScale;
            _urpAsset.shadowDistance = shadowDistance;
        }
        
        private void ApplyBloom(bool enabled, float intensity,
            float threshold, float scatter, bool highQualityFiltering)
        {
            if (_bloom == null)
            {
                Debug.LogWarning("[QualityMode] Bloom 설정을 찾을 수 없습니다.");
                return;
            }
            
            _bloom.active = enabled;
            _bloom.intensity.Override(intensity);
            _bloom.threshold.Override(threshold);
            _bloom.scatter.Override(scatter);
            _bloom.highQualityFiltering.Override(highQualityFiltering);
        }
        
        private void SetParticlesEnabled(bool enabled)
        {
            foreach (ParticleSystem particle in performanceParticles)
            {
                if (particle == null)
                {
                    continue;
                }
                
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = enabled;
                
                if (enabled)
                {
                    particle.Play(true);
                }
                else
                {
                    particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }
        
        private void SetShadowLightsEnabled(bool enabled)
        {
            foreach (Light light in shadowLights)
            {
                if (light == null)
                {
                    continue;
                }
                
                light.shadows = enabled ? LightShadows.Soft : LightShadows.None;
            }
        }
        
        private void SetOptionalLightsEnabled(bool enabled)
        {
            foreach (Light light in disabledInLowQualityLights)
            {
                if (light == null)
                {
                    continue;
                }
                
                light.enabled = enabled;
            }
        }
        
        private void SetRendererFeatureActive(string featureName, bool enabled)
        {
            if (rendererData == null)
            {
                Debug.LogWarning("[QualityMode] Renderer Data가 연결되지 않았습니다.");
                return;
            }
            
            foreach (ScriptableRendererFeature feature in rendererData.rendererFeatures)
            {
                if (feature == null)
                {
                    continue;
                }
                
                if (feature.name.Contains(featureName))
                {
                    feature.SetActive(enabled);
                    return;
                }
            }
            
            Debug.LogWarning($"[QualityMode] Renderer Feature를 찾을 수 없습니다: {featureName}");
        }
        
    }
}