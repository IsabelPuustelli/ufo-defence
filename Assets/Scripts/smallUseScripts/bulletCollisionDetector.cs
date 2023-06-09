using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletCollisionDetector : MonoBehaviour
{
    private characterActions characterActions;
    public int projectileIndex;
    public string shooter;
    void Start() 
    {
        characterActions = GameObject.Find("GameMaster").GetComponentInChildren<characterActions>();    
    }
    void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == "Cover")
        {
            Debug.Log("bullet hit wall");
            characterActions.removeProjectile(projectileIndex);
        }
    }
}
