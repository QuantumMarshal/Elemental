using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    private Vector2 direction;
    private float time;
    private float maxTime = 1;

    private void OnEnable()
    {
        StartCoroutine(Cor_Move());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
    }

    private void MoveByDirection(Vector2 direction)
    {
        transform.DOMove((Vector2)transform.position + direction, 0.3f);        
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
        time = 0f;
        
    }

    public void SetPosition(Vector2 position, float duration)
    {
        transform.position = position;
        maxTime = duration;
    }

    private IEnumerator Cor_Move()
    {
        while (time <= maxTime)
        {
            MoveByDirection(direction);
            time += Time.deltaTime;
            
            yield return null;
        }
        Destroy(gameObject);
    }
}
