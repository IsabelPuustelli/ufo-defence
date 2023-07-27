using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class tileMapManager : MonoBehaviour
{
    private Tilemap floor;
    private Tilemap cover;
    private Tilemap fog;
    public Dictionary<Vector2Int, TileBase> tileSetFloor;
    public Dictionary<Vector2Int, TileBase> tileSetCover;
    List <Vector2Int> laidTiles = new List<Vector2Int>();
    List <Vector2Int> tilesInSight = new List<Vector2Int>();
    List<TileBase> tilesList = new List<TileBase>();
    List<Vector3Int> tilesLoc = new List<Vector3Int>();
    private AnimationCurve curve;
    private AnimationClip clip;
    private int i;
    private tileLibrary tileLibrary;
    private gameMaster gameMaster;

    void Start()
    {
        i = 0;
        tileLibrary = GetComponent<tileLibrary>();
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
        
        clip = new AnimationClip();
        clip.legacy = true;
        AnimationEvent spwn = new AnimationEvent(); spwn.time = 0.005f; spwn.functionName = "spawnNextBlock";
        AnimationEvent evt = new AnimationEvent(); evt.time = 0.4f; evt.functionName = "selfDestruct";
        clip.AddEvent(evt); clip.AddEvent(spwn);

        tileSetFloor = new Dictionary<Vector2Int, TileBase>();
        tileSetCover = new Dictionary<Vector2Int, TileBase>();

        floor = GameObject.Find("Floor").GetComponent<Tilemap>();
        cover = GameObject.Find("Cover").GetComponent<Tilemap>();
        fog = GameObject.Find("fogOfWar").GetComponent<Tilemap>();
        BoundsInt bounds = floor.cellBounds;
        TileBase[] allTiles = floor.GetTilesBlock(bounds);

        GameObject obj = new GameObject();
        TextMeshPro text = obj.AddComponent(typeof(TextMeshPro)) as TextMeshPro;
        text.fontSize = 0.2f;
        text.alignment = TextAlignmentOptions.Center;

        for (int x = -27; x < 0; x++) {
            for (int y = -45; y < 0; y++) 
            {
                if (floor.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    tileSetFloor.Add(new Vector2Int(x, y), floor.GetTile(new Vector3Int(x, y, floor.origin.z)));

                    text.text = x + " " + y;
                    var cords = Instantiate(obj, floor.GetCellCenterWorld(new Vector3Int(x, y, 0)), Quaternion.identity, GameObject.Find("Coordinates").transform);
                    var temp = cords.transform.position;
                    temp.y += 0.08f;
                    cords.transform.position = temp;
                }
                    
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
                laidTiles.Add(new Vector2Int(centerCell.x + x, centerCell.y));
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
                    laidTiles.Add(new Vector2Int(centerCell.x + x, centerCell.y + y));
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
                    laidTiles.Add(new Vector2Int(centerCell.x + x, centerCell.y - y));
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
        if (tilesLoc.Count > 0)
            blockSpawner();
    }

    public void blockSpawner()
    {
        if (i != -1)
        {
            Vector3 worldLoc = floor.GetCellCenterWorld(tilesLoc[i]);
            Keyframe one = new Keyframe(0, worldLoc.y + 0.3f);
            Keyframe two = new Keyframe(0.4f, worldLoc.y);
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

    public void fogOfWar(int range, Vector2 mousePos)
    {
        Vector3 destination = new Vector3();
        List <Vector2> postions = new List<Vector2>();
        foreach (CharacterInfo character in gameMaster.getCharacterList())
        {
            if (character != gameMaster.getCurrentCharacter())
                postions.Add(character.characterObject.transform.position);
        }
        postions.Add(mousePos);
        tilesInSight.Clear();
        foreach(Vector2 position in postions)
        {
            int y = 0;
            Vector3Int centerCell = floor.WorldToCell(position);
            centerCell.y++; centerCell.x++;
            for (int x = range; x >= -range; x--)
            {
                destination = floor.GetCellCenterWorld(new Vector3Int(centerCell.x + x, centerCell.y)); destination.y += 0.083f;
                if (!tilesInSight.Contains(new Vector2Int(centerCell.x + x, centerCell.y)) && 
                    !Physics2D.Raycast(position, destination - new Vector3(position.x, position.y),
                    Vector3.Distance(position, destination),  LayerMask.GetMask("Cover")))
                {
                    tilesInSight.Add(new Vector2Int(centerCell.x + x, centerCell.y));
                }
                if (x >= 0){y = range - x;}
                else{y = range + x;}
                while(y > 0)
                {
                    destination = floor.GetCellCenterWorld(new Vector3Int(centerCell.x + x, centerCell.y - y)); destination.y += 0.083f;
                    if (!tilesInSight.Contains(new Vector2Int(centerCell.x + x, centerCell.y - y)) && 
                        !Physics2D.Raycast(position, destination - new Vector3(position.x, position.y),
                        Vector3.Distance(position, destination),  LayerMask.GetMask("Cover")))
                    {
                        tilesInSight.Add(new Vector2Int(centerCell.x + x, centerCell.y - y));
                    }
                    destination = floor.GetCellCenterWorld(new Vector3Int(centerCell.x + x, centerCell.y + y)); destination.y += 0.083f;
                    if (!tilesInSight.Contains(new Vector2Int(centerCell.x + x, centerCell.y + y)) && 
                        !Physics2D.Raycast(position, destination - new Vector3(position.x, position.y),
                        Vector3.Distance(position, destination),  LayerMask.GetMask("Cover")))
                    {
                        tilesInSight.Add(new Vector2Int(centerCell.x + x, centerCell.y + y));
                    }
                    y--;
                }
            }
        }
        tileLibrary.setFog(laidTiles);
        tileLibrary.removeFog(tilesInSight);
    }
}