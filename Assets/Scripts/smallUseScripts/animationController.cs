using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{
    Animator anim;
    Object[] numbers;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        numbers = Resources.LoadAll("UI/Numbers", typeof(Sprite));
    }

    public void damageAnimation(Vector2 location, int damage)
    {
        location.y += 0.21f;
        transform.position = location;

        foreach (Object number in numbers)
        {
            Debug.Log(number.name);
            Debug.Log(damage);
            if (int.Parse(number.name) == damage)
            {
                Debug.Log("bruh");
                gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)number;
            }
        }
        anim.SetTrigger("damage");
    }
}
