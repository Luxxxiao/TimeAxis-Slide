using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class PinchZoom : MonoBehaviour
{
    private Touch oldTouch1;  //上次触摸点1(手指1)
    private Touch oldTouch2;  //上次触摸点2(手指2)

    float speed;

    private void Start()
    {
        UIDragByMocha.isOnly = true;
        speed = 1;
    }

    void Update()
    {
        //没有触摸，就是触摸点为0
        if (Input.touchCount <= 0)
        {
            return;
        }
        //单点触摸， 水平上下旋转
        if (1 == Input.touchCount)
        {
            UIDragByMocha.isOnly = true;
        }
        //多点触摸, 放大缩小
        Touch newTouch1 = Input.GetTouch(0);
        Touch newTouch2 = Input.GetTouch(1);
        //第2点刚开始接触屏幕, 只记录，不做处理
        if (newTouch2.phase == TouchPhase.Began)
        {
            UIDragByMocha.isOnly = false;
            oldTouch2 = newTouch2;
            oldTouch1 = newTouch1;
            return;
        }
        //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型
        float oldDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint(oldTouch1.position), Camera.main.ScreenToWorldPoint(oldTouch2.position));
        float newDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint(newTouch1.position), Camera.main.ScreenToWorldPoint(newTouch2.position));
        //两个距离之差，为正表示放大手势， 为负表示缩小手势
        float offset = newDistance - oldDistance;
        //放大因子， 一个像素按 0.01倍来算(100可调整)
        float scaleFactor = offset / speed;
        Vector3 localScale = transform.localScale;
        Vector3 scale = new Vector3(localScale.x + scaleFactor,
                                    localScale.y + scaleFactor,
                                    localScale.z + scaleFactor);
        //在什么情况下进行缩放
        if (scale.x >= 0.7f && scale.y <= 3f)
        {
            transform.localScale = scale;
        }
        //记住最新的触摸点，下次使用
        oldTouch1 = newTouch1;
        oldTouch2 = newTouch2;
    }
}

public class Messagebox
{
    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);
}