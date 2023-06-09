using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : ScriptableObject
{
    public Transform characterObject;
    public Transform fullHealth;
    public Transform emptyHealth;
    public List <Transform> healthParalellograms;
    public string piece;
    public int movementPointsMax;
    public int movementPointsLeft;
    public int maxHealth = 5;
    public int remainingHealth;
    public string characterName;
    public int side;
    public int currentWeaponDamage = 3; //Testing purposes
    private Animator anim;
    private gameMaster gameMaster;

    void OnEnable()
    {
        remainingHealth = maxHealth;
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
    }
    public void updateInfo()
    {
        characterObject.GetComponent<characterCollisionDetector>().side = side;
    }

    public void removeHealth(int damage)
    {
        while(damage > 0 && remainingHealth > 0)
        {
            healthParalellograms[remainingHealth - 1].GetComponent<Animator>().SetTrigger("damage");
            damage--; remainingHealth--;
        }
        if(remainingHealth == 0){gameMaster.removeCharacter(characterName, side);}
    }
    public void enableAttackHover(int damage)
    {
        int i = remainingHealth - 1;
        while(damage > 0 && i >= 0)
        {
            healthParalellograms[i].GetComponent<Animator>().SetBool("attackHover", true);
            i--; damage--;
        }
    }

    public void disableAttackHover()
    {
        foreach(Transform hp in healthParalellograms){hp.GetComponent<Animator>().SetBool("attackHover", false);}
    }
}
