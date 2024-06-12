using System.Collections;
using Fusion;
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

        public static bool IsLocalPlayer(this NetworkObject networkObject)
        {
            return networkObject.IsValid == networkObject.HasInputAuthority;
        }

        private static int GetAnimatorLength(this Animator animator)
        {
            return animator.GetCurrentAnimatorClipInfo(0).Length;
        }
    }
}