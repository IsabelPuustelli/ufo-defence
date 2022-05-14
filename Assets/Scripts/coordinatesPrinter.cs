using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class coordinatesPrinter : MonoBehaviour
{

    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private List<TileInfo> TileInfos;

    private Dictionary<TileBase, TileInfo> dataFromTiles;

    private void Awake() 
    {
        dataFromTiles = new Dictionary<TileBase, TileInfo>();

        foreach (var TileInfo in TileInfos)
        {
            foreach (var tile in TileInfo.tiles)
            {
                dataFromTiles.Add(tile, TileInfo);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            TileBase clickedTile = map.GetTile(gridPosition);

            float coverLevel = dataFromTiles[clickedTile].coverLevel;

            print ("Coordinates: " + clickedTile + "   Cover level: " + coverLevel);
        }
    }
}
