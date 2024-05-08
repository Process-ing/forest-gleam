using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float radius;
    public int damage;
    public bool isSturdy;
    public LayerMask enemyLayer;

    void Move() => transform.position += speed * Time.fixedDeltaTime * transform.up;

    void Collide()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        foreach (Collider2D enemy in enemiesHit)
        {
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }

        if (!isSturdy && enemiesHit.Length > 0)
            Destroy(gameObject);
    }

    private void Update()
    {
        Collide();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
