using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using DG.Tweening;
using GameBase;
using GameBase.Managers;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyConfig enemyConfig;
    public EnemyConfig config;

    [SerializeField] private Transform target;

    [Header("MOVEMENT")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2 lastMoveInput;
    [SerializeField] private float moveSpeed;

    [Header("ATTACK")]
    [SerializeField] private float attackDuration;
    [SerializeField] private bool isAttack;
    [SerializeField] private bool canAttack = true;

    [Header("HURT")]
    [SerializeField] private float hurtDuration;
    [SerializeField] private bool isHurt;
    [SerializeField] private Vector2 hurtDirection;

    [Header("Health")]
    [SerializeField] private int health;

    [Header("ANIMATION")]
    [SerializeField] private EnemyAnimatorController animator;

    private Rigidbody2D rb;

    private static EnemyController instance;

    public static EnemyController Instance { get => instance; }

    private void SetComponent()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();

        config = enemyConfig.CopyConfig();
        SetAttribute();
    }

    private void SetAttribute()
    {
        moveSpeed = config.moveSpeed;
        attackDuration = config.attackDuration;
        hurtDuration = config.hurtDuration;
        health = config.health;
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(transform.parent.gameObject);
        SetComponent();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.parent.position, target.position) <= 7 && Vector3.Distance(transform.parent.position, target.position) >= 4)
        {
            EnemyMove(target);
        }
    }

    private void FixedUpdate()
    {
        if (isAttack || isHurt) return;
    }

    private void EnemyMove(Transform target)
    {
        transform.parent.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (isHurt) return;

        if (canAttack)
        {
            Attack();
            Game_SoundManager.Instance.PlaySound(GameBase.AudioPlayer.SoundID.SFX_ATTACK, Random.Range(0.3f, 1f));
        }
    }

    private void Attack()
    {
        isAttack = true;

        animator.SetAttack(lastMoveInput.x, lastMoveInput.y);
        float attackDuration = 0.3f;

        // Start coroutine to handle the attack duration
        StartCoroutine(AttackCooldown(attackDuration));
    }

    private IEnumerator AttackCooldown(float attackDuration)
    {
        canAttack = false;

        config.Attack(target);

        yield return new WaitForSeconds(attackDuration);

        isAttack = false;
        canAttack = true;
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