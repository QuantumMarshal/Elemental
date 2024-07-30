using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlyEnemyConfig", menuName = "Config/EnemyConfig/FlyEnemyConfig", order = 1)]
public class FlyEnemyConfig : EnemyConfig
{
    public GameObject indicator;
    public override void Attack(Transform target)
    {
        Vector2 targetPos = target.position;

        float angle = GetRotationAngle(Vector2.right, targetPos);
    }

    private float GetRotationAngle(Vector2 from, Vector2 to)
    {
        // Calculate the angle in radians
        float angleRad = Mathf.Atan2(to.y - from.y, to.x - from.x);

        // Convert radians to degrees
        float angleDeg = Mathf.Rad2Deg * angleRad;

        return angleDeg;
    }
}
