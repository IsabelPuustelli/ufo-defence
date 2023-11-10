using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TESTING_AI : MonoBehaviour
{

    private List<CharacterInfo> characters;
    private gameMaster master;
    public string name;
    public string displayList;
    public Text text;
    public InputField input;
    private List<Vector3Int> spawnPositions;

    void Awake()
    {
        spawnPositions = new();
        characters = new List<CharacterInfo>();
        master = GameObject.Find("GameMaster").GetComponent<gameMaster>();
    }

    void Start()
    {
        displayList = "Spawn list: ";

        var obj = Knight.CreateInstance("Toddy", 0);

        var obj1 = Rook.CreateInstance("Bobby", 0);

        var obj2 = Rook.CreateInstance("Ben", 1);

        var obj3 = Knight.CreateInstance("Lewis", 1);

        characters.Add(obj);
        characters.Add(obj1);
        characters.Add(obj2);
        characters.Add(obj3);

        spawnPositions.Add(new Vector3Int(-16, 12, 0));
        spawnPositions.Add(new Vector3Int(-14, 12, 0));
        spawnPositions.Add(new Vector3Int(12, -9, 0));
        spawnPositions.Add(new Vector3Int(14, -9, 0));

        master.spawnCharacters(characters, spawnPositions);
    }

    public void addKnight ()
    {
        name = input.GetComponent<InputField>().text;
        bool theNameAlreadyInUse = false;
        var obj = ScriptableObject.CreateInstance<CharacterInfo>();
        obj.piece = "knight";
        obj.characterName = name;

        foreach (CharacterInfo character in characters)
        {
            if (character.characterName == name)
            {
                theNameAlreadyInUse = true;
                Debug.Log("Name already in use!");
            }
        }
        if(theNameAlreadyInUse == false)
        {
            characters.Add(obj); displayList = displayList + "  Knight: " + name; text.GetComponent<Text>().text = displayList;
        }
    }

    public void addRook ()
    {
        name = input.GetComponent<InputField>().text;
        bool theNameAlreadyInUse = false;
        var obj = ScriptableObject.CreateInstance<CharacterInfo>();
        obj.piece = "rook";
        obj.characterName = name;

        foreach (CharacterInfo character in characters)
        {
            if (character.characterName == name)
            {
                theNameAlreadyInUse = true;
                Debug.Log("Name already in use!");
            }
        }
        if(theNameAlreadyInUse == false)
        {
            characters.Add(obj); displayList = displayList + "  Rook: " + name; text.GetComponent<Text>().text = displayList;
        }
    }
}