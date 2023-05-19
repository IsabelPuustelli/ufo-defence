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
    public bool currentActionMove;
    private List<CharacterInfo> characters;
    public Vector3 characterPos;
    private int currentCharacter;
    public GameObject movableTile;
    bool tileEnabled = false;
    public double length = 0;

    private Tilemap map;
    private Seeker seeker;
    private TMP_Text text;
    private LineRenderer line;
    private gameMaster gameMaster;
    public Transform playableCharacters;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        seeker = GetComponent<Seeker>();
        text = GetComponent<TMP_Text>();
        map = GameObject.Find("Floor").GetComponent<Tilemap>();
        gameMaster = transform.parent.GetComponent<gameMaster>();
        movableTile = Instantiate(movableTile, Vector3.zero, Quaternion.identity);
        movableTile.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentActionMove == true)
        {
            // Keeps the text next to the cursor
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            transform.position = new Vector2(mousePosition.x + 2.65f, mousePosition.y - 2.1f);

            Vector3 fixedCharacterPos = new Vector3(characterPos.x, characterPos.y + 0.12f, characterPos.z);
            // Calculates path to cursor and calls printPathLength with the path object
            seeker.StartPath(fixedCharacterPos, new Vector3(movableTile.transform.position.x, movableTile.transform.position.y + 0.10f, movableTile.transform.position.z), printPathLength);
            clickableTile();
        }
    }

    void OnEnable() 
    {
        updateCharacterList();
    }

    void printPathLength(Path path) // Checks the path length and shows is as a rounded int next to the cursor
    {                               // and then draw a line that's colored based on the length of the path
        //float length = path.GetTotalLength();
        //int roundedLength = (int)Math.Round(length * 100, 0);
        //text.SetText(roundedLength.ToString());

        length = 0;

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

        switch (length)
        {
            case > 6: line.startColor = Color.cyan; line.endColor = Color.red; break;
            case > 3: line.startColor = Color.cyan; line.endColor = Color.green; break;
            default: line.startColor = Color.cyan; line.endColor = Color.blue; break;
        }
    }

    public void updateCharacterList()
    {
        characters = gameMaster.getCharacterList();
    }

    void clickableTile()    // Sets movableTile active if not already and then keeps it under the cursor  
    {
        if (tileEnabled == false)
            movableTile.SetActive(true);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int gridPosition = map.WorldToCell(mousePosition);
        gridPosition = new Vector3Int(gridPosition.x, gridPosition.y, gridPosition.z);

        movableTile.transform.position = map.GetCellCenterWorld(gridPosition);
    }
    
    public void moveCharacter(Vector3Int location)
    {
        if (transform.parent.childCount > 0)
        {
            playableCharacters.Find(characters[currentCharacter].characterName).position = map.GetCellCenterWorld(location);
            characterPos = map.GetCellCenterWorld(location);
            gameMaster.setCurrentCharacter(characters[currentCharacter]);
        }
    }

    public void OnCycleCharacters()
    {
        if (currentCharacter == (characters.Count - 1))
            currentCharacter = 0;
        else
            currentCharacter++;

        updatePathFinder();
    }

    private void moveActionDeselected() 
    {
        movableTile.SetActive(false);
        line.enabled = false;
        text.enabled = false;
    }
    private void moveActionSelected()
    {
        movableTile.SetActive(true);
        line.enabled = true;
        text.enabled = true;
    }

    public void updatePathFinder()
    {
        characterPos = playableCharacters.Find(characters[currentCharacter].characterName).position;
    }

    public void updateCurrentAction()
    {
        currentActionMove = gameMaster.move;
        if(currentActionMove == false)
            moveActionDeselected();
        else
            moveActionSelected();
    }
}