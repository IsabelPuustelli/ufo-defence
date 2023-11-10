using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tileAnimation : MonoBehaviour
{
    public void selfDestruct()
    {
        var map = GameObject.Find("Floor").GetComponent<Tilemap>();
        var map1 = GameObject.Find("Cover").GetComponent<Tilemap>();
        map.RefreshTile(map.WorldToCell(transform.position));
        map1.RefreshTile(map.WorldToCell(transform.position));
        Destroy(gameObject);
    }
}