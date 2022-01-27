using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Controller : MonoBehaviour
{
    public VideoPlayer vp;
    public Image img;
    Dictionary<string, string> configDic;

    // Start is called before the first frame update
    void Start()
    {
        configDic = new Dictionary<string, string>();
        InitialConfig();
        vp.url = configDic["视频路径"];
        Stage1();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitialConfig()
    {
        string[] configs = File.ReadAllLines(Application.dataPath + @"\Config.txt");
        foreach(string s in configs)
        {
            string[] tmp = s.Split('=');
            configDic.Add(tmp[0], tmp[1]);
        }
    }

    internal void Stage1()
    {
        img.enabled = true;
        vp.enabled = false;
    }

    internal void Stage2()
    {
        img.enabled = false;
        vp.enabled = true;
    }


    internal void PlayVideo()
    {
        vp.Play();
    }

    internal void PauseVideo()
    {
        vp.Pause();
    }
}
