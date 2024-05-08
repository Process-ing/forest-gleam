using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Animator animator;
    public Animator GameUIAnimator;
    public Animator mainMenuAnimator;
    public Animator SavedTextAnimator;
    public Animator fadeBlack;
    public Player player;
    public TilemapGen tilemap;
    public MainMenu menu;

    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && player.canMove)
        {
            animator.SetTrigger("Toggle");
            Time.timeScale = 0f;
            player.canMove = false;
            isPaused = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            animator.SetTrigger("Toggle");
            Time.timeScale = 1f;
            player.canMove = true;
            isPaused = false;
        }
    }

    IEnumerator MenuSequence()
    {
        Time.timeScale = 1f;
        fadeBlack.SetTrigger("Start");
        animator.SetTrigger("Toggle");
        GameUIAnimator.SetTrigger("Toggle");
        yield return new WaitForSeconds(3f);

        menu.update = true;
        player.ResetPlayer();
        tilemap.KillEveryone(true);
        tilemap.CreateNewLevel(1);
        yield return new WaitForSeconds(3f);

        mainMenuAnimator.SetTrigger("Toggle");
    }

    public void Save()
    {
        SaveSystem.SaveData(player, tilemap);
        SavedTextAnimator.SetTrigger("Start");
    }

    public void ToMenu() => StartCoroutine(MenuSequence());
}
