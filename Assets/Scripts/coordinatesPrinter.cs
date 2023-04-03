using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class coordinatesPrinter : MonoBehaviour
{

    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private List<TileInfo> TileInfos;

    public Dictionary<TileBase, TileInfo> dataFromTiles;

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
        /*if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousePosition);

            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            gridPosition = new Vector3Int(gridPosition.x, gridPosition.y, 0);
            Debug.Log(gridPosition);

            if (map.HasTile(gridPosition)){
                TileBase clickedTile = map.GetTile(gridPosition);
                Debug.Log(clickedTile);
            }

            //float coverLevel = dataFromTiles[clickedTile].coverLevel;

            foreach (var TileInfo in TileInfos)
            {
                foreach(var tile in TileInfo.tiles)
                {
                    //if (tile == clickedTile)
                        //Debug.Log("Clicked tile belongs to: " + TileInfo);
                }
            }
        }*/
    }

    public void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(ray.origin, ray.direction));
        Debug.Log(hits.Count);
        if (hits.Count > 0)
        {
            hits.Sort((l, r) =>
            {
                var p1 = l.transform.position;
                var p2 = r.transform.position;
                return (int)(((p1.y - p2.y) * 10 + (p2.z - p1.z)) * 10);
            });

            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log(hit.collider.tag);
            }
        }
    }
}