using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int[,] grid;
    public float[,] itemInfo;
    public int size;
    public int border;

    public int playerClass;
    public int level;
    public int attackLevel;
    public int speedLevel;

    public SaveData(Player player, TilemapGen tilemap)
    {
        grid = tilemap.GetGrid();
        itemInfo = tilemap.GetItemInfo();
        size = tilemap.size;
        border = tilemap.border;

        playerClass = (int)player.playerClass;
        level = player.level;
        attackLevel = player.attackLevel;
        speedLevel = player.speedLevel;
    }
}
