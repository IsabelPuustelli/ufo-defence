using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mouseOverUIDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject.Find("GameMaster").GetComponent<characterActions>().overUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject.Find("GameMaster").GetComponent<characterActions>().overUI = false;
    }
}
