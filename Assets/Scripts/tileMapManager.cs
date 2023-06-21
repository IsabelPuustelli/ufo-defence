using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class tileMapManager : MonoBehaviour
{
    private Tilemap floor;
    private Tilemap cover;
    private Tilemap fog;
    public Dictionary<Vector2Int, TileBase> tileSetFloor;
    public Dictionary<Vector2Int, TileBase> tileSetCover;
    public Transform floorTile;
    List<TileBase> tilesList = new List<TileBase>();
    List<Vector3Int> tilesLoc = new List<Vector3Int>();
    private AnimationCurve curve;
    private AnimationClip clip;
    private int i;
    private tileLibrary tileLibrary;
    List<Vector3Int> tilesInSight = new List<Vector3Int>();

    void Start()
    {
        i = 0;
        tileLibrary = GetComponent<tileLibrary>();
        
        clip = new AnimationClip();
        clip.legacy = true;
        AnimationEvent spwn = new AnimationEvent(); spwn.time = 0.01f; spwn.functionName = "spawnNextBlock";
        AnimationEvent evt = new AnimationEvent(); evt.time = 0.5f; evt.functionName = "selfDestruct";
        clip.AddEvent(evt); clip.AddEvent(spwn);

        tileSetFloor = new Dictionary<Vector2Int, TileBase>();
        tileSetCover = new Dictionary<Vector2Int, TileBase>();

        
        floor = GameObject.Find("Floor").GetComponent<Tilemap>();
        cover = GameObject.Find("Cover").GetComponent<Tilemap>();
        fog = GameObject.Find("fogOfWar").GetComponent<Tilemap>();
        BoundsInt bounds = floor.cellBounds;
        TileBase[] allTiles = floor.GetTilesBlock(bounds);

        for (int x = -27; x < 0; x++) {
            for (int y = -45; y < 0; y++) 
            {
                if (floor.GetTile(new Vector3Int(x, y, 0)) != null)
                    tileSetFloor.Add(new Vector2Int(x, y), floor.GetTile(new Vector3Int(x, y, floor.origin.z)));
                    
                if (cover.GetTile(new Vector3Int(x, y, 0)) != null)
                    tileSetCover.Add(new Vector2Int(x, y), cover.GetTile(new Vector3Int(x, y, cover.origin.z)));
            }
        }
    }
    
    public void revealArea(Vector2 center, int range)
    {
        Vector3Int centerCell = floor.WorldToCell(center);
        int y = 0;
        tilesList.Clear(); tilesLoc.Clear();

        for (int x = range; x >= -range; x--)
        {
            if (tileSetFloor.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y)))
            {
                tilesList.Add(floor.GetTile(new Vector3Int(centerCell.x + x, centerCell.y, 0)));
                tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y, 0));
                tileSetFloor.Remove(new Vector2Int(centerCell.x + x, centerCell.y));
            }else if(tileSetCover.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y)))
            {
                tilesList.Add(cover.GetTile(new Vector3Int(centerCell.x + x, centerCell.y, 0)));
                tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y, 0));
                tileSetCover.Remove(new Vector2Int(centerCell.x + x, centerCell.y));
            }

            if (x >= 0){y = range - x;}
            else{y = range + x;}
            while(y > 0)
            {
                if (tileSetFloor.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y + y)))
                {
                    tilesList.Add(floor.GetTile(new Vector3Int(centerCell.x + x, centerCell.y + y, 0)));
                    tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y + y, 0));
                    tileSetFloor.Remove(new Vector2Int(centerCell.x + x, centerCell.y + y));
                }else if(tileSetCover.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y + y)))
                {
                    tilesList.Add(cover.GetTile(new Vector3Int(centerCell.x + x, centerCell.y + y, 0)));
                    tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y + y, 0));
                    tileSetCover.Remove(new Vector2Int(centerCell.x + x, centerCell.y + y));
                }
                if (tileSetFloor.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y - y)))
                {
                    tilesList.Add(floor.GetTile(new Vector3Int(centerCell.x + x, centerCell.y - y, 0)));
                    tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y - y, 0));
                    tileSetFloor.Remove(new Vector2Int(centerCell.x + x, centerCell.y - y));
                }else if(tileSetCover.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y - y)))
                {
                    tilesList.Add(cover.GetTile(new Vector3Int(centerCell.x + x, centerCell.y - y, 0)));
                    tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y - y, 0));
                    tileSetCover.Remove(new Vector2Int(centerCell.x + x, centerCell.y - y));
                }
                y--;
            }
        }
        i = 0;
        blockSpawner();
    }

    public void blockSpawner()
    {
        if (i != -1)
        {
            Vector3 worldLoc = floor.GetCellCenterWorld(tilesLoc[i]);
            Keyframe one = new Keyframe(0, worldLoc.y + 0.3f);
            Keyframe two = new Keyframe(0.5f, worldLoc.y);
            curve = new AnimationCurve(one, two);
            clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
            clip.SetCurve("", typeof(Transform), "localPosition.x", new AnimationCurve(new Keyframe(0, worldLoc.x)));
            var block = Instantiate(tileLibrary.findPrefab(tilesList[i]), new Vector3(worldLoc.x, worldLoc.y + 0.3f, worldLoc.z), Quaternion.identity, GameObject.Find("Cover").transform);
            i++;
            if (i == tilesLoc.Count - 1)
                i = -1;
            
            block.GetComponent<Animation>().AddClip(clip, "clip");
            block.GetComponent<Animation>().Play("clip");
        }
    }

    public void fogOfWar(Vector3 location)
    {
        float x; float y;
        tilesInSight.Clear();
        for(int i = 0; i <= 360; i += 10)
        {
            x = 2 * Mathf.Cos(i);
            y = Mathf.Sin(i);
            Vector2 direction = new Vector2(x, y);

            RaycastHit2D hit = Physics2D.Raycast(location, direction, Mathf.Infinity, LayerMask.GetMask("Floor"));
            var point = hit.point;
            if(!tilesInSight.Contains(fog.WorldToCell(hit.point)))
                tilesInSight.Add(fog.WorldToCell(hit.point));
            
            for(int p = 0; p <= 10; p++)
            {
                hit = Physics2D.Raycast(point + (direction / 10f), direction, Mathf.Infinity, LayerMask.GetMask("Floor"));
                if(!tilesInSight.Contains(fog.WorldToCell(hit.point)))
                    tilesInSight.Add(fog.WorldToCell(hit.point));
                point = hit.point;
            }
        }
        Debug.Log(tilesInSight.Count);
        foreach(Vector3Int tiles in tilesInSight)
        {
            Debug.Log(tiles);
        }
    }
}
