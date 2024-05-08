using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSelection : MonoBehaviour
{
    public Player player;
    public Animator animator;
    public Animator GameUIAnimator;
    public IEnumerator InitGame()
    {
        animator.SetTrigger("Toggle");
        yield return new WaitForSeconds(3f);

        player.canMove = true;
        GameUIAnimator.SetTrigger("Toggle");
        yield break;
    }
    public void ClassSelected(Player.PlayerClass playerClass)
    {
        if (player.playerClass != playerClass)
        {
            player.playerClass = playerClass;
        }
        else
        {
            StartCoroutine(InitGame());
        }
    }

    public void SwordsmanSelected() => ClassSelected(Player.PlayerClass.SWORDSMAN);

    public void ArcherSelected() => ClassSelected(Player.PlayerClass.ARCHER);

    public void WizardSelected() => ClassSelected(Player.PlayerClass.WIZARD);
}
