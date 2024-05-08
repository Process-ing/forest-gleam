using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.U2D;
using UnityEditor;

public class TilemapGen : MonoBehaviour
{
    public List<TileBase> tiles = new List<TileBase>();
    public int size;
    public float terrainScale;
    public float woodsScale;
    public float waterLevel;
    public float rockLevel;
    public float woodsLevel;
    public int itemCount;
    public int enemyCount;
    public int border;

    public GameObject treeLight;
    public GameObject flowerLight;
    public GameObject lightsParent;
    public GameObject[] items;
    public GameObject itemsParent;
    public GameObject[] enemies;
    public GameObject enemiesParent;
    public AudioManager audioManager;
    public Player player;
    public Tilemap trees;
    public Tilemap fog;

    enum TileType
    {
        GRASS,
        GLOW_TREE,
        GLOW_FLOWER,
        FLOWER,
        TREE,
        ROCK,
        FOG,
        WATER,
        BOAT,
        DUCK,
        WATERLILYLEFT,
        WATERLILYRIGHT,
        HARD_ROCK
    }

    private int seed;
    private TileType[,] grid;
    private Tilemap tilemap;
    private float terrainXOffset;
    private float terrainYOffset;
    private float woodsXOffset;
    private float woodsYOffset;
    private GameObject lightItem;

    void GetTile()
    {

    }

    void SetOffsets()
    {
        terrainXOffset = Random.Range(0, 2000);
        terrainYOffset = Random.Range(0, 2000);
        woodsXOffset = Random.Range(0, size);
        woodsYOffset = Random.Range(0, size);
    }

    void GenerateGrid()
    {
        grid = new TileType[size + 2 * border, size + 2 * border];
        for (int x = 0; x < size + 2 * border; x++)
            for (int y = 0; y < size + 2 * border; y++)
                grid[x, y] = TileType.GRASS;
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < size + 2 * border; x++)
        {
            for (int y = 0; y < size + 2 * border; y++)
            {
                float level = Mathf.PerlinNoise(
                    x * terrainScale + terrainXOffset, 
                    y * terrainScale + terrainYOffset
                );
                if (level < waterLevel)
                    grid[x, y] = TileType.WATER;
                else if (level > rockLevel)
                    grid[x, y] = TileType.ROCK;
            }
        }
    }

    void GenerateTrees()
    {
        for (int x = 0; x < size + 2 * border; x++)
        {
            for (int y = 0; y < size + 2 * border; y++)
            {
                if ((int)grid[x, y] < 5)
                {
                    float level = Mathf.PerlinNoise(
                        x * woodsScale + woodsXOffset,
                        y * woodsScale + woodsYOffset
                    );

                    int treeValue = Random.Range(0, 50);
                    if (level >= woodsLevel)
                    {
                        if (treeValue >= 20)
                            grid[x, y] = TileType.TREE;
                        else if (treeValue == 19)
                            grid[x, y] = TileType.GLOW_TREE;
                    }
                    else
                    {
                        if (treeValue >= 40)
                            grid[x, y] = TileType.TREE;
                        else if (treeValue == 39)
                            grid[x, y] = TileType.GLOW_TREE;
                    }
                }
            }
        }
    }

    void GenerateFlowers()
    {
        for (int x = 0; x < size + 2 * border; x++)
        {
            for (int y = 0; y < size + 2 * border; y++)
            {
                if ((int)grid[x, y] < 4)
                {
                    if (Random.Range(0, 20) <= 1)
                        grid[x, y] = TileType.FLOWER;
                    else if (Random.Range(0, 20) == 2)
                        grid[x, y] = TileType.GLOW_FLOWER;
                }
            }
        }
    }

    void GenerateExtras()
    {
        for (int x = 0; x < size + 2 * border; x++)
        {
            for (int y = 0; y < size + 2 * border; y++)
            {
                int value = Random.Range(0, 200);
                if (grid[x, y] == TileType.WATER)
                {
                    if (value == 0)
                    {
                        grid[x, y] = TileType.DUCK;
                    }
                    else if (value == 1)
                    {
                        grid[x, y] = TileType.BOAT;
                    }
                    else if (value == 2)
                    {
                        grid[x, y] = TileType.WATERLILYLEFT;
                    }
                    else if (value == 3)
                    {
                        grid[x, y] = TileType.WATERLILYRIGHT;
                    }
                }

                if (grid[x, y] == TileType.ROCK && value < 10)
                    grid[x, y] = TileType.HARD_ROCK;
            }
        }
    }

    void FillTilemap()
    {
        for (int x = 0; x < size + 2 * border; x++)
            for (int y = 0; y < size + 2 * border; y++)
                if (grid[x, y] == TileType.TREE || grid[x, y] == TileType.GLOW_TREE)
                {
                    tilemap.SetTile(
                        new Vector3Int(x - size / 2 - border, y - size / 2 - border, 0),
                        tiles[(int)TileType.GRASS]
                    );
                    trees.SetTile(
                        new Vector3Int(x - size / 2 - border, y - size / 2 - border, 0),
                        tiles[(int)grid[x, y]]
                    );
                }
                else
                {
                    tilemap.SetTile(
                        new Vector3Int(x - size / 2 - border, y - size / 2 - border, 0),
                        tiles[(int)grid[x, y]]
                    );
                }

                        for (int x = 0; x < size + 2 * border; x++)
            for (int y = 0; y < size + 2 * border; y++)
                if (!(x >= border && x < size + border && y >= border && y < size + border))
                    fog.SetTile(
                        new Vector3Int(x - size / 2 - border, y - size / 2 - border, 1),
                        tiles[(int)TileType.FOG]
                    );
    }

    void GenerateLights()
    {
        for (int x = border; x < size + border; x++)
        {
            for (int y = border; y < size + border; y++)
            {
                if (grid[x, y] == TileType.GLOW_FLOWER)
                {
                    Instantiate(
                        flowerLight,
                        tilemap.CellToWorld(new Vector3Int(x - size / 2 - border, y - size / 2 - border, 0)) + new Vector3(0, .25f, 0), 
                        Quaternion.identity,
                        lightsParent.transform
                    );
                }
                else if (grid[x, y] == TileType.GLOW_TREE)
                {
                    Instantiate(
                        treeLight,
                        tilemap.CellToWorld(new Vector3Int(x - size / 2 - border, y - size / 2 - border, 0)) + new Vector3(0, .25f, 0),
                        Quaternion.identity,
                        lightsParent.transform
                    );
                }
            }
        }

        Instantiate(
            treeLight,
            lightItem.transform.position,
            Quaternion.identity,
            lightsParent.transform
        );
    }

    bool IsFarFromCenter(int x, int y) => x * x + y * y > 2500;  // distance: 50

    bool CheckAvailableTile(int x, int y)
    {
        TileType tile = grid[x + size / 2 + border, y + size / 2 + border];
        if (tile != TileType.GRASS && tile != TileType.ROCK)
            return false;
        return IsFarFromCenter(x, y);
    }

    void GenerateItem(int index)
    {
        int x, y;
        do
        {
            x = Random.Range(-size / 2, size / 2);
            y = Random.Range(-size / 2, size / 2);
        } while (!CheckAvailableTile(x, y));

        GameObject item = Instantiate(
            items[index],
            tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, .25f, -2),
            Quaternion.identity,
            itemsParent.transform
        );
        item.GetComponent<HelpItem>().audioManager = audioManager;
    }

    void GenerateItem(int index, float x, float y)
    {
        GameObject item = Instantiate(
            items[index],
            new Vector3(x, y, -1),
            Quaternion.identity,
            itemsParent.transform
        );
        item.GetComponent<HelpItem>().audioManager = audioManager;
    }

    void GenerateLightItem()
    {
        int x, y;
        do
        {
            x = Random.Range(-size / 2, size / 2);
            y = Random.Range(-size / 2, size / 2);
        } while (!CheckAvailableTile(x, y));
        
        lightItem = Instantiate(
            items[3],
            tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, .25f, -2),
            Quaternion.identity
        );
        lightItem.GetComponent<HelpItem>().audioManager = audioManager;
    }
    
    void GenerateLightItem(float x, float y)
    {
        lightItem = Instantiate(
            items[3],
            new Vector3(x, y, -1),
            Quaternion.identity
        );
        lightItem.GetComponent <HelpItem>().audioManager = audioManager;
    }

    void GenerateItems()
    {
        for (int i = 0; i < itemCount; i++)
        {
            GenerateItem(Random.Range(0, 3));
        }
        GenerateLightItem();
    }

    void GenerateItems(float[,] items)
    {
        GenerateLightItem(items[0, 1], items[0, 2]);

        for (int i = 1; i < items.GetLength(0); i++)
            GenerateItem((int)items[i, 0], items[i, 1], items[i, 2]);
    }

    void GenerateEnemies(int level)
    {
        foreach (GameObject enemy in enemies)
            enemy.GetComponent<Enemy>().UpdateLevel(level);

        int x, y;
        for (int i = 0; i < enemyCount; i++)
        {
            do
            {
                x = Random.Range(-size / 2, size / 2);
                y = Random.Range(-size / 2, size / 2);
            } while (!CheckAvailableTile(x, y));

            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
            enemy.GetComponent<enemyAI>().target = player.transform;

            Instantiate(
                enemy,
                tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, .25f, -1),
                Quaternion.identity,
                enemiesParent.transform
            );
        }
    }

    void GenerateMap(int level)
    {
        tilemap.ClearAllTiles();
        trees.ClearAllTiles();
        fog.ClearAllTiles();
        do
        {
            SetOffsets();
            GenerateGrid();
            GenerateTerrain();
            GenerateTrees();
            GenerateFlowers();
            GenerateExtras();
        } while (grid[size / 2 + border, size / 2 + border] != TileType.GRASS);  // initial position
        FillTilemap();
        GenerateItems();
        GenerateLights();
        GenerateEnemies(level);
    }

    public void KillEnemies()
    {
        foreach (Transform enemy in enemiesParent.transform)
            enemy.GetComponent<Enemy>().Die();
    }

    public void KillEveryone(bool killLightItem)
    {
        KillEnemies();
        foreach (Transform item in itemsParent.transform)
            if (item.GetComponent<HelpItem>().type != HelpItem.Type.LIGHT)
                Destroy(item.gameObject);
        foreach (Transform light in lightsParent.transform)
            Destroy(light.gameObject);
        if (killLightItem)
            Destroy(lightItem);
    }

    public void CreateNewLevel(int level)
    {
        KillEveryone(false);
        GenerateMap(level);
    }

    public void RegenerateEnemies(int level)
    {
        foreach (Transform enemy in enemiesParent.transform)
            enemy.GetComponent<Enemy>().Die();
        GenerateEnemies(level);
    }

    public int[,] GetGrid()
    {
        int[,] newGrid = new int[size + 2 * border, size + 2 * border];
        for (int x = 0; x < size + 2 * border; x++)
            for (int y = 0; y < size + 2 * border; y++)
                newGrid[x, y] = (int)grid[x, y];
        return newGrid;
    }

    public float[,] GetItemInfo()
    {
        float[,] itemInfo = new float[itemsParent.transform.childCount + 1, 3];

        itemInfo[0, 0] = (int)HelpItem.Type.LIGHT;
        itemInfo[0, 1] = lightItem.transform.position.x;
        itemInfo[0, 2] = lightItem.transform.position.y;
        for (int i = 0; i < itemInfo.GetLength(0) - 1; i++)
        {
            itemInfo[i + 1, 0] = (int)itemsParent.transform.GetChild(i).GetComponent<HelpItem>().type;
            itemInfo[i + 1, 1] = itemsParent.transform.GetChild(i).position.x;
            itemInfo[i + 1, 2] = itemsParent.transform.GetChild(i).position.y;
        }

        return itemInfo;
    }

    public void LoadTilemap(int[,] grid, float[,] itemInfo, int size, int border, int level)
    {
        tilemap.ClearAllTiles();
        trees.ClearAllTiles();
        fog.ClearAllTiles();
        KillEveryone(true);
        this.size = size;
        this.border = border;
        for (int x = 0; x < size + 2 * border; x++)
            for (int y = 0; y < size + 2 * border; y++)
                this.grid[x, y] = (TileType)grid[x, y];
        FillTilemap();
        GenerateItems(itemInfo);
        GenerateLights();
        GenerateEnemies(level);
    }

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(0, int.MaxValue);
        Random.InitState(seed);
        tilemap = GetComponent<Tilemap>();
        CreateNewLevel(1);
    }
}