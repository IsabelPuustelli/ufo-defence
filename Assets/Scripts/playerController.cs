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
        var pos = transform.position; pos.y = pos.y + 0.05f; transform.position = pos;
        movementPhase();
    }

    public void movementPhase()
    {
        Debug.Log(map.WorldToCell(transform.position));
        Vector3Int testSpawn = map.WorldToCell(transform.position);
        testSpawn.y += 1; testSpawn.x += 1;
        Debug.Log(testSpawn);
        Instantiate(movementTile, map.CellToWorld(testSpawn), Quaternion.identity);
    }
}
