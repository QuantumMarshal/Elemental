using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;
    private string currentState;

    // Animation state names
    private const string STATE_IDLE = "IDLE";
    private const string STATE_MOVE = "MOVE";
    private const string STATE_ATTACK = "ATTACK";

    public bool isDashAnimFinish = true;

    private void Awake()
    {
        animator = transform.parent.Find("Model").GetComponent<Animator>();
    }

    public void SetIdle(float LastXInput, float LastYInput)
    {
        // Debug.Log("Setting Idle State");
        ChangeState(STATE_IDLE);
        animator.SetFloat("LastXInput", LastXInput);
        animator.SetFloat("LastYInput", LastYInput);
    }

    public void SetMove(float XInput, float YInput)
    {
        // Debug.Log("Setting Move State");
        ChangeState(STATE_MOVE);
        animator.SetFloat("XInput", XInput);
        animator.SetFloat("YInput", YInput);
    }

    private void ChangeState(string newState)
    {
        if (currentState == newState) return;

        // Debug.Log($"Changing state from {currentState} to {newState}");
        animator.Play(newState);
        currentState = newState;
    }

    public void SetAttack(float LastXInput, float LastYInput)
    {
        ChangeState(STATE_ATTACK);

        animator.SetFloat("LastXInput", LastXInput);
        animator.SetFloat("LastYInput", LastYInput);
    }

    public float GetCurrentAnimationLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    public string GetState()
    {
        return currentState;
    }
}
