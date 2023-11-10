using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class moveAnimationController : MonoBehaviour
{
    private Tilemap map;
    private Tilemap anchors;
    private movementPointsToReach pathfinder;
    private tileMapManager mapManager;
    private gameMaster gameMaster;
    private characterActions characterActions;
    private List<Vector3> path;
    private AnimationClip clip;
    private AnimationClip spawnClip;
    private Animation anim;
    private AnimationEvent evt = new AnimationEvent();
    private AnimationEvent nextSpawn = new AnimationEvent();
    public int i;
    public bool stopAnimation = false;

    void Awake() 
    {
        spawnClip = new AnimationClip();
        spawnClip.legacy = true;

        evt.time = 0.1f;
        evt.functionName = "moveAnimation";

        nextSpawn.time = 0.5f;
        nextSpawn.functionName = "nextChar";
        
        spawnClip.AddEvent(nextSpawn);

        anim = GetComponent<Animation>();
        pathfinder = GameObject.Find("pathPrinter").GetComponent<movementPointsToReach>();
        mapManager = GameObject.Find("Grid").GetComponent<tileMapManager>();
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
        characterActions = GameObject.Find("GameMaster").GetComponent<characterActions>();
        map = GameObject.Find("Floor").GetComponent<Tilemap>();
        anchors = GameObject.Find("roomAnchors").GetComponent<Tilemap>();
    }

    public void moveAnimation()
    {
        if (stopAnimation)
        {
            characterActions.restrictAction(false);
            transform.position =  map.GetCellCenterWorld(map.WorldToCell(transform.position));
            characterActions.walking = false;
            i = 0;
        }
        else
        {
            clip = new AnimationClip();
            clip.legacy = true;
            if (i == 0){path = pathfinder.movementPath;}
            if (i < path.Count)
            {
                var pos = new Vector3(transform.position.x, transform.position.y, 0);
                if (anchors.GetTile(anchors.WorldToCell(pos)) != null)
                    StartCoroutine(mapManager.blockSpawner(pos));

                AnimationCurve xCurve = AnimationCurve.EaseInOut(0, transform.localPosition.x, 0.1f, path[i].x);
                AnimationCurve yCurve = AnimationCurve.EaseInOut(0, transform.localPosition.y, 0.1f, (path[i].y - 0.15f));
                AnimationCurve zCurve = AnimationCurve.Linear(0, transform.localPosition.z, 0.1f, transform.localPosition.z);

                clip.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
                clip.SetCurve("", typeof(Transform), "localPosition.y", yCurve);
                clip.SetCurve("", typeof(Transform), "localPosition.z", zCurve); 
                clip.AddEvent(evt);
                i++;

                anim.AddClip(clip, clip.name);
                anim.Play(clip.name);
            }else{
                var pos = new Vector3(transform.position.x, transform.position.y, 0);
                if (anchors.GetTile(anchors.WorldToCell(pos)) != null)
                    StartCoroutine(mapManager.blockSpawner(pos));
                i = 0;
                transform.position =  map.GetCellCenterWorld(map.WorldToCell(transform.position));
                characterActions.walking = false;
                characterActions.restrictAction(false);
            }
        }
    }

    public void spawnAnimation(Vector3Int pos)
    {
        AnimationCurve xCurve = AnimationCurve.Linear(0, map.GetCellCenterWorld(pos).x, 0f, map.GetCellCenterWorld(pos).x);
        AnimationCurve yCurve = AnimationCurve.EaseInOut(0, map.GetCellCenterWorld(pos).y + 1f, 1f, map.GetCellCenterWorld(pos).y + 0.1f);
        AnimationCurve zCurve = AnimationCurve.Linear(0, map.GetCellCenterWorld(pos).z, 0f, map.GetCellCenterWorld(pos).z);

        spawnClip.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
        spawnClip.SetCurve("", typeof(Transform), "localPosition.y", yCurve);
        spawnClip.SetCurve("", typeof(Transform), "localPosition.z", zCurve);

        anim.AddClip(spawnClip, spawnClip.name);
        anim.Play(spawnClip.name);
    }

    public void nextChar()
    {
        gameMaster.spawnLoop();
    }
}
