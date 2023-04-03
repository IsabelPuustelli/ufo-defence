using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class gameMaster : MonoBehaviour
{
    private List<CharacterInfo> characters;
    public GameObject pathFinder;
    private Transform playableCharacters;
    public Tilemap map;
    private movementPointsToReach movement;
    bool gameEnd = false;
    public int currentCharacter = 0;

    void Start() 
    {
        movement = transform.Find("pathPrinter").GetComponent<movementPointsToReach>();
        playableCharacters = transform.Find("PlayableCharacters");
    }

    public void spawnCharacters(List<CharacterInfo> characterList)
    {
        characters = characterList;
        int i = 0;
        foreach (CharacterInfo character in characters)
        {
            var trans = Instantiate(character.characterObject, map.GetCellCenterWorld(new Vector3Int(-8 - i, 0, -32)), Quaternion.identity, playableCharacters);
            trans.name = character.characterName;
            i++;
        }
        movement.updateCharacterList();
    }
/*
    public void updatePathFinder()
    {
        pathFinder.GetComponent<movementPointsToReach>().characterPos = transform.parent.Find(characters[currentCharacter].characterName).position;
    }
    
    public void moveCharacter(Vector3Int location)
    {
        if (transform.parent.childCount > 0)
        {
            transform.parent.Find(characters[currentCharacter].characterName).position = map.GetCellCenterWorld(location);
            pathFinder.GetComponent<movementPointsToReach>().characterPos = map.GetCellCenterWorld(location);
        }
    }
*/
    public List<CharacterInfo> getCharacterList(List<CharacterInfo> _characters)
    {
        _characters = characters;
        return _characters;
    }

    public void OnCycleCharacters()
    {
        if (currentCharacter == (characters.Count - 1))
            currentCharacter = 0;
        else
            currentCharacter++;
    }

    public void OnClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        movement.moveCharacter(map.WorldToCell(mousePosition));
    }
}