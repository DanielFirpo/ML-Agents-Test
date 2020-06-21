using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionDiscriminator: MonoBehaviour {

    [SerializeField]
    private Animator animator;

    // Callback for processing animation movements for modifying root motion.
    void OnAnimatorMove() {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // APPLY DEFAULT ROOT MOTION, ONLY WHEN IN THESE ANIMATION STATES
        if (stateInfo.IsTag("RootMotion")) {
            animator.ApplyBuiltinRootMotion();
        }
        else {
            animator.applyRootMotion = false;
        }

    }
}
