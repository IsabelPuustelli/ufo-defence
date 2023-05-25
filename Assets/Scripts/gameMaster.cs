using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class gameMaster : MonoBehaviour
{
    UnityEvent updateCharacterList = new UnityEvent();
    private List<CharacterInfo> characters;
    private Transform playableCharacters;
    public Tilemap map;
    private movementPointsToReach pathFinder;
    private characterActions characterActions;
    bool gameEnd = false;
    public int currentCharacter = 0;

    void Awake()
    {
        characters = new List<CharacterInfo>();
        playableCharacters = transform.Find("PlayableCharacters");
        pathFinder = transform.Find("pathPrinter").GetComponent<movementPointsToReach>();
        characterActions = GetComponent<characterActions>();
    }

    void Start() 
    {
        updateCharacterList.AddListener(pathFinder.updateCharacterList);
        updateCharacterList.AddListener(characterActions.updateCharacterList);
    }

    public void spawnCharacters(List<CharacterInfo> characterList)
    {
        characters = characterList;
        int i = 0;
        foreach (CharacterInfo character in characters)
        {
            character.characterObject.name = character.characterName;
            character.characterObject = Instantiate(character.characterObject, map.GetCellCenterWorld(new Vector3Int(-8 - i, 0, -32)), Quaternion.identity, playableCharacters);
            i++;
        }
        Debug.Log("Invoking updateCharacterList");
        updateCharacterList.Invoke();
        characterActions.updateCharacterList();
    }

    public void OnCycleCharacters()
    {
        if (currentCharacter == (characters.Count - 1))
            currentCharacter = 0;
        else
            currentCharacter++;
    }
    
    public List<CharacterInfo> getCharacterList()
    {
        return characters;
    }

    public CharacterInfo getCurrentCharacter()
    {
        return characters[currentCharacter];
    }

    public void setCharacterList(List<CharacterInfo> _characters) 
    {
        characters = _characters;
    }

    public void setCurrentCharacter(CharacterInfo _character)
    {
        characters[currentCharacter] = _character;
    }
}