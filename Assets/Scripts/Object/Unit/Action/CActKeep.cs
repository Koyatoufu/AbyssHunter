using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CActKeep : StateMachineBehaviour
{
    [SerializeField]
    private string m_flagName = "InAction";
    [SerializeField]
    private bool m_flagStatus = true;
    [SerializeField]
    private bool m_isFlagReset = true;
    [SerializeField]
    private bool m_isAnyUpdates = true;

    [SerializeField]
    private string m_subFlagName = "";
    [SerializeField]
    private bool m_subFlagStatus = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(m_flagName, m_flagStatus);

        if (!string.IsNullOrEmpty(m_subFlagName))
            animator.SetBool(m_subFlagName, m_subFlagStatus);
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_isAnyUpdates)
            return;

        animator.SetBool(m_flagName, m_flagStatus);

        if (!string.IsNullOrEmpty(m_subFlagName))
            animator.SetBool(m_subFlagName, m_subFlagStatus);
    }

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_isFlagReset)
        {
            animator.SetBool(m_flagName, !m_flagStatus);

            if (!string.IsNullOrEmpty(m_subFlagName))
                animator.SetBool(m_subFlagName, !m_subFlagStatus);
        }            
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
