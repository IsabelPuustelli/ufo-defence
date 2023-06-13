using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tileMapManager : MonoBehaviour
{
    private Tilemap[] map;
    public Dictionary<Vector2Int, TileBase> tileSetFloor;
    public Dictionary<Vector2Int, TileBase> tileSetCover;
    public Transform floorTile;
    public TileBase emptyTile;
    private AnimationCurve curve;
    private AnimationClip clip;

    void Start()
    {
        clip = new AnimationClip();
        clip.legacy = true;
        tileSetFloor = new Dictionary<Vector2Int, TileBase>();
        map = GetComponentsInChildren<Tilemap>();
        BoundsInt bounds = map[0].cellBounds;
        TileBase[] allTiles = map[0].GetTilesBlock(bounds);

        for (int x = -27; x < 0; x++) {
            for (int y = -45; y < 0; y++) 
            {
                if (map[0].GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    tileSetFloor.Add(new Vector2Int(x, y), map[0].GetTile(new Vector3Int(x, y, map[0].origin.z)));
                }
                    
                //if (map[1].GetTile(new Vector3Int(x, y, 0)) != null)
                //    tileSetCover.Add(new Vector2Int(x, y), map[1].GetTile(new Vector3Int(x, y, map[1].origin.z)));
            }
        }
    }
    
    public void revealArea(Vector2 center, int range)
    {
        Vector3Int centerCell = map[0].WorldToCell(center);
        int y = 0; int i = 0;
        List<TileBase> tilesList = new List<TileBase>();
        List<Vector3Int> tilesLoc = new List<Vector3Int>();
        tilesList.Clear(); tilesLoc.Clear();

        for (int x = range; x >= -range; x--)
        {
            Debug.Log(x);
            if (tileSetFloor.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y)))
            {
                tilesList.Add(map[0].GetTile(new Vector3Int(centerCell.x + x, centerCell.y, 0)));
                tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y, 0));
            }

            if (x >= 0){y = range - x;}
            else{y = range + x;}
            while(y > 0)
            {
                if (tileSetFloor.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y + y)))
                {
                    tilesList.Add(map[0].GetTile(new Vector3Int(centerCell.x + x, centerCell.y + y, 0)));
                    tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y, 0));
                }
                if (tileSetFloor.ContainsKey(new Vector2Int(centerCell.x + x, centerCell.y - y)))
                {
                    tilesList.Add(map[0].GetTile(new Vector3Int(centerCell.x + x, centerCell.y - y, 0)));
                    tilesLoc.Add(new Vector3Int(centerCell.x + x, centerCell.y, 0));
                    //var block = Instantiate(floorTile, map[0].GetCellCenterWorld(new Vector3Int(centerCell.x + x, centerCell.y - y, map[0].origin.z)), Quaternion.identity, GameObject.Find("animationLayer").transform);
                    //map[0].RefreshTile(new Vector3Int(centerCell.x + x, centerCell.y - y, map[0].origin.z));
                }
                y--;
            }
        }
        for (int j = 0; j < tilesList.Count; i++)
        {
            Keyframe one = new Keyframe(0, tilesLoc[i].y + 0.3f);
            Keyframe two = new Keyframe(0.5f, tilesLoc[i].y);
            curve = new AnimationCurve(one, two);
            clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
            AnimationEvent evt = new AnimationEvent(); evt.time = 0.5f; evt.functionName = "selfDestruct";
            clip.AddEvent(evt);
            var block = Instantiate(floorTile, map[0].GetCellCenterWorld(tilesLoc[j]), Quaternion.identity, GameObject.Find("Cover").transform);
            block.GetComponent<Animation>().AddClip(clip, "clip");
            block.GetComponent<Animation>().Play("clip");
        }
    }
}
