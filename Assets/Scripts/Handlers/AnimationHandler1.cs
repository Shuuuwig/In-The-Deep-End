using UnityEngine;

public class AnimationHandler
{
    public static void PlayAnimation(Animator animator, AnimationClip animationClip)
    {
        animator.Play(animationClip.name);
    }
}
