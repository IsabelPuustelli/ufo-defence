using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tileLibrary : MonoBehaviour
{
    public blockData bd;
    public Tilemap map;

    void Awake()
    {
        bd.isHidden = true;
        map.RefreshAllTiles();
        bd.isHidden = false;
    }
}
