using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class fireAction : MonoBehaviour
{
    private gameMaster gameMaster;
    private List<CharacterInfo> characters;
    public Tilemap map;
    public GameObject attackTile;
    bool spawned;
    bool aimStart;
    Vector3 currentCharacterPos = new Vector3(0, 0, 0);

    bool currentActionFire;

    void Awake() 
    {
        aimStart = true;
        gameMaster = GetComponent<gameMaster>();
        attackTile = Instantiate(attackTile, Vector3.zero, Quaternion.identity);
        attackTile.SetActive(false);
    }

    void Update() 
    {
        if(currentActionFire)
        {
            aim();
        }
    }

    void aim()
    {
        if (aimStart)
        {
            attackTile.SetActive(true);
            aimStart = false;
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int gridPosition = map.WorldToCell(mousePosition);
        gridPosition = new Vector3Int(gridPosition.x, gridPosition.y, gridPosition.z);
        attackTile.transform.position = map.GetCellCenterWorld(gridPosition);
        currentCharacterPos = characters[gameMaster.currentCharacter].characterObject.transform.position;

        Debug.DrawRay(currentCharacterPos, map.GetCellCenterWorld(gridPosition) - currentCharacterPos, Color.green);
    }
    public void updateCharacterList()
    {
        characters = gameMaster.getCharacterList();
    }
}