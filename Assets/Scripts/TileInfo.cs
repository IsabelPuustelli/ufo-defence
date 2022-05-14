using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "Tile", menuName = "Tile")]
public class TileInfo : ScriptableObject
{
    public TileBase[] tiles;

    public float coverLevel;
}
