using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private string currentState;

    // Animation state names
    private const string STATE_IDLE = "IDLE";
    private const string STATE_MOVE = "MOVE";
    private const string STATE_DASH = "DASH";
    private const string STATE_ATTACK1 = "ATTACK";
    //private const string STATE_ATTACK2 = "Attack2";
    //private const string STATE_ATTACK3 = "Attack3";

    private void Awake()
    {
        animator = transform.parent.Find("Model").GetComponent<Animator>();
    }

    public void SetIdle(float LastXInput, float LastYInput)
    {
        ChangeState(STATE_IDLE);
        animator.SetFloat("LastXInput", LastXInput);
        animator.SetFloat("LastYInput", LastYInput);
    }

    public void SetMove(float XInput, float YInput)
    {
        ChangeState(STATE_MOVE);
        animator.SetFloat("XInput", XInput);
        animator.SetFloat("YInput", YInput);
    }

    public void SetDash(float LastXInput, float LastYInput)
    {
        ChangeState(STATE_DASH);
        animator.SetFloat("LastXInput", LastXInput);
        animator.SetFloat("LastYInput", LastYInput);
    }

    public void SetAttack(int attackNumber, float LastXInput, float LastYInput)
    {
        switch (attackNumber)
        {
            case 1:
                ChangeState(STATE_ATTACK1);
                break;
            //case 2:
            //    ChangeState(STATE_ATTACK2);
            //    break;
            //case 3:
            //    ChangeState(STATE_ATTACK3);
            //    break;
            default:
                ChangeState(STATE_IDLE);
                break;
        }

        animator.SetFloat("LastXInput", LastXInput);
        animator.SetFloat("LastYInput", LastYInput);
    }

    private void ChangeState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);
        currentState = newState;
    }

    public float GetCurrentAnimationLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }
}
