using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class moveAnimationController : MonoBehaviour
{
    private Tilemap map;
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

        nextSpawn.time = 1.5f;
        nextSpawn.functionName = "nextChar";
        
        spawnClip.AddEvent(nextSpawn);

        anim = GetComponent<Animation>();
        pathfinder = GameObject.Find("pathPrinter").GetComponent<movementPointsToReach>();
        mapManager = GameObject.Find("Grid").GetComponent<tileMapManager>();
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
        characterActions = GameObject.Find("GameMaster").GetComponent<characterActions>();
        map = GameObject.Find("Floor").GetComponent<Tilemap>();
    }

    public void moveAnimation()
    {
        if (!stopAnimation)
        {
            clip = new AnimationClip();
            clip.legacy = true;
            if (i == 0){path = pathfinder.movementPath;}
            if (i < path.Count)
            {
                AnimationCurve xCurve = AnimationCurve.EaseInOut(0, transform.localPosition.x, 0.1f, (path[i].x + 0.02f));
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
                i = 0;
                transform.position =  map.GetCellCenterWorld(map.WorldToCell(transform.position));
                characterActions.walking = false;
            }
        }
    }

    public void spawnAnimation()
    {
        AnimationCurve xCurve = AnimationCurve.Linear(0, transform.localPosition.x + 0f, 0f, transform.localPosition.x + 0f);
        AnimationCurve yCurve = AnimationCurve.EaseInOut(0, transform.localPosition.y + 1f, 1.5f, transform.localPosition.y - 0.20f);

        spawnClip.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
        spawnClip.SetCurve("", typeof(Transform), "localPosition.y", yCurve);

        anim.AddClip(spawnClip, spawnClip.name);
        anim.Play(spawnClip.name);
    }

    public void nextChar()
    {
        gameMaster.spawnLoop();
    }
}
