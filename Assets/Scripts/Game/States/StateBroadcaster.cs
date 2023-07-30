using UnityEngine;
using Toolbox.Events;

public class StateBroadcaster : StateMachineBehaviour
{
    [Header("Broadcasting on ...")]
    [SerializeField] VoidChannelSO OnEnter;
    [SerializeField] VoidChannelSO OnExit;

    ///<summary> OnStateEnter is called when a transition starts and the state machine starts to evaluate this state</summary>///
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        OnEnter?.Invoke();
    }

    ///<summary> OnStateExit is called when a transition ends and the state machine finishes evaluating this state</summary>///
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        OnExit?.Invoke();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
}
