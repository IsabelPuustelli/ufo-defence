using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class playerController : MonoBehaviour
{
    public Tilemap map;
    public GameObject movementTile;

    // Start is called before the first frame update
    void Start()
    {
        Vector3Int currentCell = map.WorldToCell(transform.position);
        transform.position =  map.GetCellCenterWorld(currentCell);
        //var pos = transform.position; pos.y = pos.y + 0.05f; transform.position = pos;
        //initialTiles(4);
    }

    public void movementPhase()
    {
        Vector3Int testSpawn = map.WorldToCell(transform.position);
        testSpawn.y += 1; testSpawn.x += 1;
        Instantiate(movementTile, map.CellToWorld(testSpawn), Quaternion.identity);
    }

    public void initialTiles(int movementCost)
    {
        Vector3Int playerTile = map.WorldToCell(transform.position);
        Vector3Int currentTile = map.WorldToCell(transform.position);

        for (int y = movementCost; y >= 0; y--)
        {
            currentTile.x = playerTile.x;
    
            currentTile.y = playerTile.y - y; 
            if (playerTile != currentTile)
                Instantiate(movementTile, map.GetCellCenterWorld(currentTile), Quaternion.identity);
            currentTile.y = playerTile.y + y;
            if (playerTile != currentTile )
                Instantiate(movementTile, map.GetCellCenterWorld(currentTile), Quaternion.identity);

            for (int x = movementCost - y; x > 0; x--)
            {
                currentTile.x = playerTile.x - x;
                Instantiate(movementTile, map.GetCellCenterWorld(currentTile), Quaternion.identity);
                currentTile.x = playerTile.x + x;
                Instantiate(movementTile, map.GetCellCenterWorld(currentTile), Quaternion.identity);

                currentTile.y = playerTile.y - y;

                currentTile.x = playerTile.x - x;
                Instantiate(movementTile, map.GetCellCenterWorld(currentTile), Quaternion.identity);
                currentTile.x = playerTile.x + x;
                Instantiate(movementTile, map.GetCellCenterWorld(currentTile), Quaternion.identity);

                currentTile.y = playerTile.y + y;
            }
        }
    }
}
