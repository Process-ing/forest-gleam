using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public Player player;
    public PlayerTestMov playerMov;

    public string[] staticDirs = { "Static NW", "Static SW", "Static SE", "Static NE" };
    public string[] runDirs = { "Run NW", "Run SW", "Run SE", "Run NE" };
    public string[] attackDirs = { "Attack NW", "Attack SW", "Attack SE", "Attack NE" };
    public int lastDirection;

    public void SetDirection()
    {
        string[] directionArray = null;

        if (player.isAttacking)
        {
            directionArray = attackDirs;
            lastDirection = DirectionToIndex(player.attackVector);
        }
        else if (playerMov.movement.sqrMagnitude != 0)
        {
            directionArray = runDirs;
            lastDirection = DirectionToIndex(playerMov.movement);
        }
        else
        {
            directionArray = staticDirs;
        }
        if (animator.runtimeAnimatorController == player.NormalAnimator)
            animator.Play("Static SW");
        else
            animator.Play(directionArray[lastDirection]);
    }

    private int DirectionToIndex(Vector2 direction)
    {
        const float step = 90;

        float angle = Vector2.SignedAngle(Vector2.up, direction.normalized);

        if (angle < 0)
            angle += 360;

        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount);
    }

    private void Start()
    {
        lastDirection = 1;
        animator = player.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        SetDirection();
    }
}
