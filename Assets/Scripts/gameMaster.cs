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
    public int currentTurn;
    private List<CharacterInfo> blacks;
    private List<CharacterInfo> whites;
    public List<CharacterInfo> allCharacters;
    private Transform whitesTrans;
    private Transform blacksTrans;
    public Transform fullHealth;
    public Transform emptyHealth;
    public Transform wKnight;
    public Transform wRook;
    public Transform bKnight;
    public Transform bRook;
    public Tilemap map;
    private movementPointsToReach pathFinder;
    private characterActions characterActions;
    private animationController anim;
    bool gameEnd = false;
    public int currentCharacter = 0;

    void Awake()
    {
        whites = new List<CharacterInfo>();
        blacks = new List<CharacterInfo>();
        allCharacters = new List<CharacterInfo>();

        whitesTrans = transform.Find("whitesTrans");
        blacksTrans = transform.Find("blacksTrans");

        pathFinder = transform.Find("pathPrinter").GetComponent<movementPointsToReach>();
        characterActions = gameObject.GetComponent<characterActions>();
        anim = transform.Find("animations").GetComponent<animationController>();

        updateCharacterList.AddListener(pathFinder.updateCharacterList);
        updateCharacterList.AddListener(characterActions.updateCharacterList);
    }

    public void switchTurns()
    {
        if(currentTurn == 0){currentTurn = 1;}
        else{currentTurn = 0;}
        updateCharacterList.Invoke();
    }

    public void spawnCharacters(List<CharacterInfo> characterList)
    {
        int i = 0;
        foreach (CharacterInfo character in characterList)
        {
            float j = -0.024f;
            if (character.side == 0) 
            {
                switch (character.piece)
                {
                    case "pawn": break;
                    case "knight": character.characterObject = Instantiate(wKnight, map.GetCellCenterWorld(new Vector3Int(-11 - i, -20, 0)), Quaternion.identity, whitesTrans); break;
                    case "rook":   character.characterObject = Instantiate(wRook, map.GetCellCenterWorld(new Vector3Int(-11 - i, -20, 0)), Quaternion.identity, whitesTrans);   break;
                }
                character.characterObject.name = character.characterName;
                character.updateInfo();
                List <Transform> healthParalellograms = new List<Transform>();

                for(int x = 0; x < character.maxHealth; x++)
                {
                    var inst = Instantiate(emptyHealth, new Vector2(character.characterObject.transform.position.x + j, character.characterObject.transform.position.y + 0.49f), Quaternion.identity, character.characterObject.transform);
                    j += 0.09f;
                }
                j = -0.024f;
                for(int x = 0; x < character.maxHealth; x++)
                {
                    var inst = Instantiate(fullHealth, new Vector2(character.characterObject.transform.position.x + j, character.characterObject.transform.position.y + 0.49f), Quaternion.identity, character.characterObject.transform);
                    healthParalellograms.Add(inst);
                    j += 0.09f;
                }
                character.healthParalellograms = healthParalellograms;
                character.fullHealth = fullHealth;
                character.emptyHealth = emptyHealth;
                whites.Add(character);
                allCharacters.Add(character);
                Debug.Log(character.characterName + " spawned on white side");
            }else{
                switch (character.piece)
                {
                    case "pawn": break;
                    case "knight": character.characterObject = Instantiate(bKnight, map.GetCellCenterWorld(new Vector3Int(-11 - i, -20, 0)), Quaternion.identity, blacksTrans); break;
                    case "rook":   character.characterObject = Instantiate(bRook, map.GetCellCenterWorld(new Vector3Int(-11 - i, -20, 0)), Quaternion.identity, blacksTrans);   break;
                }
                character.characterObject.name = character.characterName;
                character.updateInfo();
                List <Transform> healthParalellograms = new List<Transform>();

                for(int x = 0; x < character.maxHealth; x++)
                {
                    var inst = Instantiate(emptyHealth, new Vector2(character.characterObject.transform.position.x + j, character.characterObject.transform.position.y + 0.49f), Quaternion.identity, character.characterObject.transform);
                    j += 0.09f;
                }
                j = -0.024f;
                for(int x = 0; x < character.maxHealth; x++)
                {
                    var inst = Instantiate(fullHealth, new Vector2(character.characterObject.transform.position.x + j, character.characterObject.transform.position.y + 0.49f), Quaternion.identity, character.characterObject.transform);
                    healthParalellograms.Add(inst);
                    j += 0.09f;
                }
                character.healthParalellograms = healthParalellograms;
                character.fullHealth = fullHealth;
                character.emptyHealth = emptyHealth;
                blacks.Add(character);
                allCharacters.Add(character);
                Debug.Log(character.characterName + " spawned on black side");
            }
            i++;
        }
        updateCharacterList.Invoke();
    }

    public void removeCharacter(string name, int side)
    {
        if (side == 0)
        {
            foreach(CharacterInfo character in whites)
            {
                if (name == character.characterName)
                {
                    Destroy(character.characterObject.gameObject);
                }
            }
        }else{
            foreach(CharacterInfo character in blacks)
            {
                if (name == character.characterName)
                {
                    Destroy(character.characterObject.gameObject);
                }
            }
        }
    }

    public void characterDamage(string name, int side, int damage)
    {
        if (side == 0)
        {
            foreach (CharacterInfo character in whites)
            {
                if (character.characterName == name)
                {
                    Debug.Log(name + " took " + damage + " damage!");
                    anim.damageAnimation(character.characterObject.transform.position, damage);
                    character.removeHealth(damage);
                }
            }
        }else{
            foreach (CharacterInfo character in blacks)
            {
                if (character.characterName == name)
                {
                    Debug.Log(name + " took " + damage + " damage!");
                    anim.damageAnimation(character.characterObject.transform.position, damage);
                    character.removeHealth(damage);
                }
            }
        }

    }

    public void OnCycleCharacters()
    {
        if(!characterActions.pieceMoved)
        {
            if (currentCharacter == (allCharacters.Count - 1))
                currentCharacter = 0;

            currentCharacter++;
            while (currentTurn != allCharacters[currentCharacter].side)
            {
                currentCharacter++;
                if (currentCharacter == (allCharacters.Count))
                    currentCharacter = 0;
            }
        }
        pathFinder.cycleCharacters();
        characterActions.cycleCharacters();
    }
    
    public List<CharacterInfo> getCharacterList()
    {
        return allCharacters;
    }

    public CharacterInfo getCurrentCharacter()
    {
        if(currentTurn == 0){return whites[currentCharacter];}
        else                {return blacks[currentCharacter];}

    }

    public void setCharacterList(List<CharacterInfo> _characters) 
    {
        allCharacters = _characters; whites.Clear(); blacks.Clear();
        foreach(CharacterInfo character in _characters)
        {
            if (character.side == 0) {whites.Add(character);}
            else{blacks.Add(character);}
        }
        updateCharacterList.Invoke();
    }

    public void setCharacter(CharacterInfo _character, int i)
    {
        allCharacters[i] = _character;
        
        if (_character.side == 0)
        {
            whites[findChar(_character.characterName, whites)] = _character;
        }else{
            blacks[findChar(_character.characterName, blacks)] = _character;
        }
        updateCharacterList.Invoke();
    }

    public int findChar(string name, List<CharacterInfo> characters)
    {
        int i = 0;
        foreach(CharacterInfo character in characters)
        {
            if (character.characterName == name){return i;}
            else{i++;}
        }
        return -1;
    }
}