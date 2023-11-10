using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class tileMapManager : MonoBehaviour
{
    private Tilemap floor;
    private Tilemap cover;
    private Tilemap fog;
    private Tilemap anchors;
    public Dictionary<Vector2Int, TileBase> tileSetFloor;
    public Dictionary<Vector2Int, TileBase> tileSetCover;
    public Dictionary<TileBase, List<Vector2Int>> anchorTiles;
    List <Vector2Int> laidTiles = new();
    List <Vector2Int> tilesInSight = new();
    List<TileBase> tilesList = new();
    List<Vector3Int> tilesLoc = new();
    private AnimationCurve curve;
    private AnimationClip clip;
    private int i;
    private tileLibrary tileLibrary;
    private gameMaster gameMaster;
    private characterActions characterActions;
    bool animStarted = false;
    
    void Start()
    {
        floor = GameObject.Find("Floor").GetComponent<Tilemap>();
        cover = GameObject.Find("Cover").GetComponent<Tilemap>();
        fog = GameObject.Find("fogOfWar").GetComponent<Tilemap>();
        anchors = GameObject.Find("roomAnchors").GetComponent<Tilemap>();
        floor.CompressBounds(); cover.CompressBounds(); fog.CompressBounds(); anchors.CompressBounds();

        i = 0;
        tileLibrary = GetComponent<tileLibrary>();
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
        characterActions = GameObject.Find("GameMaster").GetComponent<characterActions>();
        
        clip = new AnimationClip();
        clip.legacy = true;
        //AnimationEvent spwn = new AnimationEvent(); spwn.time = 0.005f; spwn.functionName = "spawnNextBlock";
        AnimationEvent evt = new AnimationEvent(); evt.time = 0.4f; evt.functionName = "selfDestruct";
        clip.AddEvent(evt); //clip.AddEvent(spwn);

        tileSetFloor = new Dictionary<Vector2Int, TileBase>();
        tileSetCover = new Dictionary<Vector2Int, TileBase>();
        anchorTiles = new Dictionary<TileBase, List<Vector2Int>>();

        BoundsInt bounds = floor.cellBounds;
        TileBase[] allTiles = floor.GetTilesBlock(bounds);

        GameObject obj = new();
        TextMeshPro text = obj.AddComponent(typeof(TextMeshPro)) as TextMeshPro;
        text.fontSize = 0.2f;
        text.alignment = TextAlignmentOptions.Center;

        foreach(Object tile in tileLibrary.anchorTiles)
        {
            blockData block = (blockData)tile;
            anchorTiles.Add(block, new List<Vector2Int>());
        }

        for (int x = floor.cellBounds.xMin; x < floor.cellBounds.xMax; x++) {
            for (int y = floor.cellBounds.yMin; y < floor.cellBounds.yMax; y++) 
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
                if (anchors.GetTile(new Vector3Int(x, y, 0)) != null)
                    anchorTiles[anchors.GetTile(new Vector3Int(x, y, 0))].Add(new Vector2Int(x, y));
            }
        }
    }

    public IEnumerator blockSpawner (Vector2 collision)
    {
        if (!animStarted)
        {
            characterActions.restrictAction(true);
            animStarted = true;

            var tilesToLay = anchorTiles[anchors.GetTile(anchors.WorldToCell(collision))];
            laidTiles.AddRange(tilesToLay);
            foreach(Vector2Int pos in tilesToLay)
            {

                Vector3 worldLoc = floor.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));

                Keyframe one = new Keyframe(0, worldLoc.y + 0.3f);
                Keyframe two = new Keyframe(0.4f, worldLoc.y);
                curve = new AnimationCurve(one, two);
                clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
                clip.SetCurve("", typeof(Transform), "localPosition.x", new AnimationCurve(new Keyframe(0, worldLoc.x)));

                if (tileSetFloor.ContainsKey(pos))
                {
                    var block = Instantiate(tileLibrary.findPrefab(tileSetFloor[pos]), new Vector3(worldLoc.x, worldLoc.y + 0.3f, worldLoc.z), Quaternion.identity, GameObject.Find("Floor").transform);
                    block.GetComponent<Animation>().AddClip(clip, "clip");
                    block.GetComponent<Animation>().Play("clip");
                }
                if (tileSetCover.ContainsKey(pos))
                {
                    var block = Instantiate(tileLibrary.findPrefab(tileSetCover[pos]), new Vector3(worldLoc.x, worldLoc.y + 0.3f, worldLoc.z), Quaternion.identity, GameObject.Find("Cover").transform);
                    block.GetComponent<Animation>().AddClip(clip, "clip");
                    block.GetComponent<Animation>().Play("clip");
                }
                anchors.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
                yield return null;
            }
            animStarted = false;
            characterActions.restrictAction(false);
            StopCoroutine(blockSpawner(collision));
        }
    }

    public void fogOfWar(int range, Vector2 mousePos)
    {
        Vector3 tileToEvaluate = new Vector3();
        List <Vector2> postions = new List<Vector2>();
        foreach (CharacterInfo character in gameMaster.getCharacterList())
        {
            if (character != gameMaster.getCurrentCharacter())
                postions.Add(new Vector2(character.characterObject.transform.position.x, character.characterObject.transform.position.y + 0.12f));
        }
        postions.Add(new Vector2(mousePos.x, mousePos.y + 0.12f));
        tilesInSight.Clear();
        foreach(Vector2 position in postions)
        {
            int y = 0;
            Vector3Int centerCell = floor.WorldToCell(position);
            centerCell.y++; centerCell.x++;
            for (int x = range; x >= -range; x--)
            {
                tileToEvaluate = floor.GetCellCenterWorld(new Vector3Int(centerCell.x + x, centerCell.y)); tileToEvaluate.y += 0.083f;
                if (!tilesInSight.Contains(new Vector2Int(centerCell.x + x, centerCell.y)) && 
                    !Physics2D.Raycast(position, tileToEvaluate - new Vector3(position.x, position.y),
                    Vector3.Distance(position, tileToEvaluate),  LayerMask.GetMask("Cover")))
                {
                    tilesInSight.Add(new Vector2Int(centerCell.x + x, centerCell.y));
                }
                if (x >= 0){y = range - x;}
                else{y = range + x;}
                while(y > 0)
                {
                    tileToEvaluate = floor.GetCellCenterWorld(new Vector3Int(centerCell.x + x, centerCell.y - y)); tileToEvaluate.y += 0.083f;
                    if (!tilesInSight.Contains(new Vector2Int(centerCell.x + x, centerCell.y - y)) && 
                        !Physics2D.Raycast(position, tileToEvaluate - new Vector3(position.x, position.y),
                        Vector3.Distance(position, tileToEvaluate),  LayerMask.GetMask("Cover")))
                    {
                        tilesInSight.Add(new Vector2Int(centerCell.x + x, centerCell.y - y));
                    }
                    tileToEvaluate = floor.GetCellCenterWorld(new Vector3Int(centerCell.x + x, centerCell.y + y)); tileToEvaluate.y += 0.083f;
                    if (!tilesInSight.Contains(new Vector2Int(centerCell.x + x, centerCell.y + y)) && 
                        !Physics2D.Raycast(position, tileToEvaluate - new Vector3(position.x, position.y),
                        Vector3.Distance(position, tileToEvaluate),  LayerMask.GetMask("Cover")))
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