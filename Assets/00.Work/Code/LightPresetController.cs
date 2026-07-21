using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.Code
{
    public enum LightPreset
    {
        Normal,
        Live,
        Emergency
    }

    [Serializable]
    public class LightSettings
    {
        public Light light;
        public bool enabled = true;
        public Color color = Color.white;
        public float intensity = 1f;
        public float range = 10f;
        public float spotAngle = 30f;
        public LightShadows shadows = LightShadows.None;
    }
    
    public class LightPresetController : MonoBehaviour
    {
        [Header("Current Preset")]
        [SerializeField] private LightPreset currentPreset = LightPreset.Normal;
        [SerializeField] private bool applyInEditor = true;

        [Header("Normal")]
        [SerializeField] private LightSettings[] normalLights;

        [Header("Live")]
        [SerializeField] private LightSettings[] liveLights;

        [Header("Emergency")]
        [SerializeField] private LightSettings[] emergencyLights;

        private void OnValidate()
        {
            if (!applyInEditor)
            {
                return;
            }

            ApplyCurrentPreset();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }
            
            if (keyboard.qKey.wasPressedThisFrame)
            {
                ApplyPreset(normalLights);
            }
            if (keyboard.wKey.wasPressedThisFrame)
            {
                ApplyPreset(liveLights);
            }
            if (keyboard.eKey.wasPressedThisFrame)
            {
                ApplyPreset(emergencyLights);
            }
        }

        [ContextMenu("Apply Current Preset")]
        public void ApplyCurrentPreset()
        {
            switch (currentPreset)
            {
                case LightPreset.Normal:
                    ApplyPreset(normalLights);
                    break;
                case LightPreset.Live:
                    ApplyPreset(liveLights);
                    break;
                case LightPreset.Emergency:
                    ApplyPreset(emergencyLights);
                    break;
            }
        }

        [ContextMenu("Apply Normal")]
        public void ApplyNormal()
        {
            currentPreset = LightPreset.Normal;
            ApplyPreset(normalLights);
        }

        [ContextMenu("Apply Live")]
        public void ApplyLive()
        {
            currentPreset = LightPreset.Live;
            ApplyPreset(liveLights);
        }

        [ContextMenu("Apply Emergency")]
        public void ApplyEmergency()
        {
            currentPreset = LightPreset.Emergency;
            ApplyPreset(emergencyLights);
        }

        private void ApplyPreset(LightSettings[] settings)
        {
            foreach (LightSettings setting in settings)
            {
                if (setting.light == null)
                {
                    continue;
                }

                setting.light.enabled = setting.enabled;
                setting.light.color = setting.color;
                setting.light.intensity = setting.intensity;
                setting.light.shadows = setting.shadows;

                if (setting.light.type == LightType.Point || setting.light.type == LightType.Spot)
                {
                    setting.light.range = setting.range;
                }

                if (setting.light.type == LightType.Spot)
                {
                    setting.light.spotAngle = setting.spotAngle;
                }
            }
        }
        
    }
}
