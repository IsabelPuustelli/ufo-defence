using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New tile", menuName = "Custom Tile")]
public class blockData : TileBase
{
    public bool isHidden = true;
    public bool inFog = false;
    public Sprite m_DefaultSprite;
    public Sprite m_hiddenSprite;
    public Sprite m_fogSprite;
    public Tile.ColliderType m_DefaultColliderType;
 
    public override void GetTileData(Vector3Int cell, ITilemap tilemap, ref TileData tileData)
    {
        if(isHidden)
            tileData.sprite = m_hiddenSprite;
        else if (inFog)
            tileData.sprite = m_fogSprite;
        else
            tileData.sprite = m_DefaultSprite;

        //tileData.sprite = isHidden ? m_hiddenSprite : m_DefaultSprite;
        tileData.colliderType = this.m_DefaultColliderType;
    }
}