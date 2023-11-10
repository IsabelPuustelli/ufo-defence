using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class characterActions : MonoBehaviour
{
    bool initExec;
    public GameObject attackTile;
    public GameObject moveTile;
    public GameObject emptyTile;
    public GameObject bullet;
    public GameObject attackArea;
    public GameObject holdArea;
    private GameObject mainCamera;
    private Camera cam;
    public Tilemap map;
    private int currentCharacter;
    private int projectileIndex;
    private int animationIndex;
    private gameMaster gameMaster;
    private tileMapManager mapManager;
    private Rigidbody2D rb;
    private movementPointsToReach pathFinder;
    private List<CharacterInfo> characters = new List<CharacterInfo>();
    private List<projectileInfo> projectiles = new List<projectileInfo>();
    private List<CharacterInfo> charInLineOfFire = new List<CharacterInfo>();
    public List<CharacterInfo> charInOverwatch = new List<CharacterInfo>();
    delegate void actionDelecate(Vector2 mousePos);
    actionDelecate currentAction;
    actionDelecate currentGraphic;

    public Toggle moveToggle;
    public Toggle attackToggle;
    public Toggle holdToggle;
    public Button endTurnButton;

    public bool overUI;
    public bool pieceMoved;
    public bool abilityUsed;
    public bool walking = false;

    public string walker;
    AnimationClip cameraMove;
    Animation anim;

    void Awake()
    {
        pathFinder = transform.Find("pathPrinter").GetComponent<movementPointsToReach>();
        mainCamera = GameObject.Find("Main Camera");
        cam = mainCamera.GetComponent<Camera>();
        gameMaster = GetComponent<gameMaster>();
        mapManager = GameObject.Find("Grid").GetComponent<tileMapManager>();
        anim = mainCamera.GetComponent<Animation>();
        cameraMove = new AnimationClip();
        disableGraphics();
    }

    void Start()
    {
        moveTile = Instantiate(moveTile);
        attackTile = Instantiate(attackTile);
        attackArea = Instantiate(attackArea);
        holdArea = Instantiate(holdArea);
        emptyTile = Instantiate(emptyTile);
        moveToggle.isOn = true;
        currentAction = moveAction;
        currentGraphic = moveGraphic;
        projectileIndex = 0;

        pieceMoved = false;
        abilityUsed = false;

        cameraMove.legacy = true;
    }

    void Update()
    {

        if(!overUI)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            currentGraphic(map.GetCellCenterWorld(gridPosition));
        }

        if(walking)
            foreach(CharacterInfo character in charInOverwatch)
            {
                if (character.side != gameMaster.currentTurn)
                {
                    var hit = character.checkOverwatch();
                    if (hit != null)
                    {
                        overwatchTrigger(character, hit);
                    }
                }
            }
    }

    void OnClick()
    {
        Vector2 _mousePos = new Vector2(map.GetCellCenterWorld(map.WorldToCell(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()))).x,
                                        map.GetCellCenterWorld(map.WorldToCell(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()))).y);
        if(!overUI)
            currentAction(_mousePos);
    }

    void attackAction(Vector2 mousePos)
    {
        Vector2 charPos = characters[currentCharacter].characterObject.transform.position;
        charPos.y += 0.083f; mousePos.y += 0.12f;
        var lookDir = mousePos - charPos;

        var bulletSO = ScriptableObject.CreateInstance<projectileInfo>();
        bulletSO.shooter = characters[currentCharacter].characterName;
        bulletSO.damage = 3;
        var bulletInst = Instantiate(bullet, charPos, Quaternion.FromToRotation(Vector2.left, lookDir));
        bulletSO.projectileIndex = projectileIndex;
        bulletSO.setObject(bulletInst);
        projectiles.Add(bulletSO);

        projectileIndex++;
        
        rb = bulletInst.GetComponent<Rigidbody2D>();
        rb.AddForce(-bulletInst.transform.right * 0.001f, ForceMode2D.Impulse);
        abilityUsed = true;

        foreach (CharacterInfo character in characters)
            character.disableAttackHover();

        actionUpdated();
    }

    void attackGraphic(Vector2 mousePos)
    {
        Vector3 currentCharacterPos = characters[currentCharacter].characterObject.transform.position;
        currentCharacterPos.y += 0.12f; mousePos.y += 0.083f;
        if (initExec)
        {
            charInLineOfFire.Clear();
            attackTile.SetActive(true);
            attackArea.SetActive(true);
            initExec = false;
        }
        attackArea.transform.position = currentCharacterPos;
        var moveCheck = attackTile.transform.position;
        attackTile.transform.position = new Vector2(mousePos.x, (mousePos.y - 0.083f));

        Vector3 targetPos = new Vector3(mousePos.x, mousePos.y, attackArea.transform.position.z);
        var lookDir = targetPos - currentCharacterPos;
        Vector3 fixedLookDir = Quaternion.Euler(0, 0, 90) * lookDir;
        attackArea.transform.rotation = Quaternion.LookRotation(Vector3.forward, fixedLookDir);

        if (moveCheck != attackTile.transform.position)
        {
            foreach(CharacterInfo character in charInLineOfFire) {character.disableAttackHover();}
            charInLineOfFire.Clear();
            charInLineOfFire = lineOfFire(currentCharacterPos, lookDir, 0.2f);  
            foreach(CharacterInfo character in charInLineOfFire) 
            {
                if (inCover(characters[currentCharacter].characterObject.position, character.characterObject.position))
                    character.enableAttackHover(characters[currentCharacter].currentWeaponDamage / 2);
                else
                    character.enableAttackHover(characters[currentCharacter].currentWeaponDamage);
            }
        }

        if(attackArea.transform.rotation.eulerAngles.z < 180)
        {
            float rotationAmount = 30 - (60 * (attackArea.transform.rotation.eulerAngles.z / 180));
            attackArea.transform.RotateAround(attackArea.transform.position, lookDir, rotationAmount);
        }else{
            float rotationAmount = -1 * (30 - (60 * ((attackArea.transform.rotation.eulerAngles.z - 180) / 180)));
            attackArea.transform.RotateAround(attackArea.transform.position, lookDir, rotationAmount);
        }
        Debug.DrawRay(currentCharacterPos, lookDir, Color.green);
    }

    void moveAction(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(ray.origin, ray.direction));
        if (hits.Count > 0)
        {
            hits.Sort((l, r) =>
            {
                var p1 = l.transform.position;
                var p2 = r.transform.position;
                return (int)(((p1.y - p2.y) * 10 + (p2.z - p1.z)) * 10);
            });
        }
        if (hits[0].collider.tag == "Floor" &&
            Convert.ToInt32(Math.Round(pathFinder.length * 10, 0)) <= characters[currentCharacter].movementPointsLeft)
        {
            characters[currentCharacter].movementPointsLeft -= Convert.ToInt32(Math.Round(pathFinder.length * 10, 0));
            characters[currentCharacter].characterObject.GetComponent<moveAnimationController>().i = 0; 
            walking = true; walker = characters[currentCharacter].characterName;
            restrictAction(true);
            characters[currentCharacter].characterObject.GetComponent<moveAnimationController>().moveAnimation();
            gameMaster.setCharacter(characters[currentCharacter], currentCharacter);
            pieceMoved = true;
            mapManager.fogOfWar(9, mousePos);

        }else if (hits[0].collider.tag == "Floor"){Debug.Log("Distance " + Convert.ToInt32(Math.Round(pathFinder.length * 10, 0)) + " is greater than movement poinst " + characters[currentCharacter].movementPointsLeft);}
    }

    void moveGraphic(Vector2 mousePos) 
    {
        if (initExec)
        {
            Debug.Log("pathfinder = true");
            moveTile.SetActive(true);
            pathFinder.updateCharacterList();
            pathFinder.enableGraphics(true);
            initExec = false;
        }
        moveTile.transform.position = mousePos;
    }

    void holdAction (Vector2 mousePos)
    {
        Vector2 origin = new Vector2(characters[currentCharacter].characterObject.transform.position.x, characters[currentCharacter].characterObject.transform.position.y);
        characters[currentCharacter].setOverwatch(mousePos - origin);
        abilityUsed = true;
        actionUpdated();
    }

    void holdGraphic (Vector2 mousePos)
    {
        Vector3 currentCharacterPos = characters[currentCharacter].characterObject.transform.position;
        currentCharacterPos.y += 0.12f; mousePos.y += 0.083f;
        if (initExec)
        {
            holdArea.SetActive(true);
            initExec = false;
        }
        holdArea.transform.position = currentCharacterPos;

        Vector3 targetPos = new Vector3(mousePos.x, mousePos.y, holdArea.transform.position.z);
        var lookDir = targetPos - currentCharacterPos;
        Vector3 fixedLookDir = Quaternion.Euler(0, 0, 90) * lookDir;
        holdArea.transform.rotation = Quaternion.LookRotation(Vector3.forward, fixedLookDir);

        if(holdArea.transform.rotation.eulerAngles.z < 180)
        {
            float rotationAmount = 30 - (60 * (holdArea.transform.rotation.eulerAngles.z / 180));
            holdArea.transform.RotateAround(holdArea.transform.position, lookDir, rotationAmount);
        }else{
            float rotationAmount = -1 * (30 - (60 * ((holdArea.transform.rotation.eulerAngles.z - 180) / 180)));
            holdArea.transform.RotateAround(holdArea.transform.position, lookDir, rotationAmount);
        }
    }

    void emptyGraphic(Vector2 mousePos)
    {
        if (initExec)
        {
            emptyTile.SetActive(true);
            initExec = false;
        }
        emptyTile.transform.position = mousePos;
    }

    public void restrictAction(bool restrict)
    {
        if (restrict)
            animationIndex++;
        else
            animationIndex--;
        
        if (animationIndex > 0)
        {
            disableGraphics();
            moveToggle.interactable = false;
            attackToggle.interactable = false;
            holdToggle.interactable = false;
            endTurnButton.interactable = false;
            currentGraphic = emptyGraphic;
            currentAction = emptyGraphic;
        }
        else
        {
            moveToggle.interactable = true;
            attackToggle.interactable = true;
            holdToggle.interactable = true;
            endTurnButton.interactable = true;
            actionUpdated();
        }
    }

    public void actionUpdated()
    {
        if(abilityUsed)
        {
            moveToggle.isOn = false;
            attackToggle.isOn = false;
            holdToggle.isOn = false;
            moveToggle.interactable = false;
            attackToggle.interactable = false;
            holdToggle.interactable = false;
            disableGraphics(); currentAction = emptyGraphic; currentGraphic = emptyGraphic;
        }else
        {
            if (attackToggle.isOn) { disableGraphics(); currentAction = attackAction; currentGraphic = attackGraphic; }
            else if (moveToggle.isOn) { disableGraphics(); currentAction = moveAction; currentGraphic = moveGraphic; }
            else if (holdToggle.isOn) { disableGraphics(); currentAction = holdAction; currentGraphic = holdGraphic; }
            else {disableGraphics(); currentAction = emptyGraphic; currentGraphic = emptyGraphic;}
        }
    }

    void disableGraphics()
    {
        moveTile.SetActive(false);
        attackTile.SetActive(false);
        emptyTile.SetActive(false);
        attackArea.SetActive(false);
        holdArea.SetActive(false);
        pathFinder.enableGraphics(false);
        initExec = true;
    }

    List<CharacterInfo> lineOfFire (Vector2 origin, Vector2 direction, float range)
    {
        List<CharacterInfo> hitCharacters = new List<CharacterInfo>();
        hitCharacters.Clear();
        Vector2 perp = Vector2.Perpendicular(direction);

        for(float i = -1; i <= 1; i += 0.1f)
        {
            var newTarget = origin + direction + (perp * (range * i));
            var hits = new List<RaycastHit2D>(Physics2D.RaycastAll(origin, newTarget - origin, 20f, LayerMask.GetMask("Characters")));
            if(hits.Count > 1)  // > 1 because the ray initially hits the character shooting, so we want to belay that
            {
                hitCharacters.Add(characters[gameMaster.findChar(hits[1].collider.gameObject.name, characters)]);
            }
        }
        return hitCharacters;
    }
    bool inCover(Vector3 origin, Vector3 target)    //Return true if target is behind effective cover from origin
    {
        origin.y += 0.12f; target.y += 0.12f;
        var hits = Physics2D.RaycastAll(origin, target - origin, Vector3.Distance(origin, target), LayerMask.GetMask("Cover"));
        if (hits.Length != 0)
        {
            var hit = hits[hits.Length - 1];
            if (hit.collider.tag == "lowCover")  //Is there cover between the two points
            {
                var coverPos = map.WorldToCell(hit.point);
                var targetCords = map.WorldToCell(target);
                if (Vector3.Distance(map.GetCellCenterWorld(coverPos), map.GetCellCenterWorld(targetCords)) < 0.34f &&  //Is the target next to the cover
                    Vector3.Distance(map.GetCellCenterWorld(coverPos), map.GetCellCenterWorld(map.WorldToCell(origin))) > 0.34f)    //Is the shooter not on the other side of the same cover
                    return true;
                else
                    return false;
            }
            else
                return false; 
        }
        else
            return false;
    }
    public void shotAnimation(Vector2 shooter, Vector2 target)
    {
        Vector2 middle = shooter + ((target - shooter) / 2);

        AnimationCurve xCurve = AnimationCurve.EaseInOut(0, mainCamera.transform.position.x, 0.5f, middle.x);
        AnimationCurve yCurve = AnimationCurve.EaseInOut(0, mainCamera.transform.position.y, 0.5f, middle.y);
        AnimationCurve zCurve = AnimationCurve.EaseInOut(0, mainCamera.transform.position.z, 0.5f, mainCamera.transform.position.z);

        cameraMove.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
        cameraMove.SetCurve("", typeof(Transform), "localPosition.y", yCurve);
        cameraMove.SetCurve("", typeof(Transform), "localPosition.z", zCurve);

        anim.AddClip(cameraMove, cameraMove.name);
        anim.Play(cameraMove.name);
        StartCoroutine(cameraZoomOut(Vector2.Distance(shooter, target)));
    }
    IEnumerator cameraZoomOut(float distance)
    {
        for(float height = cam.orthographicSize; height < distance / 2; height += 0.01f)
        {
            cam.orthographicSize = height;
            yield return null;
        }
        StopCoroutine(cameraZoomOut(distance));
    }
    public IEnumerator overwatchAnimation(CharacterInfo shooter, CharacterInfo target)
    {
        Vector2 shooterPos = shooter.characterObject.transform.position;
        Vector2 targetPos = target.characterObject.transform.position;
        shotAnimation(shooterPos, targetPos);

        yield return new WaitForSeconds(1.5f);

        var lookDir = targetPos - shooterPos;
        var bulletSO = ScriptableObject.CreateInstance<projectileInfo>();
        bulletSO.shooter = shooter.characterName;
        bulletSO.damage = 3;
        var bulletInst = Instantiate(bullet, shooterPos, Quaternion.FromToRotation(Vector2.left, lookDir));
        bulletSO.projectileIndex = projectileIndex;
        bulletSO.setObject(bulletInst);
        projectiles.Add(bulletSO);

        projectileIndex++;
        
        rb = bulletInst.GetComponent<Rigidbody2D>();
        rb.AddForce(-bulletInst.transform.right * 0.001f, ForceMode2D.Impulse);
        target.characterObject.GetComponent<moveAnimationController>().stopAnimation = false;
        StopCoroutine(overwatchAnimation(shooter, target));
    }
    public void overwatchTrigger(CharacterInfo shooter, CharacterInfo target)
    {
        walking = false;
        StartCoroutine(overwatchAnimation(shooter, target));
        charInOverwatch.Remove(shooter);
    }
    public void addOverwatch(CharacterInfo character)
    {
        charInOverwatch.Add(character);
    }
    public void removeProjectile(int i)
    {
        Debug.Log("Removing projectile " + i);
        Destroy(projectiles[i].projectileObject);
    }
    public void cycleCharacters()
    {
        if(!pieceMoved)
        {
            if (currentCharacter == (characters.Count - 1))
                currentCharacter = 0;
            else
                currentCharacter++;
            while (gameMaster.currentTurn != characters[currentCharacter].side)
            {
                currentCharacter++;
                if (currentCharacter == (characters.Count))
                    currentCharacter = 0;
            }
        }
    }
    public void updateCharacterList()
    {
        characters = gameMaster.getCharacterList();
    }
    public void turnReset()
    {
        pieceMoved = false;
        abilityUsed = false;
        moveToggle.interactable = true;
        attackToggle.interactable = true;
        moveToggle.isOn = true;
        actionUpdated();
        gameMaster.OnCycleCharacters();
    }
    public void endTurn()
    {
        foreach (CharacterInfo character in characters)
            character.movementPointsLeft = character.movementPointsMax;       
        gameMaster.switchTurns();
        turnReset();
    }
}