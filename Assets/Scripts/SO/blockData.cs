using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New floorBlock", menuName = "Tiles/floorBlock")]
public class blockData : TileBase
{
    public bool isHidden = false;
    public Sprite m_DefaultSprite;
    public Sprite m_hiddenSprite;
    public Tile.ColliderType m_DefaultColliderType;
 
    public override void GetTileData(Vector3Int cell, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = isHidden ? m_hiddenSprite : m_DefaultSprite;
        tileData.colliderType = this.m_DefaultColliderType;
    }
}