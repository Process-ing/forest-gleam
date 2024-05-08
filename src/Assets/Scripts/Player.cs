using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public enum PlayerClass
    {
        NONE,
        SWORDSMAN,
        ARCHER,
        WIZARD
    }

    public Rigidbody2D rb;
    public int attackDamage;
    public float attackSpeed;
    public float attackRadius;
    public bool sturdyProjectile;
    public bool canMove;
    public bool isAttacking;
    public Vector2 attackVector;

    public int level;
    public int attackLevel;
    public int speedLevel;

    public LayerMask enemyLayer;
    public LayerMask itemLayer;
    public HealthBar healthBar;
    public LevelIndicator levelIndicator;
    public GameObject[] projectiles;
    public GameObject projectile;
    public TilemapGen tilemap;
    public Animator fadeWhite;
    public Animator fadeBlack;
    public AudioManager audioManager;

    public PlayerAnimation animator;
    public RuntimeAnimatorController NormalAnimator;
    public RuntimeAnimatorController SwordAnimator;
    public RuntimeAnimatorController BowAnimator;
    public RuntimeAnimatorController BookAnimator;

    private PlayerClass m_playerClass;
    private float nextAttackTime;
    private Vector2 drawPoint;
    public Vector2 animov;

    public PlayerClass playerClass
    {
        get { return m_playerClass; }

        set
        {
            m_playerClass = value;
            SetAttack();
            SetSpeed();
            switch (playerClass)
            {
                case PlayerClass.SWORDSMAN:
                    attackRadius = .5f;
                    sturdyProjectile = false;
                    animator.animator.runtimeAnimatorController = SwordAnimator;
                    break;
                case PlayerClass.ARCHER:
                    attackRadius = .2f;
                    projectile = projectiles[0];
                    sturdyProjectile = false;
                    animator.animator.runtimeAnimatorController = BowAnimator;
                    break;
                case PlayerClass.WIZARD:
                    attackRadius = .5f;
                    projectile = projectiles[1];
                    sturdyProjectile = true;
                    animator.animator.runtimeAnimatorController = BookAnimator;
                    break;
                case PlayerClass.NONE:
                    animator.animator.runtimeAnimatorController = NormalAnimator;
                    break;
            }
            projectile.GetComponent<Projectile>().isSturdy = sturdyProjectile;
        }
    }

    public void SetAttack()
    {
        switch (playerClass)
        {
            case PlayerClass.WIZARD:
                attackDamage = Mathf.RoundToInt(
                    32 * (1 - Mathf.Pow(.75f, (attackLevel + 3) / 4f))
                );  // initial: 8; max: 32
                break;
            case PlayerClass.SWORDSMAN:
            case PlayerClass.ARCHER:
                attackDamage = Mathf.RoundToInt(
                    40 * (1 - Mathf.Pow(.75f, (attackLevel + 3) / 4f))
                );  // initial: 10; max: 40
                break;
        }
        levelIndicator.UpdateLevel();
    }

    public void SetSpeed()
    {
        switch (playerClass)
        {
            case PlayerClass.SWORDSMAN:
                attackSpeed = Mathf.Pow(.2f, (speedLevel + 3) / 4f) + .2f;
                break;
            case PlayerClass.ARCHER:
            case PlayerClass.WIZARD:
                attackSpeed = Mathf.Pow(.7f, (speedLevel + 3) / 4f) + .5f;
                break;
        }
        levelIndicator.UpdateLevel();
    }

    void AttackMelee(Vector2 attackPoint)
    {
        drawPoint = attackPoint;
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint, attackRadius, enemyLayer);
        foreach (Collider2D enemy in enemiesHit)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    void AttackProjectile(Vector2 playerPos, Vector2 direction)
    {
        projectile.GetComponent<Projectile>().damage = attackDamage;
        projectile.GetComponent<Projectile>().radius = attackRadius;
        Instantiate(
            projectile,
            playerPos,
            Quaternion.LookRotation(Vector3.forward, direction)
        );
    }

    IEnumerator AttackAndCollect()
    {
        if (Time.time < nextAttackTime)
            yield break;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = new Vector2(rb.position.x, rb.position.y);
        attackVector = (mousePos - playerPos).normalized;

        if (CollectItems(playerPos + attackVector))
            yield break;

        canMove = false;
        isAttacking = true;
        switch (playerClass)
        {
            case PlayerClass.SWORDSMAN:
                AttackMelee(playerPos + attackVector);
                audioManager.Play("AttackSword");
                yield return new WaitForSeconds(.5f);
                break;
            case PlayerClass.ARCHER:
                yield return new WaitForSeconds(.36f);
                AttackProjectile(playerPos, attackVector);
                audioManager.Play("AttackBow");
                yield return new WaitForSeconds(.14f);
                break;
            case PlayerClass.WIZARD:
                yield return new WaitForSeconds(.36f);
                AttackProjectile(playerPos, attackVector);
                audioManager.Play("AttackMagic");
                yield return new WaitForSeconds(.14f);
                break;
        }
        nextAttackTime = Time.time + attackSpeed;

        canMove = true;
        isAttacking = false;
    }

    bool CollectItems(Vector2 collectPoint)
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(collectPoint, 1f, itemLayer);
        foreach (Collider2D item in items)
        {
            item.GetComponent<HelpItem>().Collect(this);
        }
        return items.Length > 0;
    }

    public bool CheckContact() => rb.IsTouchingLayers(enemyLayer);

    public void Heal(int health)
    {
        currentHealth += health;
        if (currentHealth > 10) currentHealth = 10;
        healthBar.SetHealth(currentHealth);
    }

    public IEnumerator LevelUp(HelpItem item)
    {
        level++;
        canMove = false;
        tilemap.KillEnemies();
        yield return new WaitForSeconds(4f);

        fadeWhite.SetTrigger("Start");
        yield return new WaitForSeconds(3f);

        tilemap.CreateNewLevel(level);
        rb.position = new Vector3(0, .5f, -1);
        Heal(maxHealth);
        levelIndicator.UpdateLevel();
        yield return new WaitForSeconds(3f);

        canMove = true;
        Destroy(item.gameObject);
        yield break;
    }

    IEnumerator ReduceVolume()
    {
        for (float volume = audioManager.GetComponent<AudioSource>().volume; volume < .3f; volume += 0.001f)
        {
            audioManager.GetComponent<AudioSource>().volume = volume;
            yield return null;
        }
    }

    IEnumerator DeathSequence()
    {
        audioManager.GetComponent<AudioSource>().volume = 0.06f;
        canMove = false;
        GetComponent<Collider2D>().enabled = false;
        Color c = sr.color;
        for (float alpha = 1f; alpha > 0f; alpha -= .01f)
        {
            c.a = alpha;
            sr.color = c;
            yield return new WaitForSeconds(0.01f);
        }
        sr.enabled = false;

        fadeBlack.SetTrigger("Start");
        yield return new WaitForSeconds(3f);

        sr.enabled = true;
        rb.position = new Vector3(0, .5f, -1);
        Heal(int.MaxValue);
        tilemap.RegenerateEnemies(level);
        c.a = 1;
        sr.color = c;
        StartCoroutine(ReduceVolume());
        yield return new WaitForSeconds(3f);

        GetComponent<Collider2D>().enabled = true;
        canMove = true;
    }

    public override void Die() => StartCoroutine(DeathSequence());

    public void ResetPlayer()
    {
        SetHealth(maxHealth);
        healthBar.SetMaxHealth(maxHealth);
        level = 1;
        attackLevel = 1;
        speedLevel = 1;
        nextAttackTime = Time.time;
        playerClass = PlayerClass.NONE;
        canMove = false;
        rb.position = new Vector3(0, .5f, -1);
    }

    public void LoadPlayer(int playerClass, int level, int attackLevel, int speedLevel)
    {
        ResetPlayer();
        this.playerClass = (PlayerClass)playerClass;
        this.level = level;
        this.attackLevel = attackLevel;
        this.speedLevel = speedLevel;
        SetAttack();
        SetSpeed();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<PlayerAnimation>();

        ResetPlayer();

        audioManager.Play("MainTheme");
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckContact())
        {
            if (!isInvincible)
                audioManager.Play("Hurt");
            TakeDamage(1);
            healthBar.SetHealth(currentHealth);
        }

        if (Input.GetMouseButtonDown(0) && canMove)
            StartCoroutine(AttackAndCollect());
    }

    private void OnDrawGizmos()
    {
        if (rb != null)
            Gizmos.DrawWireSphere(rb.position, 1);
        if (drawPoint != null)
            Gizmos.DrawWireSphere(drawPoint, attackRadius);
    }
}
