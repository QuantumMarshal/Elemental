using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting.FullSerializer;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private PlayerConfig config;

    [Header("MOVEMENT")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2 lastMoveInput;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isMove;

    [Header("DASH")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDuration;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing;

    [Header("ATTACK")]
    [SerializeField] private float attackDuration;
    [SerializeField] private bool isAttack;
    [SerializeField] private bool canAttack = true;
    private int attackCount = 0;
    private float lastAttackTime;

    [Header("HURT")]
    [SerializeField] private float hurtDuration;
    [SerializeField] private bool isHurt;
    [SerializeField] private Vector2 hurtDirection;

    [Header("Health")]
    [SerializeField] private int health;

    [Header("ANIMATION")]
    [SerializeField] private PlayerAnimatorController animator;

    private Rigidbody2D rb;
    private PlayerInputAction PlayerInputAction;

    private static PlayerController instance;

    public static PlayerController Instance { get => instance; }

    private void SetComponent()
    {
        PlayerInputAction = new PlayerInputAction();
        PlayerInputAction.Enable();
        PlayerInputAction.InputAction.Dash.started += Dash;
        PlayerInputAction.InputAction.Attack.started += Attack;

        rb = transform.parent.GetComponent<Rigidbody2D>();

        config = playerConfig.CopyConfig();
        SetAttribute();
    }

    private void SetAttribute()
    {
        moveSpeed = config.moveSpeed;
        dashSpeed = config.dashSpeed;
        dashCooldown = config.dashCooldown;
        dashDuration = config.dashDuration;
        attackDuration = config.attackDuration;
        hurtDuration = config.hurtDuration;
        health = config.health;
    }

    private void ResetComponent()
    {
        PlayerInputAction.Disable();
        PlayerInputAction.InputAction.Dash.started -= Dash;
        PlayerInputAction.InputAction.Attack.started -= Attack;
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(transform.parent.gameObject);
        playerConfig = Resources.Load("Config/PlayerConfig") as PlayerConfig;
        SetComponent();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        if (isDashing || isAttack || isHurt) return;
        PlayerMove();
    }

    private void GetInput()
    {
        moveInput = PlayerInputAction.InputAction.Movement.ReadValue<Vector2>();
        
        if (moveInput != Vector2.zero)
        {
            lastMoveInput = moveInput;
            isMove = true;
            animator.SetMove(moveInput.x, moveInput.y);
        }
        else
        {
            isMove = false;
            animator.SetIdle(lastMoveInput.x, lastMoveInput.y);
        }
    }

    private void PlayerMove()
    {
        rb.velocity = moveInput * moveSpeed; 
    }

    private void Dash(InputAction.CallbackContext context)
    {
        Debug.Log("Dash Clicked");
        StartCoroutine(Cor_Dash());
    }

    private void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack Clicked");

        if (!canAttack || isHurt) return;
        isAttack = true;

        animator.SetAttack(1, lastMoveInput.x, lastMoveInput.y);

        StartCoroutine(AttackCooldown(attackDuration));
    }

    private IEnumerator AttackCooldown(float attackDuration)
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDuration);
        canAttack = true;
        isAttack = false;
        yield return new WaitForEndOfFrame();
        animator.SetAttack(999, lastMoveInput.x, lastMoveInput.y);
    }

    private IEnumerator Cor_Dash()
    {
        if (isDashing || !canDash || isHurt) yield break;

        isDashing = true;
        canDash = false;

        animator.SetDash(lastMoveInput.x, lastMoveInput.y);
        Vector2 dashVector = moveInput * dashSpeed;
        rb.velocity = dashVector;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        isDashing = false;
        animator.SetIdle(lastMoveInput.x, lastMoveInput.y);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Hurt(int atk)
    {
        isHurt = true;
        health -= atk;
        hurtDirection = lastMoveInput * -0.2f;

        StartCoroutine(Cor_Hurt());
    }

    private IEnumerator Cor_Hurt()
    {
        transform.parent.DOMove(transform.position + (Vector3)hurtDirection, 0.1f);
        SpriteRenderer sprite = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();

        sprite.DOColor(Color.red, 0.1f);
        yield return new WaitForSeconds(hurtDuration);
        sprite.DOColor(Color.white, 0.1f);
        isHurt = false;
    }

}