using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelIndicator : MonoBehaviour
{
    public Player player;
    public Text text;
    
    public void UpdateLevel()
    {
        text.text = 
            $"Nivel: {player.level}\n"
            + $"Nivel de Ataque: {player.attackLevel}\n"
            + $"Nivel de Destreza: {player.speedLevel}";
    }

    private void Start()
    {
        text = GetComponent<Text>();
    }
}
