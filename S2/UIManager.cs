using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    Button mBtn;
    Vector3 mVec;
    bool isBig, isDown;
    float downTime;
    
    // Start is called before the first frame update
    void Start()
    {
        mBtn = GetComponent<Button>();
        mVec = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Control.instance.cover.GetComponent<CoverControl>().isClickCover && isBig)
        {
            Control.instance.cover.SetActive(false);
            transform.DOMove(mVec, 1f);
            transform.DOScale(0.4f, 1f).OnComplete(() => {
                Control.instance.cover.GetComponent<CoverControl>().isClickCover = isBig = false;

            });
        }
        if (!isBig)
        {
            GetComponent<PinchZoom>().enabled = GetComponent<UIDragByMocha>().enabled = false;
        }
        else
        {
            GetComponent<PinchZoom>().enabled = GetComponent<UIDragByMocha>().enabled = true;
        }
        if (isDown)
        {
            downTime += Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        downTime = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
        if (downTime < 0.3f && isBig)
        {
            print(mVec);
            Control.instance.cover.SetActive(false);
            transform.DOMove(mVec, 1f);
            transform.DOScale(0.4f, 1f).OnComplete(()=> { isBig = false; });
            
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isBig) return;
        if (mVec.x < 1366)       //left
        {
            transform.DOMove(new Vector3(683, 384, 0), 1f);
            transform.DOScale(1, 1f);
        }
        else if (mVec.x >= 1366 && mVec.x < 2732)      //middle
        {
            transform.DOMove(new Vector3(2049, 384, 0), 1f);
            transform.DOScale(1, 1f);
        }
        else if (mVec.x >= 2732)       //right
        {
            transform.DOMove(new Vector3(3415, 384, 0), 1f);
            transform.DOScale(1, 1f);
        }
        Control.instance.cover.transform.SetAsLastSibling();
        transform.SetAsLastSibling();
        Control.instance.cover.SetActive(true);
        isBig = true;
    }
}
