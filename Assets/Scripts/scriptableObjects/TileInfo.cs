using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "Tile Group", menuName = "Tile Group")]
public class TileInfo : ScriptableObject
{
    public TileBase[] tiles;

    public float coverLevel;
}
