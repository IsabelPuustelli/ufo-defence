using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class characterActions : MonoBehaviour
{
    bool aimStart = false;
    public GameObject attackTile;
    public Tilemap map;
    private gameMaster gameMaster;
    private CharacterInfo[] characters;
    Vector3 currentCharacterPos = new Vector3(0, 0, 0);

    void Start()
    {
        gameMaster = GetComponent<gameMaster>();
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
}