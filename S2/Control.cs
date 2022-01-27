using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    public static Control instance;
    public GameObject cover;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Update()
    {

    }

}
