using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Entity
{
    public int initialHealth;
    public int healthIncrease;

    IEnumerator Fade()
    {
        Color c = sr.color;
        for (float alpha = 1f; alpha > 0f; alpha -= .01f)
        {
            c.a = alpha;
            sr.color = c;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }

    public override void Die()
    {
        Destroy(GetComponent<CircleCollider2D>());
        Destroy(GetComponent<enemyAI>());
        StartCoroutine(Fade());
    }

    public void UpdateLevel(int level)
    {
        maxHealth = initialHealth + (level - 1) * healthIncrease;
    }

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        GetComponent<UnityEngine.Rendering.Universal.ShadowCaster2D>().enabled = false;
        SetHealth(maxHealth);
    }

    private void OnBecameVisible()
    {
        GetComponent<UnityEngine.Rendering.Universal.ShadowCaster2D>().enabled = true;
    }

    private void OnBecameInvisible()
    {
        GetComponent<UnityEngine.Rendering.Universal.ShadowCaster2D>().enabled = false;
    }
}
