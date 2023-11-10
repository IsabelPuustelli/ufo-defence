using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : CharacterInfo
{
    public void Init(string _name, int _side)
    {
        if (_side == 0)
            characterPrefab = Resources.Load<Transform>("piecePrefabs/wKnight");
        else if (_side == 1)
            characterPrefab = Resources.Load<Transform>("piecePrefabs/bKnight");
        
        side = _side;
        characterName = _name;
        movementPointsMax = 180;
        maxHealth = 5;
    }

    public static Knight CreateInstance(string name, int side)
    {
        var inst = ScriptableObject.CreateInstance<Knight>();
        inst.Init(name, side);
        return inst;
    }
}
public class Rook : CharacterInfo
{
    public void Init(string _name, int _side)
    {
        if (_side == 0)
            characterPrefab = Resources.Load<Transform>("piecePrefabs/wRook");
        else if (_side == 1)
            characterPrefab = Resources.Load<Transform>("piecePrefabs/bRook");
        
        side = _side;
        characterName = _name;
        movementPointsMax = 180;
        maxHealth = 5;
    }

    public static Rook CreateInstance(string name, int side)
    {
        var inst = ScriptableObject.CreateInstance<Rook>();
        inst.Init(name, side);
        return inst;
    }
}
