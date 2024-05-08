using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isInvincible;
    public float invincibilityPeriod;
    public SpriteRenderer sr;

    public void SetHealth(int health)
    {
        currentHealth = maxHealth = health;
    }

    public IEnumerator ClearUp()
    {
        isInvincible = true;
        Color c = sr.color;

        c.a = .5f;
        sr.color = c;
        yield return new WaitForSeconds(invincibilityPeriod);

        c.a = 1f;
        sr.color = c;
        isInvincible = false;
        yield break;
    }

    public abstract void Die();

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;

        currentHealth -= damage;
        StartCoroutine(ClearUp());
        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
