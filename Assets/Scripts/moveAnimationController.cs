using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class moveAnimationController : MonoBehaviour
{
    private movementPointsToReach pathfinder;
    private List<Vector3> path;
    private AnimationClip clip;
    private Animation anim;
    private AnimationEvent evt = new AnimationEvent();
    public int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        pathfinder = GameObject.Find("pathPrinter").GetComponent<movementPointsToReach>();
        evt.time = 0.1f;
        evt.functionName = "moveAnimation";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void moveAnimation()
    {
        clip = new AnimationClip();
        clip.legacy = true;
        if (i == 0){path = pathfinder.movementPath;}
        if (i < path.Count)
        {
            AnimationCurve xCurve = AnimationCurve.EaseInOut(0, transform.localPosition.x, 0.1f, path[i].x);
            AnimationCurve yCurve = AnimationCurve.EaseInOut(0, transform.localPosition.y, 0.1f, (path[i].y - 0.12f));

            clip.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
            clip.SetCurve("", typeof(Transform), "localPosition.y", yCurve);

            if(i != (path.Count - 1)){clip.AddEvent(evt);}
            i++;
            
            anim.AddClip(clip, clip.name);
            anim.Play(clip.name);
        }else{i = 0;}
    }
}
