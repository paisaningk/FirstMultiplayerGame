using System.Collections;
using UnityEngine;

namespace utilities
{
    public static class AnimationUtils
    {
        public static IEnumerator CloseGameObjectWhenAnimEnd(this GameObject parent, Animator animator,
            bool activeStateAtTheEnd = true)
        {
            yield return new WaitForSeconds(animator.GetAnimatorLength());
            parent.SetActive(activeStateAtTheEnd);
        }

        private static int GetAnimatorLength(this Animator animator)
        {
            return animator.GetCurrentAnimatorClipInfo(0).Length;
        }
    }
}