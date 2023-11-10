using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    private List<CharacterInfo> group;
    private List<CharacterInfo> playerCharacters;
    private List<Vector3> seenCharacters;

    private gameMaster gameMaster;

    void Awake()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
    }

    void turnStart()
    {
        playerCharacters = gameMaster.getCharacterList();
        charactersInSight();
    }

    public void charactersInSight()
    {
        foreach(CharacterInfo enemy in group)
            foreach(CharacterInfo character in playerCharacters)
                if(Vector3.Distance(character.characterObject.transform.position, enemy.characterObject.transform.position) < 3.5f &&
                !seenCharacters.Contains(character.characterObject.transform.position))
                    seenCharacters.Add(character.characterObject.transform.position);
    }
}

public class moveInfo : MonoBehaviour
{
    private Vector3Int destination;
    private int action;
    private Vector3Int aim;
}
