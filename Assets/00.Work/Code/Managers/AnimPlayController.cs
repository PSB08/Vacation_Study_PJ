using System;
using UnityEngine;

namespace Work.Code.Managers
{
    public class AnimPlayController : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Anim_Idle()
        {
            PlayAnimation("Idle");
        }

        public void Anim_Walk()
        {
            PlayAnimation("Walk");
        }

        public void Anim_Dance(int index)
        {
            PlayAnimation($"Dance{index}");
        }
        
        public void PlayAnimation(string stateName)
        {
            if (_animator == null)
            {
                Debug.LogWarning("[AnimationButtonController] Animator is missing.");
                return;
            }

            _animator.CrossFadeInFixedTime(stateName, 0.15f);
        }
        
    }
}