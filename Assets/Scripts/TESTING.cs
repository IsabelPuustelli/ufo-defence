using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TESTING : MonoBehaviour
{

    private List<CharacterInfo> characters;
    private gameMaster master;
    public string name;
    public string displayList;
    public Text text;
    public InputField input;

    void Awake()
    {
        characters = new List<CharacterInfo>();
        master = GameObject.Find("GameMaster").GetComponent<gameMaster>();
    }

    void Start()
    {
        displayList = "Spawn list: ";

        var obj = ScriptableObject.CreateInstance<CharacterInfo>();
        obj.piece = "knight";
        obj.movementPointsMax = 120;
        obj.movementPointsLeft = 120;
        obj.side = 0;
        obj.characterName = "Toddy";

        var obj2 = ScriptableObject.CreateInstance<CharacterInfo>();
        obj2.piece = "rook";
        obj2.movementPointsMax = 120;
        obj2.movementPointsLeft = 120;
        obj2.side = 0;
        obj2.characterName = "Bobby";

        var obj1 = ScriptableObject.CreateInstance<CharacterInfo>();
        obj1.movementPointsMax = 120;
        obj1.piece = "rook";
        obj1.movementPointsLeft = 120;
        obj1.side = 1;
        obj1.characterName = "Ben";

        var obj3 = ScriptableObject.CreateInstance<CharacterInfo>();
        obj3.piece = "knight";
        obj3.movementPointsMax = 120;
        obj3.movementPointsLeft = 120;
        obj3.side = 1;
        obj3.characterName = "Lewis";

        characters.Add(obj);
        characters.Add(obj1);
        characters.Add(obj2);
        characters.Add(obj3);
        master.spawnCharacters(characters);
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

    public void Spawn()
    {
        master.spawnCharacters(characters);
        GameObject.Find("pathPrinter").GetComponent<movementPointsToReach>().updatePathFinder();
        displayList = ""; text.GetComponent<Text>().text = displayList; //characters.Clear();
    }
}
