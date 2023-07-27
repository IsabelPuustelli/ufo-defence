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
    private Transform mainCamera;
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
    private tileMapManager tileMapManager;
    private animationController anim;
    public InputAction cameraMovement;
    bool gameEnd = false;
    public int currentCharacter = 0;
    private int i = 0;

    void Awake()
    {
        mainCamera = transform.Find("Main Camera");

        whites = new List<CharacterInfo>();
        blacks = new List<CharacterInfo>();
        allCharacters = new List<CharacterInfo>();

        whitesTrans = transform.Find("whitesTrans");
        blacksTrans = transform.Find("blacksTrans");

        pathFinder = transform.Find("pathPrinter").GetComponent<movementPointsToReach>();
        characterActions = gameObject.GetComponent<characterActions>();
        anim = transform.Find("animations").GetComponent<animationController>();
        tileMapManager = GameObject.Find("Grid").GetComponent<tileMapManager>();

        updateCharacterList.AddListener(pathFinder.updateCharacterList);
        updateCharacterList.AddListener(characterActions.updateCharacterList);
    }

    void Update()
    {
        Vector2 movement = cameraMovement.ReadValue<Vector2>();
        mainCamera.transform.position += new Vector3(movement.x / 10, movement.y / 10, 0);
    }

    void OnEnable()
    {
        cameraMovement.Enable();
    }

    public void switchTurns()
    {
        if(currentTurn == 0){currentTurn = 1;}
        else{currentTurn = 0;}
        updateCharacterList.Invoke();
    }

    public void spawnCharacters(List<CharacterInfo> characterList)
    {
        allCharacters.AddRange(characterList);
        updateCharacterList.Invoke();
        spawnLoop();
    }

    public void spawnLoop()
    {
        float j = -0.024f;
        
        if (i != allCharacters.Count)
        {
            if (allCharacters[i].side == 0) 
            {
                allCharacters[i].characterObject = Instantiate(allCharacters[i].characterPrefab, map.GetCellCenterWorld(new Vector3Int(-11 - i, -20, 0)), Quaternion.identity, whitesTrans);
                allCharacters[i].characterObject.name = allCharacters[i].characterName;
                allCharacters[i].updateInfo();

                // Spawning characters healthbar in sepereate paralellograms
                List <Transform> healthParalellograms = new List<Transform>();
                for(int x = 0; x < allCharacters[i].maxHealth; x++)
                {
                    var inst = Instantiate(emptyHealth, new Vector2(allCharacters[i].characterObject.transform.position.x + j, allCharacters[i].characterObject.transform.position.y + 0.49f), Quaternion.identity, allCharacters[i].characterObject.transform);
                    j += 0.09f;
                }
                j = -0.024f;
                for(int x = 0; x < allCharacters[i].maxHealth; x++)
                {
                    var inst = Instantiate(fullHealth, new Vector2(allCharacters[i].characterObject.transform.position.x + j, allCharacters[i].characterObject.transform.position.y + 0.49f), Quaternion.identity, allCharacters[i].characterObject.transform);
                    healthParalellograms.Add(inst);
                    j += 0.09f;
                }
                allCharacters[i].healthParalellograms = healthParalellograms;
                allCharacters[i].fullHealth = fullHealth;
                allCharacters[i].emptyHealth = emptyHealth;


                allCharacters[i].characterObject.GetComponent<moveAnimationController>().spawnAnimation();

                whites.Add(allCharacters[i]);
                Debug.Log(allCharacters[i].characterName + " spawned on white side");
            }else{
                allCharacters[i].characterObject = Instantiate(allCharacters[i].characterPrefab, map.GetCellCenterWorld(new Vector3Int(-11 - i, -20, 0)), Quaternion.identity, whitesTrans);
                allCharacters[i].characterObject.name = allCharacters[i].characterName;
                allCharacters[i].updateInfo();
                List <Transform> healthParalellograms = new List<Transform>();

                for(int x = 0; x < allCharacters[i].maxHealth; x++)
                {
                    var inst = Instantiate(emptyHealth, new Vector2(allCharacters[i].characterObject.transform.position.x + j, allCharacters[i].characterObject.transform.position.y + 0.49f), Quaternion.identity, allCharacters[i].characterObject.transform);
                    j += 0.09f;
                }
                j = -0.024f;
                for(int x = 0; x < allCharacters[i].maxHealth; x++)
                {
                    var inst = Instantiate(fullHealth, new Vector2(allCharacters[i].characterObject.transform.position.x + j, allCharacters[i].characterObject.transform.position.y + 0.49f), Quaternion.identity, allCharacters[i].characterObject.transform);
                    healthParalellograms.Add(inst);
                    j += 0.09f;
                }
                allCharacters[i].healthParalellograms = healthParalellograms;
                allCharacters[i].fullHealth = fullHealth;
                allCharacters[i].emptyHealth = emptyHealth;

                allCharacters[i].characterObject.GetComponent<moveAnimationController>().spawnAnimation();

                blacks.Add(allCharacters[i]);
                Debug.Log(allCharacters[i].characterName + " spawned on black side");
            }
            i++;
            characterActions.actionUpdated();
        }
        else
        {
            tileMapManager.revealArea(map.GetCellCenterWorld(new Vector3Int(-11 - i, -20, 0)), 15);
            tileMapManager.fogOfWar(10, allCharacters[currentCharacter].characterObject.transform.position);
        }
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
            else
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
        return allCharacters[currentCharacter];
    }

    public CharacterInfo getCharacterByName(string name)
    {
        foreach(CharacterInfo chara in allCharacters)
        {
            if (chara.characterName == name)
                return chara;
        }
        return null;
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