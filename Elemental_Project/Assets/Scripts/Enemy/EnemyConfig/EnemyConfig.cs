using GameBase.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Config/EnemyConfig", order = 2)]
public class EnemyConfig : ScriptableObject
{
    [Header("MOVEMENT")]
    public float moveSpeed;

    [Header("ATTACK")]
    public float attackDuration;

    [Header("HURT")]
    public float hurtDuration;

    [Header("HEALTH")]
    public int health;
    public int maxHealth;

    [Header("RANGE")]
    public float minRangeToPlayer;
    public float maxRangeToPlayer;

    [Header("TIME")]
    public float attackCooldown;

    public EnemyConfig CopyConfig()
    {
        EnemyConfig copy = CreateInstance<EnemyConfig>();
        copy.moveSpeed = moveSpeed;
        copy.attackDuration = attackDuration;
        copy.hurtDuration = hurtDuration;
        copy.health = health;
        copy.maxHealth = maxHealth;
        copy.minRangeToPlayer = minRangeToPlayer;
        copy.maxRangeToPlayer = maxRangeToPlayer;

        return copy;
    }

    public virtual void Attack(Transform target)
    {
        
    }
}
