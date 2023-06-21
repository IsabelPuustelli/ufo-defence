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
    private Dictionary<string, Object> prefabDictionary;

    void Awake()
    {
        prefabs = Resources.LoadAll("tilePrefabs", typeof(GameObject));
        tileBases = Resources.LoadAll("tileBases", typeof(blockData));

        map = GetComponentsInChildren<Tilemap>();
    }
    void Start()
    {
        prefabDictionary = new Dictionary<string, Object>();

        foreach(Object prefab in prefabs)
        {
            prefabDictionary.Add(prefab.name, prefab);
        }
        foreach(Object tile in tileBases)
        {
            blockData block = (blockData)tile;
            block.isHidden = true;
        }
        map[0].RefreshAllTiles();
        map[1].RefreshAllTiles();
        foreach(Object tile in tileBases)
        {
            blockData block = (blockData)tile;
            block.isHidden = false;
        }
    }

    public GameObject findPrefab(TileBase tile)
    {
        return (GameObject)prefabDictionary[tile.name];
    }
}
