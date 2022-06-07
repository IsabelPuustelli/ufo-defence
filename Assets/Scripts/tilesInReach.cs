using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class tilesInReach : MonoBehaviour
{
    public Tilemap map;
    private BoundsInt area;
    public GameObject movementTile;

    public List<TilePos> tilesInSquare;
    public List<TilePos> tilesInRange;
    public int movementRange;
    public Seeker seeker;
    // Start is called before the first frame update
    void Start()
    {
        // Snaps player to the center of it's tile
        Vector3Int currentCell = map.WorldToCell(transform.position);
        transform.position =  map.GetCellCenterWorld(currentCell);

        movementRange = 6;
        Vector2Int playerPos = new Vector2Int(map.WorldToCell(transform.position).x, map.WorldToCell(transform.position).y);

        tilesInRange = tileInRangeFunction(playerPos);
        spawnTilesInList(tilesInRange);
    }

    public List<TilePos> tileInRangeFunction(Vector2Int playerPos)
    {
        List<TilePos> list = new List<TilePos>();
        for (int y = playerPos.y - movementRange; y <= (playerPos).y + movementRange; y++)
        {
            for (int x = playerPos.x - movementRange; x <= (playerPos).x + movementRange; x++)
            {
                Vector3 tilePosVector = new Vector3(x, y, 0);
            //    if(onPathCalculated(seeker.StartPath(transform.position, tilePosVector)))
            //    {
                    var current = ScriptableObject.CreateInstance<TilePos>();
                    current.x = x; current.y = y;
                    list.Add(current);
            //    }
            }
        }

        return list;
    }

    public void spawnTilesInList(List<TilePos> list)
    {
        Vector2Int playerPos = new Vector2Int(map.WorldToCell(transform.position).x, map.WorldToCell(transform.position).y);

        foreach (TilePos tile in list)
        {
            Vector2 tilePosVector = new Vector2(tile.x, tile.y);
            float distance = Vector2.Distance(tilePosVector, playerPos);
            Debug.Log("Tile coordinates: " + tile.x + " " + tile.y + "        Distance: " + distance);
            Instantiate(movementTile, map.GetCellCenterWorld(new Vector3Int(tile.x, tile.y, 0)), Quaternion.identity);
        }
    }

    bool onPathCalculated (Path path)
    {
        while (path.IsDone() != true){}
        float length = path.GetTotalLength();
        if(length <= movementRange)
        {
            return true;
        }else{
            return false;
        }
    }
}