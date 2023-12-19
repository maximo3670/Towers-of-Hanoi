using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class howToPlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public GameObject howToPlayObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        howToPlayObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        howToPlayObject.SetActive(false);
    }
}
