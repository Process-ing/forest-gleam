using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpItem : MonoBehaviour
{
    public enum Type
    {
        POWER,
        HASTE,
        HEAL,
        LIGHT
    }

    public Type type;
    public SpriteRenderer sr;
    public AudioManager audioManager;

    public void Collect(Player player)
    {
        switch (type)
        {
            case Type.POWER:
                player.attackLevel++;
                player.SetAttack();
                Destroy(gameObject);
                audioManager.Play("PickupItem");
                break;
            case Type.HASTE:
                player.speedLevel++;
                player.SetSpeed();
                Destroy(gameObject);
                audioManager.Play("PickupItem");
                break;
            case Type.HEAL:
                player.Heal(2);
                Destroy(gameObject);
                audioManager.Play("PickupItem");
                break;
            case Type.LIGHT:
                sr.enabled = false;
                audioManager.Play("PickupLightItem");
                StartCoroutine(player.LevelUp(this));
                break;
        }
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
}
