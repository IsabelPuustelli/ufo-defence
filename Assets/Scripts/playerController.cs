using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class playerController : MonoBehaviour
{
    private Tilemap map;
    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.Find("Cover").GetComponent<Tilemap>();
        Vector3Int currentCell = map.WorldToCell(transform.position);
        transform.position =  map.GetCellCenterWorld(currentCell);
    }
}
