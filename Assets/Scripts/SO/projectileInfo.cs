using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileInfo : ScriptableObject
{
    public GameObject projectileObject; 
    public int damage;
    public string shooter;
    public int projectileIndex;

    public void setObject(GameObject _projectileObject) 
    {
        projectileObject = _projectileObject;
        projectileObject.GetComponent<bulletCollisionDetector>().projectileIndex = projectileIndex;
        projectileObject.GetComponent<bulletCollisionDetector>().shooter = shooter;
    }
}
