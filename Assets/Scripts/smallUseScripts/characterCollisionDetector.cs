using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class characterCollisionDetector : MonoBehaviour
{
    public int side;
    private gameMaster gameMaster;
    private characterActions characterActions;
    private Tilemap map;

    void Start()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
        characterActions = GameObject.Find("GameMaster").GetComponentInChildren<characterActions>();
        map = GameObject.Find("Cover").GetComponent<Tilemap>();
        Vector3Int currentCell = map.WorldToCell(transform.position);
        transform.position =  map.GetCellCenterWorld(currentCell);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "projectile" && collision.gameObject.GetComponent<bulletCollisionDetector>().shooter != gameObject.name)
        {
            gameMaster.characterDamage(gameObject.name, side, 3);
            characterActions.removeProjectile(collision.gameObject.GetComponent<bulletCollisionDetector>().projectileIndex);
        }
    }
}
