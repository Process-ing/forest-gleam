using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Animator animator;
    public Animator classSelAnimator;
    public Animator gameUIAnimator;
    public Button loadButton;
    public Text loadButtonText;
    public Animator fadeBlack;
    public Player player;
    public TilemapGen tilemap;
    public AudioManager audioManager;
    public bool update;

    private IEnumerator IniciateNewGame()
    {
        animator.SetTrigger("Toggle");
        yield return new WaitForSeconds(1.5f);

        classSelAnimator.SetTrigger("Toggle");
        yield break;
    }

    public void PlayNewGame() => StartCoroutine(IniciateNewGame());

    public IEnumerator LoadSequence()
    {
        animator.SetTrigger("Toggle");
        yield return new WaitForSeconds(1.5f);

        fadeBlack.SetTrigger("Start");
        yield return new WaitForSeconds(3f);

        SaveData data = SaveSystem.LoadData();
        player.LoadPlayer(data.playerClass, data.level, data.attackLevel, data.speedLevel);
        tilemap.LoadTilemap(data.grid, data.itemInfo, data.size, data.border, data.level);
        yield return new WaitForSeconds(3f);

        player.canMove = true;
        gameUIAnimator.SetTrigger("Toggle");
    }

    public void LoadGame() => StartCoroutine(LoadSequence());

    IEnumerator QuitSequence()
    {
        audioManager.gameObject.SetActive(false);
        fadeBlack.SetTrigger("Start");
        animator.SetTrigger("Toggle");
        yield return new WaitForSeconds(3f);

        Application.Quit();
    }

    public void QuitGame() => StartCoroutine(QuitSequence());

    private void Start()
    {
        update = true;
    }

    private void Update()
    {
        if (update)
        {
            if (SaveSystem.SaveDataExists())
            {
                loadButton.interactable = true;
                loadButtonText.color = Color.white;
            }
            else
            {
                loadButton.interactable = false;
                loadButtonText.color = Color.gray;
            }
            update = false;
        }
    }
}
