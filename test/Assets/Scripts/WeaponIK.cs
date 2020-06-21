using UnityEngine;

[RequireComponent(typeof(Animator))]

public class WeaponIK: MonoBehaviour {

    protected Animator animator;

    [SerializeField]
    private bool ikActive = false;

    [SerializeField]
    private Transform rightHandObj;

    [SerializeField]
    private Transform leftHandTarget;

    [SerializeField]
    private Vector3 rightHandRotation;

    void Start() {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK() {
        if (animator) {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive) {

                // Set the look __target position__, if one has been assigned
                if (leftHandTarget != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(leftHandTarget.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null) {
                    //animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    //animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(Quaternion.LookRotation(leftHandTarget.position - rightHandObj.position).eulerAngles + rightHandRotation));//Quaternion.Euler(leftHandTarget.rotation.eulerAngles + rightHandRotation)
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
