using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Pathfinding;
using TMPro;
using System;

public class movementPointsToReach : MonoBehaviour
{
    bool currentActionMove;
    public double length = 0;
    public List<Vector3> movementPath;
    private List<CharacterInfo> characters;
    private Vector3 characterPos;
    private int currentCharacter;
    private Tilemap map;
    private Seeker seeker;
    private TMP_Text text;
    private LineRenderer line;
    private gameMaster gameMaster;
    private characterActions characterActions;

    void Awake()
    {
        characters = new List<CharacterInfo>();
        line = GetComponent<LineRenderer>();
        seeker = GetComponent<Seeker>();
        text = GetComponent<TMP_Text>();
        map = GameObject.Find("Floor").GetComponent<Tilemap>();
        gameMaster = transform.parent.GetComponent<gameMaster>();
        characterActions = transform.parent.GetComponent<characterActions>();
    }

    void Update()
    {
        if(currentActionMove)
        {
            // Keeps the text next to the cursor
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            Vector3 cellCenter = map.GetCellCenterWorld(gridPosition);
            cellCenter.y += 0.083f;
            transform.position = new Vector2(mousePosition.x + 2.65f, mousePosition.y - 2.1f);

            characterPos = characters[currentCharacter].characterObject.transform.position;
            characterPos.y += 0.12f;
            // Calculates path to cursor and calls printPathLength with the path object
            seeker.StartPath(characterPos, cellCenter, printPathLength);
        }
    }

    void OnEnable() 
    {
        updateCharacterList();
    }

    void printPathLength(Path path) // Checks the path length and shows is as a rounded int next to the cursor
    {                               // and then draw a line that's colored based on the length of the path
        length = 0;
        movementPath = path.vectorPath;
        line.positionCount = path.vectorPath.Count;
        line.startWidth = 0.025f; line.endWidth = 0.025f;
        for (int i = 0; i < path.vectorPath.Count; i++)
        {
            line.SetPosition (i, path.vectorPath[i]);
            
            if (i != 0)
            {
                Vector3Int first = map.WorldToCell(path.vectorPath[i - 1]);
                Vector3Int second = map.WorldToCell(path.vectorPath[i]);
                length += Math.Sqrt(Math.Pow((second.x - first.x), 2) + Math.Pow((second.y - first.y), 2));
            }
        }

        text.SetText(Math.Round(length * 10, 0).ToString());

        switch (Math.Round(length * 10, 0))
        {
            case var value when value > characters[currentCharacter].movementPointsMax: line.startColor = Color.cyan; line.endColor = Color.red; break;
            case var value when value > (characters[currentCharacter].movementPointsMax * 0.7f): line.startColor = Color.cyan; line.endColor = Color.green; break;
            default: line.startColor = Color.cyan; line.endColor = Color.blue; break;
        }
    }

    public void enableGraphics(bool enable)
    {
        if(enable)
        {
            line.enabled = true;
            text.enabled = true;
            currentActionMove = true;
        }else{
            line.enabled = false;
            text.enabled = false;
            currentActionMove = false;
        }
    }

    public void updateCharacterList()
    {
        characters = gameMaster.getCharacterList();
    }

    public void cycleCharacters()
    {
        if(!characterActions.pieceMoved)
        {
            currentCharacter++;
            if (currentCharacter == characters.Count)
                currentCharacter = 0;
            while (gameMaster.currentTurn != characters[currentCharacter].side)
            {
                currentCharacter++;
                if (currentCharacter == (characters.Count))
                    currentCharacter = 0;
            }
        }

        updatePathFinder();
    }

    public void updatePathFinder()
    {
        characterPos = characters[currentCharacter].characterObject.transform.position;
    }
}