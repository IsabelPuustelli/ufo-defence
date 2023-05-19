using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TESTING : MonoBehaviour
{

    private List<CharacterInfo> characters;
    public GameObject knight;
    public GameObject rook;
    private GameObject master;
    public string name;
    public string displayList;
    public Text text;
    public InputField input;


    void Awake()
    {
        displayList = "Spawn list: ";
        characters = new List<CharacterInfo>();
        master = GameObject.Find("GameMaster");

        /*

        var obj = ScriptableObject.CreateInstance<CharacterInfo>();
        obj.characterObject = knight.transform;
        obj.characterName = "Toddy";

        var obj1 = ScriptableObject.CreateInstance<CharacterInfo>();
        obj1.characterObject = knight.transform;
        obj1.characterName = "Ben";

        characters.Add(obj);
        characters.Add(obj1);*/
    }

    public void addKnight ()
    {
        name = input.GetComponent<InputField>().text;
        bool theNameAlreadyInUse = false;
        var obj = ScriptableObject.CreateInstance<CharacterInfo>();
        obj.characterObject = knight.transform;
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
        obj.characterObject = rook.transform;
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
        master.GetComponent<gameMaster>().spawnCharacters(characters);
        GameObject.Find("pathPrinter").GetComponent<movementPointsToReach>().updatePathFinder();
        displayList = ""; text.GetComponent<Text>().text = displayList; //characters.Clear();
    }
}
