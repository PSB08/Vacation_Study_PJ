using UnityEngine;
using UniVRM10;

namespace Work.Code.Managers
{
    public class CharacterExpressionController : MonoBehaviour
    {
        [SerializeField] private Vrm10Instance vrm;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform characterRoot;
        
        [Header("Fallback")]
        [SerializeField] private string idleStateName = "Idle";
        [SerializeField] private Vector3 fallbackLocalPosition = Vector3.zero;
        [SerializeField] private Vector3 fallbackLocalEulerAngles = Vector3.zero;
        [SerializeField] private Vector3 fallbackLocalScale = Vector3.one;
        
        public void SetNeutral()
        {
            ClearEmotionExpressions();
            SetExpression(ExpressionKey.Neutral, 1f, "Neutral");
        }
        
        public void SetHappy()
        {
            ClearEmotionExpressions();
            SetExpression(ExpressionKey.Happy, 1f, "Happy");
        }
        
        public void SetSurprised()
        {
            ClearEmotionExpressions();
            SetExpression(ExpressionKey.Surprised, 1f, "Surprised");
        }
        
        public void FallbackToIdle()
        {
            ClearEmotionExpressions();
            
            if (characterRoot != null)
            {
                characterRoot.localPosition = fallbackLocalPosition;
                characterRoot.localEulerAngles = fallbackLocalEulerAngles;
                characterRoot.localScale = fallbackLocalScale;
            }
            
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
                animator.CrossFade(idleStateName, 0.15f, 0);
            }
        }
        
        private void SetExpression(ExpressionKey key, float weight, string label)
        {
            if (vrm == null)
            {
                Debug.LogWarning("[Character] Vrm10Instance is not assigned.");
                return;
            }
            
            vrm.Runtime.Expression.SetWeight(key, weight);
        }
        
        private void ClearEmotionExpressions()
        {
            if (vrm == null)
            {
                return;
            }
            
            vrm.Runtime.Expression.SetWeight(ExpressionKey.Neutral, 0f);
            vrm.Runtime.Expression.SetWeight(ExpressionKey.Happy, 0f);
            vrm.Runtime.Expression.SetWeight(ExpressionKey.Angry, 0f);
            vrm.Runtime.Expression.SetWeight(ExpressionKey.Sad, 0f);
            vrm.Runtime.Expression.SetWeight(ExpressionKey.Relaxed, 0f);
            vrm.Runtime.Expression.SetWeight(ExpressionKey.Surprised, 0f);
        }
        
    }
}