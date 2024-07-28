using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using DG.Tweening;
using GameBase;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private PlayerConfig config;

    [Header("MOVEMENT")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2 lastMoveInput;
    [SerializeField] private float moveSpeed;

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
    [SerializeField] private float comboResetTime = 1f; // Time allowed to press the next attack in the combo

    private int comboStep = 0;
    private float lastAttackTime = 0f;

    [Header("SKILL")]
    [SerializeField] private GameObject skillPrefab;

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
        PlayerInputAction.InputAction.Skill.started += LaunchSkill;

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
        PlayerInputAction.InputAction.Skill.started -= LaunchSkill;
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


        if (isDashing || isAttack || isHurt) return;
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
            animator.SetMove(moveInput.x, moveInput.y);
        }
        else
        {
            animator.SetIdle(lastMoveInput.x, lastMoveInput.y);
        }
    }

    private void PlayerMove()
    {
        rb.velocity = moveInput * moveSpeed; 
    }

    private void Dash(InputAction.CallbackContext context)
    {
        StartCoroutine(Cor_Dash());
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (isDashing || isHurt) return;

        if (canAttack)
        {
            PerformComboAttack();
        }
    }

    private void PerformComboAttack()
    {
        isAttack = true;
        lastAttackTime = Time.time;

        comboStep++;
        if (comboStep > 2) comboStep = 1; // Assuming 2 combo steps

        animator.SetAttack(comboStep, lastMoveInput.x, lastMoveInput.y);
        float attackDuration = comboStep == 1 ? 0.2f : 0.3f;

        // Start coroutine to handle the attack duration
        StartCoroutine(AttackCooldown(attackDuration));
    }

    private IEnumerator AttackCooldown(float attackDuration)
    {
        canAttack = false;

        // Assume each attack has a different duration, you can customize these durations

        yield return new WaitForSeconds(attackDuration);

        isAttack = false;
        canAttack = true;

        if (comboStep == 2) // If it's the end of the combo
        {
            yield return new WaitForSeconds(comboResetTime);
            comboStep = 0; // Reset combo
        }
    }

    private IEnumerator Cor_Dash()
    {
        if (isDashing || !canDash || isHurt) yield break;

        Debug.Log("Starting Dash");
        isDashing = true;
        canDash = false;

        animator.SetDash(lastMoveInput.x, lastMoveInput.y);
        Vector2 dashVector = lastMoveInput * dashSpeed;
        rb.velocity = dashVector;

        yield return new WaitForSeconds(dashDuration);

        animator.SetIdle(lastMoveInput.x, lastMoveInput.y);
        rb.velocity = Vector2.zero;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void LaunchSkill(InputAction.CallbackContext context)
    {
        Debug.Log("LauchSkill");
        LaunchFireball();
    }

    private void LaunchFireball()
    {
        if (skillPrefab != null)
        {
            //float z = GetRotationAngle(Vector2.right, lastMoveInput);

            float z = Mathf.Atan2(lastMoveInput.y, lastMoveInput.x) * Mathf.Rad2Deg;

            Vector3 skillRotation = new Vector3(0, 0, z);

            GameObject fireball = Instantiate(skillPrefab, transform.position, Quaternion.Euler(skillRotation));
            fireball.GetComponent<SkillController>().SetDirection(lastMoveInput);
            fireball.SetActive(true);
        }
    }
    public float GetRotationAngle(Vector2 from, Vector2 to)
    {
        // Calculate the angle in radians
        float angleRad = Mathf.Atan2(to.y - from.y, to.x - from.x);

        // Convert radians to degrees
        float angleDeg = Mathf.Rad2Deg * angleRad;

        return angleDeg;
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