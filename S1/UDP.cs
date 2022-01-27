using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UDP : MonoBehaviour
{
    Socket socket; //目标socket
    EndPoint clientEnd; //客户端
    IPEndPoint ipEnd; //侦听端口

    byte[] recvData; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节

    bool isServerActive = false;
    int recvLen; //接收的数据长度
    string recvStr; //接收的字符串
    Thread connectThread; //连接线程

    string ip;
    int port = 3000;

    bool isWeb;
    bool isAwake = true;

    public Text warning;

    /// <summary>
    /// 指令消息集
    /// </summary>
    private List<Comm> LComm = new List<Comm>();

    public class Comm
    {
        /// <summary>
        /// 消息源地址
        /// </summary>
        public EndPoint _clientEP = new IPEndPoint(IPAddress.Any, 0);
        public string _strComm = string.Empty;
        public Comm(string strComm, EndPoint clientEP)
        {
            _strComm = strComm;
            _clientEP = clientEP;
        }
    }

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (isAwake)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }
            else
            {
                var jo = new AndroidJavaObject("com.test1.test");
#if UNITY_ANDROID
                ip = jo.Call<string>("getHostIp");
#endif
#if UNITY_EDITOR
                ip = GetIP(ADDRESSFAM.IPv4);
                print(ip);
#endif    
#if UNITY_STANDALONE_WIN
                ip = GetIP(ADDRESSFAM.IPv4);
                print(ip);
#endif

                InitSocket(); //在这里初始化server
                isServerActive = true;

                SocketSend("MaiaControl:" + ip);
                isAwake = false;
            }
        }

        //处理接收到的指令集
        if (LComm.Count > 0)
        {
            var recvStr = LComm[0]._strComm;
            var ep = LComm[0]._clientEP;
            LComm.RemoveAt(0);
            //connectIP = (IPEndPoint)ep;
            ExeCommand(recvStr, ep);
        }
    }

    #region UDP通讯
    /// <summary>
    /// 初始化
    /// </summary>
    void InitSocket()
    {
        //定义侦听端口,侦听任何IP 
        ipEnd = new IPEndPoint(IPAddress.Parse(ip), port);
        //定义套接字类型,在主线程中定义 
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //服务端需要绑定ip 
        socket.Bind(ipEnd);
        EndPoint ep = new IPEndPoint(IPAddress.Broadcast, 9090);
        clientEnd = ep;
        print(clientEnd);
        //开启一个线程连接
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }
    /// <summary>
    /// 服务器向客户端发送消息
    /// </summary>
    /// <param name="sendByte"></param>
    public void SocketSend(string sendStr)
    {
        //定义客户端
        //IPEndPoint sender = new IPEndPoint(IPAddress.Any, sendPort);
        //EndPoint ep = new IPEndPoint(IPAddress.Any, sendPort);
        //clientEnd = ep;
        //清空发送缓存 
        sendData = new byte[1024];
        //数据类型转换 
        sendData = Encoding.Unicode.GetBytes(sendStr);
        //发送给指定客户端 
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }

    /// <summary>
    /// 服务器接收来自客户端的消息
    /// </summary>
    void SocketReceive()
    {
        //进入接收循环 
        while (isServerActive)
        {
            //对data清零 
            recvData = new byte[1024];
            //Debug.Log(fileLength);
            try
            {
                //获取服务端端数据
                recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
                if (isServerActive == false)
                {
                    break;
                }
            }
            catch
            {

            }
            if (recvLen > 0)
            {
                recvStr = Encoding.Default.GetString(recvData, 0, recvLen);
                LComm.Add(new Comm(recvStr, clientEnd));
            }
        }
    }


    public void SocketQuit()
    {
        //最后关闭socket
        if (socket != null)
        {
            try
            {
                socket.Close();
            }
            catch
            {

            }
        }
        Debug.LogWarning("local：断开连接");
    }
    #endregion

    #region 指令解析
    /// <summary>
    /// 执行命令行
    /// </summary>
    /// <param name="recvStr">数据内容</param>
    /// <param name="clientEnd">数据端地址</param>
    void ExeCommand(string recvStr, EndPoint clientEnd)
    {
        try
        {
            //print(recvStr);
            switch (recvStr)
            {
                case "play":
                    GetComponent<Controller>().PlayVideo();
                    break;
                case "pause":
                    GetComponent<Controller>().PauseVideo();
                    break;
                case "1":
                    GetComponent<Controller>().Stage1();
                    break;
                case "2":
                    GetComponent<Controller>().Stage2();
                    break;
            }
        }
        catch (Exception ex) { Warning("指令错误！\r\n" + ex.Message + "\r\n" + recvStr); }
    }
    #endregion

    void Warning(string s)
    {
        warning.enabled = true;
        warning.text = s;
        Invoke("InvokeWarn", 3f);
    }
    void InvokeWarn()
    {
        warning.enabled = false;
    }

    #region ip
    public enum ADDRESSFAM
    {
        IPv4, IPv6
    }
    public static string GetIP(ADDRESSFAM Addfam)
    {
        if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
        {
            return null;
        }
        string output = "";
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    //IPv4
                    if (Addfam == ADDRESSFAM.IPv4)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                            //Debug.Log("IP:" + output);
                        }
                    }

                    //IPv6
                    else if (Addfam == ADDRESSFAM.IPv6)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
        }
        return output;
    }
    #endregion
}
