using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{
    Animator anim;
    public int animationDamage;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void damageAnimation(Vector2 location, int damage)
    {
        location.y += 0.21f;
        transform.position = location;
        animationDamage = damage;
        anim.SetTrigger("greenStar");
    }
}
