using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;
using TMPro;

public class movementPointsToReach : MonoBehaviour
{
    public Seeker seeker;
    public Tilemap map;
    public GameObject player;
    public GameObject movableTile;
    bool spawned = false;

    private TMP_Text text;
    private LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.1f; line.endWidth = 0.1f; line.startColor = Color.cyan; line.endColor = Color.blue;

        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = map.WorldToCell(mousePosition);

        transform.position = new Vector2(mousePosition.x + 2.65f, mousePosition.y - 2.1f);

        seeker.StartPath(player.transform.position, mousePosition, printPathLength);

        clickableTile();
    }

    void printPathLength(Path path)
    {
        float length = path.GetTotalLength();
        text.SetText(length.ToString());

        line.positionCount = path.vectorPath.Count;
        for (int i = 0; i < path.vectorPath.Count; i++)
            line.SetPosition (i, path.vectorPath[i]);
    }

    void clickableTile()
    {

        if (spawned == false)
            movableTile = Instantiate(movableTile, Vector3.zero, Quaternion.identity); spawned = true;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = map.WorldToCell(mousePosition);
        gridPosition = new Vector3Int(gridPosition.x + 1, gridPosition.y + 1, gridPosition.z);

        movableTile.transform.position = map.GetCellCenterWorld(gridPosition);
    }
}