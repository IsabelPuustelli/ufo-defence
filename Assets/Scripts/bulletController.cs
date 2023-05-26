using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    private Collider2D collider;

    void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "cover") { Destroy(gameObject); }
        else if (collision.gameObject.tag == "character") { }
    }
}
