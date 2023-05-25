using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class characterActions : MonoBehaviour
{
    private int currentCharacter;
    bool initExec;
    public GameObject attackTile;
    public GameObject moveTile;
    public GameObject bullet;
    public Tilemap map;
    private gameMaster gameMaster;
    private movementPointsToReach pathFinder;
    private List<CharacterInfo> characters;
    Vector3 currentCharacterPos = new Vector3(0, 0, 0);
    delegate void actionDelecate(Vector2 mousePos);
    actionDelecate currentAction;
    actionDelecate currentGraphic;

    public Toggle moveToggle;
    public Toggle attackToggle;

    void Awake()
    {
        characters = new List<CharacterInfo>();
        pathFinder = transform.Find("pathPrinter").GetComponent<movementPointsToReach>();
        gameMaster = GetComponent<gameMaster>();
    }

    void Start()
    {   
        moveTile = Instantiate(moveTile);
        moveToggle.isOn = true;
        currentAction = moveAction;
        currentGraphic = moveGraphic;
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int gridPosition = map.WorldToCell(mousePosition);
        currentGraphic(map.GetCellCenterWorld(gridPosition));
        
        //currentGraphic(map.WorldToCell(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())));
    }

    void OnClick()
    {
        Vector2 _mousePos = new Vector2(map.GetCellCenterWorld(map.WorldToCell(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()))).x,
                                        map.GetCellCenterWorld(map.WorldToCell(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()))).y);
        currentAction(_mousePos);
    }

    void attackAction(Vector2 mousePos)
    {
        Vector2 charPos = characters[gameMaster.currentCharacter].characterObject.transform.position;
        charPos.y += 0.083f;
        var lookDir = mousePos - charPos;
        var bulletInst = Instantiate(bullet, charPos, Quaternion.FromToRotation(Vector2.left, lookDir));
    }

    void attackGraphic(Vector2 mousePos)
    {
        if (initExec)
        {
            attackTile.SetActive(true);
            initExec = false;
        }
        attackTile.transform.position = mousePos;
        Vector2 currentCharacterPos = characters[gameMaster.currentCharacter].characterObject.transform.position;
        currentCharacterPos.y += 0.083f; mousePos.y += 0.083f;

        Debug.DrawRay(currentCharacterPos, mousePos - currentCharacterPos, Color.green);
    }

    void moveAction(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(ray.origin, ray.direction));
        Debug.Log(hits.Count);
        if (hits.Count > 0)
        {
            hits.Sort((l, r) =>
            {
                var p1 = l.transform.position;
                var p2 = r.transform.position;
                return (int)(((p1.y - p2.y) * 10 + (p2.z - p1.z)) * 10);
            });

            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log(hit.collider.tag);
            }
        }
        if (transform.childCount > 0 && hits[0].collider.tag == "Floor")
        {
            characters[currentCharacter].characterObject.transform.position = mousePos;
            var temp = characters[currentCharacter].characterObject.transform.position;
            temp.z += 10;
            characters[currentCharacter].characterObject.transform.position = temp;
            gameMaster.setCurrentCharacter(characters[currentCharacter]);
        }
    }

    void moveGraphic(Vector2 mousePos)
    {
        if (initExec)
        {
            moveTile.SetActive(true);
            pathFinder.updateCharacterList();
            pathFinder.enableGraphics(true);
            initExec = false;
        }
        moveTile.transform.position = mousePos;
    }

    public void actionUpdated() 
    {
        if(attackToggle.isOn){disableGraphics(); currentAction = attackAction; currentGraphic = attackGraphic;}
        if(moveToggle.isOn){disableGraphics(); currentAction = moveAction; currentGraphic = moveGraphic;}
    }

    void disableGraphics()
    {
        moveTile.SetActive(false);
        attackTile.SetActive(false);
        pathFinder.enableGraphics(false);   
        initExec = true;
    }

    public void OnCycleCharacters()
    {
        if (currentCharacter == (characters.Count - 1))
            currentCharacter = 0;
        else
            currentCharacter++;
    }
    public void updateCharacterList()
    {
        characters = gameMaster.getCharacterList();
        Debug.Log("characterActions character list updated!");
    }
}