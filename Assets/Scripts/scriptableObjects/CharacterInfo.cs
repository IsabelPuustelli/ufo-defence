using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Piece", menuName = "New piece")]
public class CharacterInfo : ScriptableObject
{
    public Transform characterObject;
    public Transform characterPrefab;
    public Transform fullHealth;
    public Transform emptyHealth;
    public List <Transform> healthParalellograms;
    public string piece;
    public int movementPointsMax = 200;
    public int movementPointsLeft;
    public int maxHealth = 5;
    public int remainingHealth;
    public string characterName;
    public int side;
    public int currentWeaponDamage = 3; //Testing purposes
    private gameMaster gameMaster;
    private characterActions characterActions;
    private List<Vector2> overwatchDirections = new List<Vector2>();

    void OnEnable()
    {
        remainingHealth = maxHealth;
        movementPointsLeft = movementPointsMax;
        gameMaster = GameObject.Find("GameMaster").GetComponent<gameMaster>();
        characterActions = GameObject.Find("GameMaster").GetComponent<characterActions>();
    }

    void Update()
    {
        if (characterActions.walking == true && overwatchDirections.Count > 0)
        {
            Vector2 origin = new Vector2(characterObject.transform.position.x, characterObject.transform.position.y);
            foreach (Vector2 dir in overwatchDirections)
            {
                var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(origin, dir - origin, 20, LayerMask.GetMask("Characters")));
                Debug.DrawRay(origin, dir - origin, Color.blue, 5);
                if(hits.Count > 1)  // > 1 because the ray initially hits the character shooting, so we want to belay that
                {
                    foreach(RaycastHit2D hit in hits)
                    {
                        if (hit.transform.name == characterActions.walker)
                        {
                            gameMaster.getCharacterByName(hit.transform.name).characterObject.GetComponent<moveAnimationController>().stopAnimation = true;
                        }
                    }
                }
            }
        }
    }

    public void setOverwatch(Vector2 direction)
    {
        Vector2 origin = new Vector2(characterObject.transform.position.x, characterObject.transform.position.y + 0.12f);
        Vector2 perp = Vector2.Perpendicular(direction);
        for(float i = -1; i <= 1; i += 0.1f)
        {
            var newTarget = origin + direction + (perp * (0.8f * i));
            overwatchDirections.Add(newTarget);
            Debug.DrawRay(origin, (newTarget - origin) * 10, Color.blue, 5);
            //var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(characterObject.transform.position, newTarget - characterObject.transform.position, 20f, LayerMask.GetMask("Characters")));
        }
        characterActions.addOverwatch(this);
    }
    public void checkOverwatch()
    {
        Vector2 origin = new Vector2(characterObject.transform.position.x, characterObject.transform.position.y + 0.12f);
        foreach (Vector2 dir in overwatchDirections)
        {
            Debug.DrawRay(origin, (dir - origin) * 10, Color.blue, 5);
            var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(origin, dir - origin, 20, LayerMask.GetMask("Characters")));
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.transform.name == characterActions.walker)
                {
                    gameMaster.getCharacterByName(hit.transform.name).characterObject.GetComponent<moveAnimationController>().stopAnimation = true;
                    characterActions.overwatchTrigger(this, hit.transform.position);
                }
            }
        }
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