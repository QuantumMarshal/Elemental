using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject
{
    [Header("MOVEMENT")]
    public float moveSpeed;

    [Header("DASH")]
    public float dashSpeed;
    public float dashCooldown;
    public float dashDuration;

    [Header("ATTACK")]
    public float attackDuration;

    [Header("HURT")]
    public float hurtDuration;

    [Header("HEALTH")]
    public int health;

    public PlayerConfig CopyConfig()
    {
        PlayerConfig copy = new PlayerConfig();
        copy.moveSpeed = moveSpeed;
        copy.dashSpeed = dashSpeed;
        copy.dashCooldown = dashCooldown;
        copy.dashDuration = dashDuration;   
        copy.attackDuration = attackDuration;   
        copy.hurtDuration = hurtDuration;
        copy.health = health;
        return copy;
    }
}
