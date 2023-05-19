using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class gameMaster : MonoBehaviour
{
    UnityEvent updateCurrentAction = new UnityEvent();
    UnityEvent updateCharacterList = new UnityEvent();
    private List<CharacterInfo> characters;
    public GameObject pathFinder;
    private Transform playableCharacters;
    public Tilemap map;
    private movementPointsToReach movement;
    private fireAction fireAction;
    bool gameEnd = false;
    public int currentCharacter = 0;

    public Toggle moveToggle;
    public bool move;
    public Toggle fireToggle;
    public bool fire;

    void Awake() 
    {
        movement = transform.Find("pathPrinter").GetComponent<movementPointsToReach>();
        fireAction = GetComponent<fireAction>();
        playableCharacters = transform.Find("PlayableCharacters");

        updateCharacterList.AddListener(movement.updateCharacterList);
        updateCharacterList.AddListener(fireAction.updateCharacterList);

        updateCurrentAction.AddListener(fireAction.updateCurrentAction);
        updateCurrentAction.AddListener(movement.updateCurrentAction);

        move = moveToggle.isOn;
        fire = fireToggle.isOn;
        updateCurrentAction.Invoke();
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
        updateCharacterList.Invoke();
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

    public void currentActionFire()
    {
        fire = fireToggle.isOn;
        updateCurrentAction.Invoke();
    }

    public void currentActionMove()
    {
        move = moveToggle.isOn;
        Debug.Log("Invoking action update");
        updateCurrentAction.Invoke();
    }
}