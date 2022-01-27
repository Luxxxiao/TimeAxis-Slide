using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoverControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isClickCover;
    bool isDown;
    float downTime= 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDown)
        {
            downTime += Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        isClickCover = true;
        downTime = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
        if(downTime < 1f)
        {
        }
    }
}
