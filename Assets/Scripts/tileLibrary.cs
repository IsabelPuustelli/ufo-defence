using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tileLibrary : MonoBehaviour
{
    public blockData bd;
    private Tilemap[] map;
    private Object[] prefabs;
    private Object[] tileBases;
    public Object[] anchorTiles;
    private Dictionary<string, Object> prefabDictionary;
    private Dictionary<string, blockData> blockDictionary;

    void Awake()
    {
        prefabs = Resources.LoadAll("tilePrefabs", typeof(GameObject));
        tileBases = Resources.LoadAll("tileBases", typeof(blockData));
        anchorTiles = Resources.LoadAll("anchorTiles", typeof(blockData));

        map = GetComponentsInChildren<Tilemap>();
    }
    void Start()
    {
        prefabDictionary = new Dictionary<string, Object>();
        blockDictionary = new Dictionary<string, blockData>();

        foreach(Object prefab in prefabs)
        {
            prefabDictionary.Add(prefab.name, prefab);
        }
        foreach(Object tile in tileBases)
        {
            blockData block = (blockData)tile;
            blockDictionary.Add(block.name, block);
            block.isHidden = true;
        }
        foreach(Object tile in anchorTiles)
        {
            blockData block = (blockData)tile;
            block.isHidden = true;
        }
        map[0].RefreshAllTiles();
        map[1].RefreshAllTiles();
        map[2].RefreshAllTiles();
        map[3].RefreshAllTiles();
        foreach(Object tile in tileBases)
        {
            blockData block = (blockData)tile;
            block.isHidden = false;
        }
    }

    public void setFog (List <Vector2Int> fogLoc)
    {
        blockDictionary["fog"].isHidden = false;
        foreach (Vector2Int loc in fogLoc)
        {
            map[2].RefreshTile(new Vector3Int(loc.x, loc.y, 0));
        }
    }
    public void removeFog (List <Vector2Int> fogLoc)
    {
        blockDictionary["fog"].isHidden = true;
        foreach (Vector2Int loc in fogLoc)
        {
            map[2].RefreshTile(new Vector3Int(loc.x, loc.y, 0));
        }
    }

    public GameObject findPrefab(TileBase tile)
    {
        return (GameObject)prefabDictionary[tile.name];
    }
}
