/*============================== FILE HEADER =================================
*
*           Copyright (c) 2013, Samsung Research Institute India , Bangalore
*                      Samsung Confidential Proprietary
*                            All Rights Reserved
*
==============================================================================
*                               Module Name : C# Chord Unity Interface
==============================================================================
* File                    : Spin.cs
*
* Author(s)           : Dhanya R, Ganesh VR
*
* Description         : C# Sample Application Implementation for Chord Unity Plugin verification.                  
*
==============================================================================
*                            Modification History
*-----------------------------------------------------------------------------
*    Date       |       Name                    |        Modification
*-----------------------------------------------------------------------------
*                  |                                    |
*  24-07-13  |  Dhanya R, Ganesh VR         | Newly Added
*                  |                                    |
============================================================================*/
using UnityEngine;

using System;

using System.Collections;

using System.Collections.Generic;

using System.IO;

using System.Text;

public class Spin : MonoBehaviour,ChordUnityNetworkListener
{
    public Transform nodeCube;
    private ChordUnity mChordWrapper = null;
    private ChordUnity mWiFiDirectChordWrapper = null;
    private ChordUnity mMobileAPChordWrapper = null;
    String wifiPrivateDataHistory = "";
    String wifiDirectPrivateDataHistory = "";
    String wifiAPPrivateDataHistory = "";
    String myWiFiIp = "";
    String myWiFiDirectIp = "";
    String myWiFiAPIp = "";
    String fileSelected = "";
    String[] selectedFiles = null;
    public const String chordFilePath = "mnt/sdcard/ChordUnity";
    const String UNITY_SEND_DATA_TEST_CHANNEL = "com.samsung.android.sdk.chord.example.HELLOTESTCHANNEL";
    const String UNITY_SEND_FILE_TEST_CHANNEL = "com.samsung.android.sdk.chord.example.SENDTESTCHANNEL";
    const String UNITY_UDP_TEST_CHANNEL = "com.samsung.android.sdk.chord.example.UNITYUDPTESTCHANNEL";
    const String UNITY_SECURE_TEST_CHANNEL = "com.samsung.android.sdk.chord.example.SECURETESTCHANNEL";
    const String CHORD_UNITY_MESSAGE_TYPE = "com.samsung.android.sdk.chord.example.MESSAGE_TYPE";
    const String CHORD_UNITY_FILE_TYPE = "FILE_NOTIFICATION_V2";
    int SHARE_FILE_TIMEOUT_SECONDS = 60 * 60 * 1000;
    int UNITY_FILE_CHUNK_TIMEOUT = 30 * 1000;
    int UNITY_FILE_CHUNK_RETRY = 2;
    long UNITY_FILE_CHUNK_SIZE = 300 * 1024;
    int privateSelGridInt = 0;
    bool isWiFiPrivateChannelJoined = false;
    bool isWiFiDirectPrivateChannelJoined = false;
    bool isWiFiAPPrivateChannelJoined = false;
    bool isSmartDiscoveryEnabled = false;
    bool isInterfaceAvailable = false;
    String privateChannelWiFi = UNITY_SECURE_TEST_CHANNEL;
    String privateChannelWiFiDirect = UNITY_SECURE_TEST_CHANNEL;
    String privateChannelWiFiAP = UNITY_SECURE_TEST_CHANNEL;
    int x_cor = 70;
    int y_cor = 75;
    String[] privateSelStrings = null;
    List<int> availInterfaces = new List<int>();
    int windowArea = 0;
    int selectedInterface = -1;
    bool isTab = true;
    bool isWiFiDisconnected = false;
    bool isWiFiDirectDisconnected = false;
    bool isWiFiAPDisconnected = false;
    bool isWiFiStarted = false;
    bool isWiFiDirectStarted = false;
    bool isWiFiAPStarted = false;
    bool isUnityInitialized = false;
    Listener wifiLsnr = null;
    Listener wifiDirectLsnr = null;
    Listener wifiAPLsnr = null;
    int startedCount = 0;
    Vector2 scrollPosition1;
    Vector2 scrollPosition2;
    Vector2 scrollPosition3;
    Rect windowRect;
    int wifiCount = 0;
    int wifiDirectCount = 0;
    int wifiAPCount = 0;
    int reliableMode = 0;
    int[] reliableTime = {-1,10,0};
    bool needResponseUdp = false;
    int mul = 1;
    // Use this for initialization      
    String myNodeName = "";

    private class Node
    {   
        public string nodeName;
        public string interfaceName;
        public bool bChecked;

        public Node(string name, string intfc, bool check)
        {
            this.bChecked = check;
            this.interfaceName = intfc;
            this.nodeName = name;
        }

        public void setCheckMode(bool checkMode)
        {
            bChecked = checkMode;
        }
        
    };
    private List<Node> mNodeList = new List<Node>();
    private Transform myWifiNodeCube;
    private Transform myWifiDirectNodeCube;
    private Transform myWifiAPNodeCube;
    int choice;
    bool isWiFiSecureEnabled = false;
    bool isWiFiDirectSecureEnabled = false;
    bool isWiFiAPSecureEnabled = false;
    
    void Start()
    {
        
        // Noticed that start is not executed all the time , so not using to initialize
        //isUnityInitialized = false;
            
    }

    // Start is called first time if the script instance is enabled
    void unityInitialize()
    {
        
        CLog.Log("[Spin][unityInitialize]:unityInitialize() entry1");

        mChordWrapper = ChordUnity.getObject();
        
        CLog.Log("[Spin][unityInitialize]:unityInitialize() entry2");
        
        
        mWiFiDirectChordWrapper = ChordUnity.getObject();
        
        CLog.Log("[Spin][unityInitialize]:unityInitialize() entry3");
        
        
        mMobileAPChordWrapper = ChordUnity.getObject();
        
        CLog.Log("[Spin][unityInitialize]:unityInitialize() entry4");
        
        
        mChordWrapper.setNetworkListener(this);
        CLog.Log("[Spin][unityInitialize]:Start() Exit 1");
        
        mChordWrapper.setTempDirectory(chordFilePath);  
        mChordWrapper.setSmartDiscoveryEnabled(isSmartDiscoveryEnabled);
        
        mWiFiDirectChordWrapper.setTempDirectory(chordFilePath);        
        mWiFiDirectChordWrapper.setSmartDiscoveryEnabled(isSmartDiscoveryEnabled);
        
        mMobileAPChordWrapper.setTempDirectory(chordFilePath);  
        mMobileAPChordWrapper.setSmartDiscoveryEnabled(isSmartDiscoveryEnabled);        

        checkInterfaces();  
        // determine whether phone or tablet based on screensize
        CLog.Log("[Spin][unityInitialize]:Screen Size " + Screen.width + "x" + Screen.height);
        if (Screen.width > 1000 || Screen.height > 1500)
            mul = 2;

        windowRect = new Rect(50 * mul, 50 * mul, 300 * mul, 400 * mul);

        CLog.Log("[Spin][checkInterfaces]:Start() Exit 2");
    }

    void checkInterfaces()
    {
        CLog.Log("[Spin][checkInterfaces]:Start");
        //check for avaialble Interfaces

        List<Int32> interfaces = mChordWrapper.getAvailableInterfaceTypes();
        CLog.Log("[Spin][checkInterfaces]after mChordWrapper.getAvailableInterfaceTypes()");
        availInterfaces.Clear();
        if (interfaces == null)
            CLog.Log("[Spin][checkInterfaces]: interfaces is null");
        if (interfaces.Count == 0)
        {
            isInterfaceAvailable = false;
            CLog.Log("[Spin][checkInterfaces]: interfaces are not avaialble");
        }
        else
        {
            isInterfaceAvailable = true;
            CLog.Log("[Spin][checkInterfaces]: interfaces avaialble: " + interfaces.Count);

            // add available interfaces to available intefaces list
            for (int i = 0; i < interfaces.Count; i++)
            { // Loop through List with for

                if (interfaces [i] == ChordUnityConstants.INTERFACE_TYPE_WIFI)
                {
                    availInterfaces.Add(ChordUnityConstants.INTERFACE_TYPE_WIFI);
                    CLog.Log("[Spin][checkInterfaces]: interfacs avaialble :Added wifi");        
                }
                else if (interfaces [i] == ChordUnityConstants.INTERFACE_TYPE_WIFIAP)
                {

                    availInterfaces.Add(ChordUnityConstants.INTERFACE_TYPE_WIFIAP);
                    CLog.Log("[Spin][checkInterfaces]: interfacs avaialble :Added wifiap");
                }
                else if (interfaces [i] == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
                {

                    availInterfaces.Add(ChordUnityConstants.INTERFACE_TYPE_WIFIP2P);              
                    CLog.Log("[Spin][checkInterfaces]: interfacs avaialble :Added wifip2p");
                }

            }

        }
    }

    // start the chord service

    void startChord()
    {
        if (availInterfaces.Contains(ChordUnityConstants.INTERFACE_TYPE_WIFI) && !isWiFiStarted && !isWiFiDisconnected)
        {
            if (wifiLsnr == null)           
                wifiLsnr = new Listener(this, ChordUnityConstants.INTERFACE_TYPE_WIFI); 
            wifiPrivateDataHistory += "\n[A] Start Chord!";
            int ret = mChordWrapper.start(ChordUnityConstants.INTERFACE_TYPE_WIFI, wifiLsnr, wifiLsnr);
            wifiPrivateDataHistory += "\n$$ start(Wi-Fi)";
            if (ret != 0)
                wifiPrivateDataHistory += "\n$$ error(Wi-Fi) : " + getErrorString(ret);

            
        }

        if (availInterfaces.Contains(ChordUnityConstants.INTERFACE_TYPE_WIFIP2P) && !isWiFiDirectStarted && !isWiFiDirectDisconnected)
        {
            if (wifiDirectLsnr == null)         
                wifiDirectLsnr = new Listener(this, ChordUnityConstants.INTERFACE_TYPE_WIFIP2P);    
            wifiDirectPrivateDataHistory += "\n[A] Start Chord!";
            int ret = mWiFiDirectChordWrapper.start(ChordUnityConstants.INTERFACE_TYPE_WIFIP2P, wifiDirectLsnr, wifiDirectLsnr);  
            wifiDirectPrivateDataHistory += "\n$$ start(Wi-Fi Direct)";
            if (ret != 0)
                wifiDirectPrivateDataHistory += "\n$$ error(Wi-Fi Direct) : " + getErrorString(ret);
        }

        if (availInterfaces.Contains(ChordUnityConstants.INTERFACE_TYPE_WIFIAP) && !isWiFiAPStarted && !isWiFiAPDisconnected)
        {
            if (wifiAPLsnr == null)         
                wifiAPLsnr = new Listener(this, ChordUnityConstants.INTERFACE_TYPE_WIFIAP); 
            wifiAPPrivateDataHistory += "\n[A] Start Chord!";
            int ret = mMobileAPChordWrapper.start(ChordUnityConstants.INTERFACE_TYPE_WIFIAP, wifiAPLsnr, wifiAPLsnr);
            wifiAPPrivateDataHistory += "\n$$ start(Mobile AP)";
            if (ret != 0)
                wifiDirectPrivateDataHistory += "\n$$ error(Mobile AP) : " + getErrorString(ret);
            
        }
    }
    
    void stopChord()
    {       
        if (isWiFiStarted)
            mChordWrapper.stop();
        if (isWiFiDirectStarted)
            mWiFiDirectChordWrapper.stop();
        if (isWiFiAPStarted)
            mMobileAPChordWrapper.stop();
        mNodeList.Clear();
    }
    

    // The OnGUI() function gets called every frame as long as the containing script is enabled 
    void OnGUI()
    {        
        if (isUnityInitialized == false)
        {
            isUnityInitialized = true;
            unityInitialize();          
        }

        if (windowArea == 0)
        {
            wifiPrivateDataHistory = "";
            wifiDirectPrivateDataHistory = "";
            wifiAPPrivateDataHistory = "";

            var x = 70;

            var y = 250;
            
            Texture2D rowTexture = Resources.Load("row2") as Texture2D;
            
            var BtStyleChoice = new GUIStyle(GUI.skin.button);
            BtStyleChoice.normal.textColor = Color.white;   
            BtStyleChoice.normal.background = rowTexture;
            BtStyleChoice.alignment = TextAnchor.MiddleLeft;
            BtStyleChoice.imagePosition = ImagePosition.ImageAbove;
            BtStyleChoice.fontSize = 20 * mul;          
            
            string[] SelStrings = {
                                "Hello Chord",
                                "Send Files",
                                "Use Secure Channel",
                                "Udp Framework",
                                "License"
                        };
            choice = GUI.SelectionGrid(new Rect(x, y, Screen.width - 140, Screen.height / 3), -1, SelStrings, 1, BtStyleChoice);          
            switch (choice)
            {
                case 0:
                    windowArea = 1;
                    break;
                case 1:
                    windowArea = 2;
                    startChord();
                    break;
                case 2:
                    mChordWrapper.setSecureModeEnabled(true);
                    mWiFiDirectChordWrapper.setSecureModeEnabled(true);
                    mMobileAPChordWrapper.setSecureModeEnabled(true);
                    windowArea = 3;
                    startChord();
                    break;
                case 3:
                    windowArea = 4;
                    startChord();
                    break;
                case 4:
                    mChordWrapper.viewLicense();
                    break;
            }
        }
        else if (windowArea == 1)
        {
            showInterfaces();  

            showLogViewer();            
        }
        else if (windowArea == 2)
        {  
            showInterfaces();   

            showJoinedNodeList();   

            sendFile();

            multiSendFile();
            
            showLogViewer();    
        }
        else if (windowArea == 3)
        {
            showInterfaces(); 
            
            joinChannel();
            
            showJoinedNodeList();   

            showLogViewer();    
        }
        else if (windowArea == 4)
        {
            showHelpWindow();
        }
            
    }

    // Update is called once per frame

    void Update()
    {
        // rotate at 90 degrees per second
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // Call Unity's Quit() to exit application
            if (windowArea == 0)
            {
                mChordWrapper.close();
                mWiFiDirectChordWrapper.close();
                mMobileAPChordWrapper.close();
                
                mChordWrapper = null;
                mWiFiDirectChordWrapper = null;
                mMobileAPChordWrapper = null;

                Application.Quit();
            }
            else
            {
                if (windowArea == 3)
                {
                    mChordWrapper.setSecureModeEnabled(false);
                    mWiFiDirectChordWrapper.setSecureModeEnabled(false);
                    mMobileAPChordWrapper.setSecureModeEnabled(false);
                }
                stopChord();
                clearData();
                windowArea = 0;
            }
         
        }
        if (isWiFiStarted)
        {
            myWifiNodeCube.GetChild(0).Rotate(Vector3.up * Time.deltaTime * 90);
        }
        if (isWiFiDirectStarted)
        {
            myWifiDirectNodeCube.GetChild(0).Rotate(Vector3.up * Time.deltaTime * 90);
        }
        if (isWiFiAPStarted)
        {
            myWifiAPNodeCube.GetChild(0).Rotate(Vector3.up * Time.deltaTime * 90);
        }
        
        if (windowArea == 5)
        {
            if (Input.GetButton("Fire1"))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit) && hit.collider.tag.Equals("My Node"))
                {
                    Transform node = hit.collider.transform.parent;
                    node.GetChild(0).position = new Vector3(hit.point.x, hit.point.y, 0);
                    node.GetChild(1).position = new Vector3(hit.point.x, hit.point.y - 0.5f, 0);
                    sendUdpPositionToAll(hit.point, node.name);
                }
            }
        }
    }
            
    // Show the interfaces
    public void showInterfaces()
    {
        var x = 0;
        var y = 25;
        Texture2D lineTexture = Resources.Load("row3") as Texture2D;
        GUIStyle style = new GUIStyle();
        style.normal.background = lineTexture;
        GUI.Box(new Rect(x, y, Screen.width, 5 * mul), new GUIContent(""), style);
        
        
        int width = Screen.width / 3 - 10 * mul;
        int height = 40 * mul;
    
        var Tx = new GUIStyle(GUI.skin.label);     
        Tx.onHover.textColor = Color.white;
        Tx.fontSize = 18 * mul;

        Texture2D rowTexture = Resources.Load("row2") as Texture2D;

        var BtStyle = new GUIStyle(GUI.skin.button);
        BtStyle.normal.textColor = Color.white;   
        BtStyle.normal.background = rowTexture;
        BtStyle.alignment = TextAnchor.MiddleCenter;
        BtStyle.imagePosition = ImagePosition.ImageAbove;
        BtStyle.fontSize = 20 * mul;   
    
        Texture2D myTexture1 = Resources.Load("accept") as Texture2D;
        Texture2D myTexture2 = Resources.Load("cancel") as Texture2D;
             
        x = x + 5;
        y = y + 10 * mul;
        
        GUI.Label(new Rect(x + 50 * mul, y, 100 * mul, 30 * mul), "Wi-Fi", Tx);
        if (availInterfaces.Contains(ChordUnityConstants.INTERFACE_TYPE_WIFI))
        {       
            GUI.DrawTexture(new Rect(x, y + 10 * mul, 30 * mul, 30 * mul), myTexture1);   
            GUI.Label(new Rect(x + 50 * mul, y + 25 * mul, 50 * mul, 30 * mul), "ON", Tx);
        }
        else
        {
            GUI.DrawTexture(new Rect(x, y + 10 * mul, 30 * mul, 30 * mul), myTexture2);
            GUI.Label(new Rect(x + 50, y + 25 * mul, 50 * mul, 30 * mul), "OFF", Tx);
            GUI.enabled = false;
        }       

        if (choice == 0)
        {
            String text = "";           
            if (isWiFiStarted)
            {   
                text = "Stop";              
            }
            else
            {           
                text = "Start";
            }
            if (GUI.Button(new Rect(x, y + 50 * mul, width, height), text, BtStyle))
            {
                if (isWiFiStarted)
                {
                    wifiPrivateDataHistory += "\n[B] Stop Chord!";
                    mChordWrapper.stop();
                    wifiPrivateDataHistory += "\n$$ stop(Wi-Fi)";
                }
                else
                {
                    if (wifiLsnr == null)           
                        wifiLsnr = new Listener(this, ChordUnityConstants.INTERFACE_TYPE_WIFI); 
                    wifiPrivateDataHistory += "\n[A] Start Chord!";
                    mChordWrapper.start(ChordUnityConstants.INTERFACE_TYPE_WIFI, wifiLsnr, wifiLsnr);
                    wifiPrivateDataHistory += "\n$$ start(Wi-Fi)";
                    
                }
            }
        }               
        GUI.enabled = true;         
        x = x + width + 10 * mul;
        
        GUI.Label(new Rect(x + 50 * mul, y, 100 * mul, 30 * mul), "Wi-Fi Direct", Tx); 
        if (availInterfaces.Contains(ChordUnityConstants.INTERFACE_TYPE_WIFIP2P))
        {
            GUI.DrawTexture(new Rect(x, y + 10 * mul, 30 * mul, 30 * mul), myTexture1);   
            GUI.Label(new Rect(x + 50 * mul, y + 25 * mul, 50 * mul, 30 * mul), "ON", Tx);
        }
        else
        {
            GUI.DrawTexture(new Rect(x, y + 10 * mul, 30 * mul, 30 * mul), myTexture2);
            GUI.Label(new Rect(x + 50 * mul, y + 25 * mul, 50 * mul, 30 * mul), "OFF", Tx);
            GUI.enabled = false;
        }

        if (choice == 0)
        {
            String text1 = "";              
            if (isWiFiDirectStarted)
            {       
                text1 = "Stop";         
            }
            else
            {           
                text1 = "Start";
            }
            if (GUI.Button(new Rect(x, y + 50 * mul, width, height), text1, BtStyle))
            {
                if (isWiFiDirectStarted)
                {
                    wifiDirectPrivateDataHistory += "\n[B] Stop Chord!";
                    mWiFiDirectChordWrapper.stop();
                    wifiDirectPrivateDataHistory += "\n$$ stop(Wi-Fi Direct)";
                }
                else
                {
                    if (wifiDirectLsnr == null)         
                        wifiDirectLsnr = new Listener(this, ChordUnityConstants.INTERFACE_TYPE_WIFIP2P);    
                    wifiDirectPrivateDataHistory += "\n[A] Start Chord!";
                    mWiFiDirectChordWrapper.start(ChordUnityConstants.INTERFACE_TYPE_WIFIP2P, wifiDirectLsnr, wifiDirectLsnr);  
                    wifiDirectPrivateDataHistory += "\n$$ start(Wi-Fi Direct)";
                }
            }       
        }

        GUI.enabled = true;
        
        x = x + width + 10 * mul;
        
        GUI.Label(new Rect(x + 50 * mul, y, 100 * mul, 30 * mul), "Mobile AP", Tx);   
        if (availInterfaces.Contains(ChordUnityConstants.INTERFACE_TYPE_WIFIAP))
        {   
            GUI.DrawTexture(new Rect(x, y + 10 * mul, 30 * mul, 30 * mul), myTexture1);   
            GUI.Label(new Rect(x + 50 * mul, y + 25 * mul, 50 * mul, 30 * mul), "ON", Tx);
        }
        else
        {
            GUI.DrawTexture(new Rect(x, y + 10 * mul, 30 * mul, 30 * mul), myTexture2);
            GUI.Label(new Rect(x + 50 * mul, y + 25 * mul, 50 * mul, 30 * mul), "OFF", Tx);
            GUI.enabled = false;
        }   

        if (choice == 0)
        {
            String text3 = "";
            if (isWiFiAPStarted)
            {   
                text3 = "Stop";
                
            }
            else
            {           
                text3 = "Start";
            }
            if (GUI.Button(new Rect(x, y + 50 * mul, width, height), text3, BtStyle))
            {
                if (isWiFiAPStarted)
                {
                    wifiAPPrivateDataHistory += "\n[B] Stop Chord!";
                    mMobileAPChordWrapper.stop();
                    wifiAPPrivateDataHistory += "\n$$ stop(Mobile AP)";
                }
                else
                {
                    if (wifiAPLsnr == null)         
                        wifiAPLsnr = new Listener(this, ChordUnityConstants.INTERFACE_TYPE_WIFIAP); 
                    wifiAPPrivateDataHistory += "\n[A] Start Chord!";
                    mMobileAPChordWrapper.start(ChordUnityConstants.INTERFACE_TYPE_WIFIAP, wifiAPLsnr, wifiAPLsnr);
                    wifiAPPrivateDataHistory += "\n$$ start(Mobile AP)";
                    
                }
            }
        }

        GUI.enabled = true;

        if (choice == 0)
        {
            y = y + 100 * mul;
        }
        else
        {
            y = y + 50 * mul;
        }

        GUI.Label(new Rect(5 * mul, y, 500 * mul, 100 * mul), "My Node Name:", Tx);
        if (startedCount > 0)
        {
            GUI.Label(new Rect(150 * mul, y, 500 * mul, 100 * mul), myNodeName, Tx);
        }
        y = y + 25 * mul;
        
        GUI.Box(new Rect(0, y, Screen.width, 5 * mul), new GUIContent(""), style);
        
        y_cor = y;
    }

    public void showJoinedNodeList()
    {
        var x = 10;
        var y = y_cor + 10 * mul;

        Texture2D lineTexture = Resources.Load("row3") as Texture2D;
        GUIStyle style = new GUIStyle();
        style.normal.background = lineTexture;
        Texture2D checkTrue = Resources.Load("true") as Texture2D;
        Texture2D checkFalse = Resources.Load("false") as Texture2D;        
        var Tx = new GUIStyle(GUI.skin.label);     
        Tx.onHover.textColor = Color.white;
        Tx.fontSize = 18 * mul;     
        
        GUI.Label(new Rect(x, y, 500 * mul, 30 * mul), "Joined Node List:", Tx);
        y = y + 50 * mul;
        x = x + 80 * mul;
        if (mNodeList.Count != 0)
        {
            for (int i =0; i < mNodeList.Count; i++)
            {
                GUI.Label(new Rect(x, y, 200 * mul, 30 * mul), mNodeList [i].nodeName, Tx);
                string text = "[" + mNodeList [i].interfaceName + "]";
                GUI.Label(new Rect(x + 200 * mul, y, 500 * mul, 30 * mul), text, Tx);
                if (choice == 1)
                {
                    Texture2D content1;             
                    if (mNodeList [i].bChecked)
                    {
                        content1 = checkTrue;
                    }
                    else
                    {
                        content1 = checkFalse;      
                    }   
                    GUIStyle Bt = new GUIStyle();
                    Bt.normal.background = content1;            
                    if (GUI.Button(new Rect(Screen.width - 170 * mul, y, 30 * mul, 30 * mul), "", Bt))
                    {
                        bool temp = !mNodeList [i].bChecked;
                        mNodeList [i].setCheckMode(temp);
                        //mNodeList[i].setCheckMode(!mNodeList[i].bChecked);                    
                    }
                }
                y = y + 35 * mul;
                GUI.Box(new Rect(x, y, Screen.width - 220 * mul, 5 * mul), new GUIContent(""), style);
                y = y + 10 * mul;
            }
        }
    }
   
    public void showLogViewer()
    {   
        var x = 10 * mul;
        var y = Screen.height / 2;//y_cor;
        if (windowArea == 1)
            y = y_cor + 10 * mul;
        var Tx = new GUIStyle(GUI.skin.label);     
        Tx.onHover.textColor = Color.white;
        Tx.fontSize = 18 * mul; 
        GUI.Label(new Rect(x, y, 150 * mul, 30 * mul), "Log Viewer", Tx);
        
        var TextAreaStyle1 = new GUIStyle(GUI.skin.textArea);
        TextAreaStyle1.normal.textColor = Color.white;
        TextAreaStyle1.fontSize = 16 * mul;
        TextAreaStyle1.wordWrap = true;     
        
        Texture2D rowTexture = Resources.Load("row2") as Texture2D;
        
        var BtStyle = new GUIStyle(GUI.skin.button);
        BtStyle.normal.textColor = Color.white;   
        BtStyle.normal.background = rowTexture;
        BtStyle.alignment = TextAnchor.MiddleCenter;
        BtStyle.imagePosition = ImagePosition.ImageAbove;
        BtStyle.fontSize = 20 * mul;  
        
        int height = 10 * mul;
     
        if (windowArea == 1)
            height = (Screen.height - y) / 2 - 30;
        else
            height = (Screen.height / 3) / 2 - 30;

        if (!wifiPrivateDataHistory.Equals(""))
        {   
            GUI.enabled = false;
            GUI.Label(new Rect(Screen.width - 220 * mul, y, 150 * mul, 30 * mul), "Wi-Fi", Tx);
                    
            var ScrollStyle = GUI.skin.verticalScrollbar;
            GUI.skin.verticalScrollbar = GUIStyle.none; 
            GUILayout.BeginArea(new Rect(x + 10 * mul, y + 30 * mul, Screen.width - 40 * mul, height));
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, ScrollStyle);
                    
            GUI.skin.textArea = TextAreaStyle1;
                    
            GUILayout.TextArea(wifiPrivateDataHistory);
            GUILayout.EndScrollView();
            GUILayout.EndArea();            
            GUI.enabled = true;        
                    
            if (GUI.Button(new Rect(Screen.width - 120 * mul, y, 100 * mul, 25 * mul), "Clear", BtStyle))
            {
                wifiPrivateDataHistory = "";
            }
            y = y + height + 30 * mul;
        }
                    
        if (!wifiDirectPrivateDataHistory.Equals(""))
        {   
            GUI.enabled = false;
            GUI.Label(new Rect(Screen.width - 220 * mul, y, 150 * mul, 30 * mul), "Wi-Fi Direct", Tx);
            var ScrollStyle = GUI.skin.verticalScrollbar;
            GUI.skin.verticalScrollbar = GUIStyle.none; 
            GUILayout.BeginArea(new Rect(x + 10 * mul, y + 30 * mul, Screen.width - 40 * mul, height));
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, ScrollStyle);
                    
            GUI.skin.textArea = TextAreaStyle1;
                    
            GUILayout.TextArea(wifiDirectPrivateDataHistory);
            GUILayout.EndScrollView();
            GUILayout.EndArea();            
            GUI.enabled = true;
                    
            if (GUI.Button(new Rect(Screen.width - 120 * mul, y, 100 * mul, 25 * mul), "Clear", BtStyle))
            {
                wifiDirectPrivateDataHistory = "";
            }
            y = y + height + 30 * mul;
        }
                           
        if (!wifiAPPrivateDataHistory.Equals(""))
        {   
            GUI.enabled = false;
            GUI.Label(new Rect(Screen.width - 220 * mul, y, 150 * mul, 30 * mul), "Mobile AP", Tx);
            var ScrollStyle = GUI.skin.verticalScrollbar;
            GUI.skin.verticalScrollbar = GUIStyle.none; 
            GUILayout.BeginArea(new Rect(x + 10 * mul, y + 30 * mul, Screen.width - 40 * mul, height));
            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, ScrollStyle);
                    
            GUI.skin.textArea = TextAreaStyle1;
                    
            GUILayout.TextArea(wifiAPPrivateDataHistory);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUI.enabled = true;
                                                
            if (GUI.Button(new Rect(Screen.width - 120 * mul, y, 100 * mul, 25 * mul), "Clear", BtStyle))
            {
                wifiAPPrivateDataHistory = "";
            }
        }

    }
    // join channel UI
    public void joinChannel()
    {    
        var x = 5 * mul;
        var y = y_cor + 5 * mul;
    
        Texture2D lineTexture = Resources.Load("row3") as Texture2D;
        GUIStyle style = new GUIStyle();
        style.normal.background = lineTexture;
   
        var Tx = new GUIStyle(GUI.skin.label);     
        Tx.onHover.textColor = Color.white;
        Tx.fontSize = 18 * mul; 

        Texture2D rowTexture = Resources.Load("row2") as Texture2D;
        
        var BtStyle = new GUIStyle(GUI.skin.button);
        BtStyle.normal.textColor = Color.white;   
        BtStyle.normal.background = rowTexture;
        BtStyle.alignment = TextAnchor.MiddleCenter;
        BtStyle.imagePosition = ImagePosition.ImageAbove;
        BtStyle.fontSize = 20 * mul; 

        var TfStyle = new GUIStyle(GUI.skin.textField);     
        TfStyle.onHover.textColor = Color.white;
        TfStyle.fontSize = 18 * mul; 

        var TgStyle = new GUIStyle(GUI.skin.toggle);     
        TgStyle.onHover.textColor = Color.white;
        TgStyle.fontSize = 18 + (9 * mul - 9); 
    
        int width = Screen.width / 3 - 10 * mul;
        int height = 40 * mul;
    
        GUI.Label(new Rect(x, y + 50 * mul, width, 40 * mul), "Channel Name:", Tx);
        
        String textCh = "";
        if (!isWiFiPrivateChannelJoined)
        {
            textCh = "Join Channel";
        }
        else
        {
            textCh = "Leave Channel";
            GUI.enabled = false;
        }
        if (!isWiFiStarted)
            GUI.enabled = false;
        if (privateChannelWiFi.StartsWith("#"))
            isWiFiSecureEnabled = true;
        else
            isWiFiSecureEnabled = false;           
        isWiFiSecureEnabled = GUI.Toggle(new Rect(x, y + height * 3 + 5 * mul, width, 30 * mul), isWiFiSecureEnabled, "Use Secure Channel", TgStyle);
        if (isWiFiSecureEnabled && !privateChannelWiFi.StartsWith("#"))
            privateChannelWiFi = "#" + privateChannelWiFi;
        else if (!isWiFiSecureEnabled && privateChannelWiFi.StartsWith("#"))
            privateChannelWiFi = privateChannelWiFi.Substring(1);
        privateChannelWiFi = GUI.TextField(new Rect(x, y + height * 2 + 5 * mul, width, 30 * mul), privateChannelWiFi, 4096, TfStyle);

        if (privateChannelWiFi.Equals(""))
            privateChannelWiFi = UNITY_SECURE_TEST_CHANNEL;
        else if (privateChannelWiFi.Equals("#"))
            privateChannelWiFi = ChordUnityConstants.SECURE_PREFIX + UNITY_SECURE_TEST_CHANNEL;

        GUI.enabled = true;
        if (!isWiFiStarted)
            GUI.enabled = false;
        if (GUI.Button(new Rect(x, y, width, height), textCh, BtStyle))
        {
            if (isWiFiPrivateChannelJoined)
            {
                mChordWrapper.leaveChannel(privateChannelWiFi);
                wifiPrivateDataHistory += "\n$$ leaveChannel() ";
                scrollPosition1.y = Mathf.Infinity;
                lock (mNodeList)
                {
                    for (int i = 0; i < mNodeList.Count; i++)
                    {
                        Node temp = mNodeList [i];
                        if (temp.interfaceName.Equals("Wi-Fi"))
                        {
                            mNodeList.Remove(temp);
                            i--;
                        }
                    
                    }
                }
                GameObject[] joinedNodeCubeList = GameObject.FindGameObjectsWithTag("Joined Node");
                String stoppedInterfaceName = "[" + getInterfaceName(ChordUnityConstants.INTERFACE_TYPE_WIFI) + "]";
                for (int i = 0; i < joinedNodeCubeList.Length; i++)
                {
                    if (joinedNodeCubeList [i].transform.name.Contains(stoppedInterfaceName))
                    {
                        Destroy(joinedNodeCubeList [i]);
                    }
                }
                isWiFiPrivateChannelJoined = false;
            }
            else
            {
                if (privateChannelWiFi.StartsWith("#"))
                {
                    isWiFiSecureEnabled = true;
                }
                else if (isWiFiSecureEnabled)
                {
                    privateChannelWiFi = ChordUnityConstants.SECURE_PREFIX + privateChannelWiFi;
                }
                isWiFiPrivateChannelJoined = mChordWrapper.joinChannel(privateChannelWiFi);
                wifiPrivateDataHistory += "\n$$ joinChannel(" + privateChannelWiFi + ") ";
                scrollPosition1.y = Mathf.Infinity;       
            }
        }       
        GUI.enabled = true;         
        x = x + width + 10 * mul;         

        GUI.Label(new Rect(x, y + 50 * mul, width, 40 * mul), "Channel Name:", Tx);
        String textCh1 = "";                
        if (!isWiFiDirectPrivateChannelJoined)
        {
            textCh1 = "Join Channel";
        }
        else
        {
            textCh1 = "Leave Channel";
            GUI.enabled = false;
        }
        if (!isWiFiDirectStarted)
            GUI.enabled = false;
        if (privateChannelWiFiDirect.StartsWith("#"))
            isWiFiDirectSecureEnabled = true;
        else
            isWiFiDirectSecureEnabled = false;
        isWiFiDirectSecureEnabled = GUI.Toggle(new Rect(x, y + height * 3 + 5 * mul, width, 30 * mul), isWiFiDirectSecureEnabled, "Use Secure Channel", TgStyle);
        if (isWiFiDirectSecureEnabled && !privateChannelWiFiDirect.StartsWith("#"))
            privateChannelWiFiDirect = "#" + privateChannelWiFiDirect;
        else if (!isWiFiDirectSecureEnabled && privateChannelWiFiDirect.StartsWith("#"))
            privateChannelWiFiDirect = privateChannelWiFiDirect.Substring(1);
        privateChannelWiFiDirect = GUI.TextField(new Rect(x, y + height * 2 + 5 * mul, width, 30 * mul), privateChannelWiFiDirect, 4096, TfStyle);

        if (privateChannelWiFiDirect.Equals(""))
            privateChannelWiFiDirect = UNITY_SECURE_TEST_CHANNEL;
        else if (privateChannelWiFiDirect.Equals("#"))
            privateChannelWiFiDirect = ChordUnityConstants.SECURE_PREFIX + UNITY_SECURE_TEST_CHANNEL;

        GUI.enabled = true;
        if (!isWiFiDirectStarted)
            GUI.enabled = false;
        if (GUI.Button(new Rect(x, y, width, height), textCh1, BtStyle))
        {
            if (isWiFiDirectPrivateChannelJoined)
            {
                mWiFiDirectChordWrapper.leaveChannel(privateChannelWiFiDirect);
                wifiDirectPrivateDataHistory += "\n$$   leaveChannel() ";
                scrollPosition2.y = Mathf.Infinity;
                lock (mNodeList)
                {
                    for (int i = 0; i < mNodeList.Count; i++)
                    {
                        Node temp = mNodeList [i];
                        if (temp.interfaceName.Equals("Wi-Fi Direct"))
                        {
                            mNodeList.Remove(temp);
                            i--;
                        }
                    
                    }
                }
                GameObject[] joinedNodeCubeList = GameObject.FindGameObjectsWithTag("Joined Node");
                String stoppedInterfaceName = "[" + getInterfaceName(ChordUnityConstants.INTERFACE_TYPE_WIFIP2P) + "]";
                for (int i = 0; i < joinedNodeCubeList.Length; i++)
                {
                    if (joinedNodeCubeList [i].transform.name.Contains(stoppedInterfaceName))
                    {
                        Destroy(joinedNodeCubeList [i]);
                    }
                }
                isWiFiDirectPrivateChannelJoined = false;
            }
            else
            {
                if (privateChannelWiFiDirect.StartsWith("#"))
                {
                    isWiFiDirectSecureEnabled = true;
                }
                else if (isWiFiDirectSecureEnabled)
                {
                    privateChannelWiFiDirect = ChordUnityConstants.SECURE_PREFIX + privateChannelWiFiDirect;
                }
                isWiFiDirectPrivateChannelJoined = mWiFiDirectChordWrapper.joinChannel(privateChannelWiFiDirect);
                wifiDirectPrivateDataHistory += "\n$$   joinChannel(" + privateChannelWiFiDirect + ") ";
                scrollPosition2.y = Mathf.Infinity;
            }
        }       
        GUI.enabled = true;
        
        x = x + width + 10 * mul;         

        GUI.Label(new Rect(x, y + 50 * mul, width, 40 * mul), "Channel Name:", Tx);
        String textCh3 = "";
        if (!isWiFiAPPrivateChannelJoined)
        {
            textCh3 = "Join Channel";
        }
        else
        {           
            textCh3 = "Leave Channel";  
            GUI.enabled = false;
        }
        if (!isWiFiAPStarted)
            GUI.enabled = false;
        if (privateChannelWiFiAP.StartsWith("#"))
            isWiFiAPSecureEnabled = true;
        else
            isWiFiAPSecureEnabled = false;
        isWiFiAPSecureEnabled = GUI.Toggle(new Rect(x, y + height * 3 + 5 * mul, width, 30 * mul), isWiFiAPSecureEnabled, "Use Secure Channel", TgStyle);
        if (isWiFiAPSecureEnabled && !privateChannelWiFiAP.StartsWith("#"))
            privateChannelWiFiAP = "#" + privateChannelWiFiAP;
        else if (!isWiFiAPSecureEnabled && privateChannelWiFiAP.StartsWith("#"))
            privateChannelWiFiAP = privateChannelWiFiAP.Substring(1);
        privateChannelWiFiAP = GUI.TextField(new Rect(x, y + height * 2 + 5 * mul, width, 30 * mul), privateChannelWiFiAP, 4096, TfStyle);

        if (privateChannelWiFiAP.Equals(""))
            privateChannelWiFiAP = UNITY_SECURE_TEST_CHANNEL;
        else if (privateChannelWiFiAP.Equals("#"))
            privateChannelWiFiAP = ChordUnityConstants.SECURE_PREFIX + UNITY_SECURE_TEST_CHANNEL;

        GUI.enabled = true;
        if (!isWiFiAPStarted)
            GUI.enabled = false;
        if (GUI.Button(new Rect(x, y, width, height), textCh3, BtStyle))
        {
            if (isWiFiAPPrivateChannelJoined)
            {
                mMobileAPChordWrapper.leaveChannel(privateChannelWiFiAP);
                wifiAPPrivateDataHistory += "\n$$   leaveChannel() ";
                scrollPosition3.y = Mathf.Infinity;
              
                lock (mNodeList)
                {
                    for (int i = 0; i < mNodeList.Count; i++)
                    {
                        Node temp = mNodeList [i];
                        if (temp.interfaceName.Equals("Mobile AP"))
                        {
                            mNodeList.Remove(temp);
                            i--;
                        }
                    
                    }
                }
                GameObject[] joinedNodeCubeList = GameObject.FindGameObjectsWithTag("Joined Node");
                String stoppedInterfaceName = "[" + getInterfaceName(ChordUnityConstants.INTERFACE_TYPE_WIFIAP) + "]";
                for (int i = 0; i < joinedNodeCubeList.Length; i++)
                {
                    if (joinedNodeCubeList [i].transform.name.Contains(stoppedInterfaceName))
                    {
                        Destroy(joinedNodeCubeList [i]);
                    }
                }
                isWiFiAPPrivateChannelJoined = false;
            }
            else
            {   
                if (privateChannelWiFiAP.StartsWith("#"))
                {
                    isWiFiAPSecureEnabled = true;
                }
                else if (isWiFiAPSecureEnabled)
                {
                    privateChannelWiFiAP = ChordUnityConstants.SECURE_PREFIX + privateChannelWiFiAP;
                }
                isWiFiAPPrivateChannelJoined = mMobileAPChordWrapper.joinChannel(privateChannelWiFiAP);
                wifiAPPrivateDataHistory += "\n$$   joinChannel(" + privateChannelWiFiAP + ") ";
                scrollPosition3.y = Mathf.Infinity;
            }
        }
        y = y + height * 3 + 20 * mul;    
        GUI.Box(new Rect(0, y, Screen.width, 5 * mul), new GUIContent(""), style);
    
        y_cor = y;
        GUI.enabled = true;
    }

    public void sendUdpPosition(String nodeName, Vector3 pos, int interfaceType)
    {
        if (mNodeList.Count != 0)
        {
            CLog.Log("[Spin][sendUdpPosition]" + pos.x.ToString() + ", " + pos.y.ToString());

            Byte[][] bytes = new Byte[1][];
            bytes [0] = Encoding.UTF8.GetBytes("CubePosition : x[" + pos.x.ToString() + "]y[" + pos.y.ToString() + "]");

            switch (interfaceType)
            {
                case ChordUnityConstants.INTERFACE_TYPE_WIFI:
                    mChordWrapper.sendUdpData(UNITY_UDP_TEST_CHANNEL, nodeName, reliableTime [reliableMode], needResponseUdp, CHORD_UNITY_MESSAGE_TYPE, bytes, "Testing");
                    break;
                case ChordUnityConstants.INTERFACE_TYPE_WIFIP2P:
                    mWiFiDirectChordWrapper.sendUdpData(UNITY_UDP_TEST_CHANNEL, nodeName, reliableTime [reliableMode], needResponseUdp, CHORD_UNITY_MESSAGE_TYPE, bytes, "Testing");
                    break;
                case ChordUnityConstants.INTERFACE_TYPE_WIFIAP:
                    mMobileAPChordWrapper.sendUdpData(UNITY_UDP_TEST_CHANNEL, nodeName, reliableTime [reliableMode], needResponseUdp, CHORD_UNITY_MESSAGE_TYPE, bytes, "Testing");
                    break;                      
            }
        } 
    }

    public void sendUdpPositionToAll(Vector3 pos, String interfaceName)
    {
        if (mNodeList.Count != 0)
        {
            CLog.Log("[Spin][sendUdpPositionToAll]" + pos.x.ToString() + ", " + pos.y.ToString());

            Byte[][] bytes = new Byte[1][];
            bytes [0] = Encoding.UTF8.GetBytes("CubePosition : x[" + pos.x.ToString() + "]y[" + pos.y.ToString() + "]");

            if (interfaceName.Equals(getInterfaceName(ChordUnityConstants.INTERFACE_TYPE_WIFI)))
            {
                mChordWrapper.sendUdpDataToAll(UNITY_UDP_TEST_CHANNEL, reliableTime [reliableMode], needResponseUdp, CHORD_UNITY_MESSAGE_TYPE, bytes, "Testing");
            }
            else if (interfaceName.Equals(getInterfaceName(ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)))
            {
                mWiFiDirectChordWrapper.sendUdpDataToAll(UNITY_UDP_TEST_CHANNEL, reliableTime [reliableMode], needResponseUdp, CHORD_UNITY_MESSAGE_TYPE, bytes, "Testing");
            }
            else if (interfaceName.Equals(getInterfaceName(ChordUnityConstants.INTERFACE_TYPE_WIFIAP)))
            {
                mMobileAPChordWrapper.sendUdpDataToAll(UNITY_UDP_TEST_CHANNEL, reliableTime [reliableMode], needResponseUdp, CHORD_UNITY_MESSAGE_TYPE, bytes, "Testing");
            }   
        }
    }

    // Send File
    public void sendFile()
    {
        var x = 20 * mul;
        var y = Screen.height / 2 - 70 * mul;   

        Texture2D rowTexture = Resources.Load("row2") as Texture2D;
        
        var BtStyle = new GUIStyle(GUI.skin.button);
        BtStyle.normal.textColor = Color.white;   
        BtStyle.normal.background = rowTexture;
        BtStyle.alignment = TextAnchor.MiddleCenter;
        BtStyle.imagePosition = ImagePosition.ImageAbove;
        BtStyle.fontSize = 20 * mul; 

        var TextStyle = new GUIStyle(GUI.skin.label);
        TextStyle.normal.textColor = Color.white;
        TextStyle.fontSize = 18 * mul;

        Texture2D lineTexture = Resources.Load("row3") as Texture2D;
        GUIStyle style = new GUIStyle();
        style.normal.background = lineTexture;
        // if the peers are available, show senddata option

        GUI.Box(new Rect(0, y, Screen.width, 5 * mul), new GUIContent(""), style);
        y = y + 5 * mul;          
        GUI.Box(new Rect(0, y + 45 * mul, Screen.width, 5 * mul), new GUIContent(""), style);
        if (!isWiFiStarted && !isWiFiDirectStarted && !isWiFiAPStarted)
            GUI.enabled = false;
        if (GUI.Button(new Rect(x, y, 80 * mul, 40 * mul), "Browse", BtStyle))
        {
            selectedFiles = null;
            fileSelected = "";

            if(isWiFiStarted)
                mChordWrapper.browseFiles();
            else if(isWiFiDirectStarted)
                mWiFiDirectChordWrapper.browseFiles();
            else if(isWiFiAPStarted)
                mMobileAPChordWrapper.browseFiles();
        }
        GUI.enabled = false;
        fileSelected = GUI.TextArea(new Rect(x + 85 * mul, y, Screen.width - 345 * mul, 40 * mul), fileSelected, 4096);
        GUI.enabled = true;
        if (mNodeList.Count == 0)
        {
            GUI.enabled = false;
        }
        if (GUI.Button(new Rect(Screen.width - 235 * mul, y, 90 * mul, 40 * mul), "SendFile", BtStyle))
        {
            CLog.Log("[Spin][SendFile]: SendFile Pressed"); 
            if (selectedFiles != null && selectedFiles.Length != 0)
            {               
                for (int i = 0; i < mNodeList.Count; i++)
                {
                    if (true == mNodeList [i].bChecked)
                    {
                        switch (mNodeList [i].interfaceName)
                        {
                            case "Wi-Fi":
                                string exId1 = mChordWrapper.sendFile(UNITY_SEND_FILE_TEST_CHANNEL, mNodeList [i].nodeName, CHORD_UNITY_FILE_TYPE, selectedFiles [0], SHARE_FILE_TIMEOUT_SECONDS);

                                if (null == exId1)
                                {           
                                    int error1 = mChordWrapper.getLastError();
                                    lock (wifiPrivateDataHistory)
                                    {
                                        wifiPrivateDataHistory += "\n$$ Send file failed, reason: " + getErrorString(error1);
                                    }
                                    scrollPosition1.y = Mathf.Infinity;
                                }
                                else
                                {
                                    lock (wifiPrivateDataHistory)
                                    {
                                        wifiPrivateDataHistory += "\n$$ Send file started, file name : " + selectedFiles [0] + ", to node : " + mNodeList [i].nodeName;
                                    }
                                    scrollPosition1.y = Mathf.Infinity;
                                }                       
                                break;

                            case "Wi-Fi Direct":
                                string exId2 = mWiFiDirectChordWrapper.sendFile(UNITY_SEND_FILE_TEST_CHANNEL, mNodeList [i].nodeName, CHORD_UNITY_FILE_TYPE, selectedFiles [0], SHARE_FILE_TIMEOUT_SECONDS);

                                if (null == exId2)
                                {
                                    int error2 = mWiFiDirectChordWrapper.getLastError();
                                    lock (wifiDirectPrivateDataHistory)
                                    {
                                        wifiDirectPrivateDataHistory += "\n$$ Send file failed, reason: " + getErrorString(error2);
                                    }
                                    scrollPosition2.y = Mathf.Infinity;
                                }
                                else
                                {
                                    lock (wifiDirectPrivateDataHistory)
                                    {
                                        wifiDirectPrivateDataHistory += "\n$$ Send file started, file name : " + selectedFiles [0] + ", to node : " + mNodeList [i].nodeName;
                                    }
                                    scrollPosition2.y = Mathf.Infinity;
                                }

                                break;

                            case "Mobile AP":
                                string exId3 = mMobileAPChordWrapper.sendFile(UNITY_SEND_FILE_TEST_CHANNEL, mNodeList [i].nodeName, CHORD_UNITY_FILE_TYPE, selectedFiles [0], SHARE_FILE_TIMEOUT_SECONDS);

                                if (null == exId3)
                                {
                                    int error3 = mMobileAPChordWrapper.getLastError();
                                    lock (wifiAPPrivateDataHistory)
                                    {
                                        wifiAPPrivateDataHistory += "\n$$ Send file failed, reason: " + getErrorString(error3);
                                    }
                                    scrollPosition3.y = Mathf.Infinity;
                                }
                                else
                                {
                                    lock (wifiAPPrivateDataHistory)
                                    {
                                        wifiAPPrivateDataHistory += "\n$$ Send file started, file name : " + selectedFiles [0] + ", to node : " + mNodeList [i].nodeName;
                                    }
                                    scrollPosition3.y = Mathf.Infinity;
                                }                           

                                break;
                        }
                    }                 
                }
                selectedFiles = null;
                fileSelected = "";
            }
        }              
        y_cor = y;
        GUI.enabled = true;
    }

    // Send multiple files

    public void multiSendFile()
    {
        var y = y_cor;

        Texture2D rowTexture = Resources.Load("row2") as Texture2D;
        
        var BtStyle = new GUIStyle(GUI.skin.button);
        BtStyle.normal.textColor = Color.white;   
        BtStyle.normal.background = rowTexture;
        BtStyle.alignment = TextAnchor.MiddleCenter;
        BtStyle.imagePosition = ImagePosition.ImageAbove;
        BtStyle.fontSize = 20 * mul;

        if (mNodeList.Count == 0)
        {
            GUI.enabled = false;
        }
        
        if (GUI.Button(new Rect(Screen.width - 140 * mul, y, 125 * mul, 40 * mul), "MultiSendFile", BtStyle))
        {
            List<String> fileList = null;
            fileList = new List<String>();

            CLog.Log("[Spin][multiSendFile]: MultiSendFile Pressed");
            
            // add the selected files to filelist

            if (selectedFiles != null && selectedFiles.Length != 0)
            {

                for (int i = 0; i < selectedFiles.Length; i++)
                {
                    fileList.Add(selectedFiles [i]);
                }
                for (int i=0; i<mNodeList.Count; i++)
                {
                    if (true == mNodeList [i].bChecked)
                    {
                        switch (mNodeList [i].interfaceName)
                        {
                            case "Wi-Fi":
                                string exId1 = mChordWrapper.sendMultiFiles(UNITY_SEND_FILE_TEST_CHANNEL, mNodeList [i].nodeName, CHORD_UNITY_FILE_TYPE, fileList, SHARE_FILE_TIMEOUT_SECONDS);

                                if (null == exId1)
                                {           
                                    int error1 = mChordWrapper.getLastError();
                                    lock (wifiPrivateDataHistory)
                                    {
                                        wifiPrivateDataHistory += "\n$$ Send file failed, reason: " + getErrorString(error1);
                                    }
                                    scrollPosition1.y = Mathf.Infinity;
                                }
                                else
                                {
                                    lock (wifiPrivateDataHistory)
                                    {
                                        wifiPrivateDataHistory += "\n$$ Send file started, file name : " + selectedFiles [0] + ", to node : " + mNodeList [i].nodeName;
                                    }
                                    scrollPosition1.y = Mathf.Infinity;
                                }                       
                                break;
                                
                            case "Wi-Fi Direct":
                                string exId2 = mWiFiDirectChordWrapper.sendMultiFiles(UNITY_SEND_FILE_TEST_CHANNEL, mNodeList [i].nodeName, CHORD_UNITY_FILE_TYPE, fileList, SHARE_FILE_TIMEOUT_SECONDS);

                                if (null == exId2)
                                {       
                                    int error2 = mWiFiDirectChordWrapper.getLastError();
                                    lock (wifiDirectPrivateDataHistory)
                                    {
                                        wifiDirectPrivateDataHistory += "\n$$ Send file failed, reason: " + getErrorString(error2);
                                    }
                                    scrollPosition2.y = Mathf.Infinity;
                                }
                                else
                                {
                                    lock (wifiDirectPrivateDataHistory)
                                    {
                                        wifiDirectPrivateDataHistory += "\n$$ Send file started, file name : " + selectedFiles [0] + ", to node : " + mNodeList [i].nodeName;
                                    }
                                    scrollPosition2.y = Mathf.Infinity;
                                }
                                    
                                break;
                            case "Mobile AP":
                                string exId3 = mMobileAPChordWrapper.sendMultiFiles(UNITY_SEND_FILE_TEST_CHANNEL, mNodeList [i].nodeName, CHORD_UNITY_FILE_TYPE, fileList, SHARE_FILE_TIMEOUT_SECONDS);

                                if (null == exId3)
                                {
            
                                    int error3 = mMobileAPChordWrapper.getLastError();
                                    lock (wifiAPPrivateDataHistory)
                                    {
                                        wifiAPPrivateDataHistory += "\n$$ Send file failed, reason: " + getErrorString(error3);
                                    }
                                    scrollPosition3.y = Mathf.Infinity;
                                }
                                else
                                {
                                    lock (wifiAPPrivateDataHistory)
                                    {
                                        wifiAPPrivateDataHistory += "\n$$ Send file started, file name : " + selectedFiles [0] + ", to node : " + mNodeList [i].nodeName;
                                    }
                                    scrollPosition3.y = Mathf.Infinity;
                                }                           
                                    
                                break;
                        }
                    }                 
                }
                    
                selectedFiles = null;
                fileSelected = "";
                fileList.Clear();

            }
        }
        GUI.enabled = true;
    }
 
    public void showHelpWindow()
    {
        var WdStyle = new GUIStyle(GUI.skin.window);     
        WdStyle.onHover.textColor = Color.white;
        WdStyle.fontSize = 18 * mul; 
        windowRect = GUI.Window(0, windowRect, doMyWindow, "Help", WdStyle);
    }

    void doMyWindow(int windowID)
    {
        var Tx = new GUIStyle(GUI.skin.label);     
        Tx.onHover.textColor = Color.white;
        Tx.fontSize = 18 * mul; 
        
        Texture2D rowTexture = Resources.Load("row2") as Texture2D;
        
        var BtStyle = new GUIStyle(GUI.skin.button);
        BtStyle.normal.textColor = Color.white;   
        BtStyle.normal.background = rowTexture;
        BtStyle.alignment = TextAnchor.MiddleCenter;
        BtStyle.imagePosition = ImagePosition.ImageAbove;
        BtStyle.fontSize = 20 * mul; 

        GUI.Label(new Rect(10 * mul, 20 * mul, 280 * mul, 350 * mul), 
                  "This menu is for testing the UDP framework.\nTouch your node cube. Then you can share your position with another node using UDP framework.\nClose this window please."
                  , Tx);
        if (GUI.Button(new Rect(10 * mul, 350 * mul, 280 * mul, 40 * mul), "Start", BtStyle))
        {
            windowArea = 5;
        }
        GUI.DragWindow();
    }

    private void clearData()
    {
        mNodeList.Clear();
        wifiPrivateDataHistory = "";
        wifiDirectPrivateDataHistory = "";
        wifiAPPrivateDataHistory = "";

        isWiFiDisconnected = false;
        isWiFiDirectDisconnected = false;
        isWiFiAPDisconnected = false;
        
        isWiFiStarted = false;
        isWiFiDirectStarted = false;
        isWiFiAPStarted = false;
        
        isWiFiPrivateChannelJoined = false;
        isWiFiDirectPrivateChannelJoined = false;
        isWiFiAPPrivateChannelJoined = false;
        
        isWiFiSecureEnabled = false;
        isWiFiDirectSecureEnabled = false;
        isWiFiAPSecureEnabled = false;
        
        wifiCount = 0;
        wifiDirectCount = 0;
        wifiAPCount = 0;     
    }

    // Interface connected

    public void onConnected(int interfaceType)
    {
        CLog.Log("[Spin][onConnected]: interface : " + interfaceType);
        checkInterfaces();  
        if (windowArea == 2 || windowArea == 3 || windowArea == 4 || windowArea == 5)
            startChord();
    }

    public String getInterfaceName(int interfacetype)
    {
        String interfaceName = "";
        switch (interfacetype)
        {
            case ChordUnityConstants.INTERFACE_TYPE_WIFI:
                {
                    interfaceName = "Wi-Fi";
                }
                break;
            case ChordUnityConstants.INTERFACE_TYPE_WIFIP2P:
                {
                    interfaceName = "Wi-Fi Direct";
                }
                break;
            case ChordUnityConstants.INTERFACE_TYPE_WIFIAP:
                {
                    interfaceName = "Mobile AP";
                }
                break;
            default:
                {
                    // invalid interface type
                    interfaceName = "Unknown";
                }
                break;
        }
        return interfaceName;
    }
    
    // Interface got disconnected

    public void onDisconnected(int interfaceType)
    {
        CLog.Log("[Spin][onDisconnected]: interface : " + interfaceType);
        checkInterfaces();
    }

   
    // Result of Browsefiles

    public void onBrowseResult(String[] fileList, int interfaceType)
    {

        selectedFiles = fileList;

        fileSelected = "";

        for (int i = 0; i < selectedFiles.Length; i++)
        {

            fileSelected += selectedFiles [i] + ",";

        }
    }
    // Chord started event

    public void onStarted(string nodeName, int reason, int interfaceType)
    {
        CLog.Log("[Spin][onStarted]: onStarted");       
        string msg = "";
        myNodeName = nodeName;
        String cubeName = nodeName + "[" + getInterfaceName(interfaceType) + "]";
        Transform newNodeCube = (Transform)Instantiate(nodeCube, new Vector3(UnityEngine.Random.Range(0.0f, 8.0f), UnityEngine.Random.Range(-8.0f, 2.0f), 0), new Quaternion());
        CLog.Log("[Spin][onStarted]: nobe created : x = " + newNodeCube.position.x + ", y = " + newNodeCube.position.y);
        newNodeCube.GetChild(0).localScale = new Vector3(1.5f, 1.5f, 1.5f);
        TextMesh newNodeCubeTextMesh = (TextMesh)newNodeCube.GetChild(1).GetComponent<TextMesh>();
        newNodeCubeTextMesh.text = cubeName;
        newNodeCubeTextMesh.color = new Color(255, 0, 0);
        newNodeCube.tag = "My Node";
        newNodeCube.name = getInterfaceName(interfaceType);
        newNodeCube.GetChild(0).tag = "My Node";
        newNodeCube.GetChild(1).tag = "My Node";
    
        if (reason == ChordUnityConstants.STARTED_BY_USER)
        {   
            msg = "STARTED_BY_USER";
            startedCount++;
        }
        else if (reason == ChordUnityConstants.STARTED_BY_RECONNECTION)
        {
            msg = "STARTED_BY_RECONNECTION";
        }

        ChordUnity tempChord = null;
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {           
            tempChord = mChordWrapper;
            isWiFiStarted = true;
            wifiPrivateDataHistory += "\n$$ [Wi-Fi] onStarted(" + msg + ")";
            scrollPosition1.y = Mathf.Infinity;
            myWifiNodeCube = newNodeCube;
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {           
            tempChord = mWiFiDirectChordWrapper;
            isWiFiDirectStarted = true;
            wifiDirectPrivateDataHistory += "\n$$   [Wi-Fi Direct] onStarted(" + msg + ")";
            scrollPosition2.y = Mathf.Infinity;
            myWifiDirectNodeCube = newNodeCube;
        }
        else
        {
            tempChord = mMobileAPChordWrapper;
            isWiFiAPStarted = true;
            wifiAPPrivateDataHistory += "\n$$   [Mobile AP] onStarted(" + msg + ")";
            scrollPosition3.y = Mathf.Infinity;
            myWifiAPNodeCube = newNodeCube;
        }       

        String channelName = "";
        if (choice == 0)
        {
            channelName = UNITY_SEND_DATA_TEST_CHANNEL;
        }
        else if (choice == 1)
        {
            channelName = UNITY_SEND_FILE_TEST_CHANNEL;
        }
        else if (choice == 3)
        {
            channelName = UNITY_UDP_TEST_CHANNEL;
        }

        if (choice == 0 || choice == 1 || choice == 3)
        {
            tempChord.joinChannel(channelName);
            if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
            {           
                wifiPrivateDataHistory += "\n$$ joinChannel() ";
                scrollPosition1.y = Mathf.Infinity;
                isWiFiPrivateChannelJoined = true;
            }
            else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
            {           
                wifiDirectPrivateDataHistory += "\n$$   joinChannel() ";
                scrollPosition2.y = Mathf.Infinity;
                isWiFiDirectPrivateChannelJoined = true;
            }
            else
            {
                wifiAPPrivateDataHistory += "\n$$   joinChannel() ";
                scrollPosition3.y = Mathf.Infinity;
                isWiFiAPPrivateChannelJoined = true;
            }
        }
    }

    // Chord stopped event

    public void onStopped(int reason, int interfaceType)
    {
        CLog.Log("[Spin][onStopped]: reason " + reason);
        String msg = "";
        if (reason == ChordUnityConstants.STOPPED_BY_USER)
        {
            startedCount--;
            msg = "STOPPED_BY_USER";
        }
        if (ChordUnityConstants.NETWORK_DISCONNECTED == reason)
        {           
            msg = "NETWORK_DISCONNECTED";
            if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
            {
                isWiFiDisconnected = true;
            }
            else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
            {
                isWiFiDirectDisconnected = true;
            }
            else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIAP)
            {
                isWiFiAPDisconnected = true;
            }
        }
        //else
        {
            // Reset the flags 
            GameObject[] joinedNodeCubeList = GameObject.FindGameObjectsWithTag("Joined Node");
            String stoppedInterfaceName = "[" + getInterfaceName(interfaceType) + "]";
            for (int i = 0; i < joinedNodeCubeList.Length; i++)
            {
                if (joinedNodeCubeList [i].transform.name.Contains(stoppedInterfaceName))
                {
                    Destroy(joinedNodeCubeList [i]);
                }
            }

            if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
            {
                isWiFiStarted = false;
                if (ChordUnityConstants.STOPPED_BY_USER == reason)
                { 
                    isWiFiPrivateChannelJoined = false;
                }
                
                wifiPrivateDataHistory += "\n$$ [Wi-Fi] onStopped(" + msg + ")";                
                scrollPosition1.y = Mathf.Infinity;
                lock (mNodeList)
                {
                    for (int i =0; i<mNodeList.Count; i++)
                    {
                        Node temp = mNodeList [i];
                        if (temp.interfaceName.Equals("Wi-Fi"))
                        {
                            mNodeList.Remove(temp);
                            i--;
                        }
                            
                    }
                }
                Destroy(myWifiNodeCube.gameObject);
            }
            else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
            {
                isWiFiDirectStarted = false;
                if (ChordUnityConstants.STOPPED_BY_USER == reason)
                { 
                    isWiFiDirectPrivateChannelJoined = false;
                }              
                wifiDirectPrivateDataHistory += "\n$$ [Wi-Fi Direct] onStopped(" + msg + ")";               
                scrollPosition2.y = Mathf.Infinity;
                lock (mNodeList)
                {
                    for (int i =0; i<mNodeList.Count; i++)
                    {
                        Node temp = mNodeList [i];
                        if (temp.interfaceName.Equals("Wi-Fi Direct"))
                        {
                            mNodeList.Remove(temp);
                            i--;
                        }
                        
                    }   
                }
                Destroy(myWifiDirectNodeCube.gameObject);
            }
            else
            {
                isWiFiAPStarted = false;

                if (ChordUnityConstants.STOPPED_BY_USER == reason)
                { 
                    isWiFiAPPrivateChannelJoined = false;               
                }
                wifiAPPrivateDataHistory += "\n$$ [Mobile AP] onStopped(" + msg + ")";              
                scrollPosition3.y = Mathf.Infinity;
                lock (mNodeList)
                {
                    for (int i =0; i<mNodeList.Count; i++)
                    {
                        Node temp = mNodeList [i];
                        if (temp.interfaceName.Contains("Mobile AP"))
                        {
                            mNodeList.Remove(temp);
                            i--;
                        }
                        
                    }
                }
                Destroy(myWifiAPNodeCube.gameObject);
            }
        }
    }

    // Node joined event 
    public  void onNodeJoined(String nodeName, String fromChannel, int interfaceType)
    {
        CLog.Log("[Spin][onNodeJoined]: nodeName : " + nodeName + " channelName : " + fromChannel + " interfaceType" + interfaceType);
        //add node to nodelist
        
        Node newNode = new Node(nodeName, getInterfaceName(interfaceType), false);
        mNodeList.Add(newNode);

        String cubeName = nodeName + "[" + getInterfaceName(interfaceType) + "]";

        Transform joinedNodeCube = (Transform)Instantiate(nodeCube, new Vector3(UnityEngine.Random.Range(0.0f, 8.0f), UnityEngine.Random.Range(-8.0f, 2.0f), 0), new Quaternion());
        TextMesh joinedNodeName = (TextMesh)joinedNodeCube.GetChild(1).GetComponent<TextMesh>();
        joinedNodeName.text = cubeName;

        joinedNodeCube.tag = "Joined Node";
        joinedNodeCube.name = cubeName;

        Byte[][] bytes = new Byte[1][];
        bytes [0] = Encoding.UTF8.GetBytes("Hello chord!!"); 
                
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            wifiPrivateDataHistory += "\n$$ onNodeJoined() : " + nodeName;
            if (choice == 0)
            {
                wifiPrivateDataHistory += "\n$$ SendData(" + nodeName + ",Hello chord!!)";
                mChordWrapper.sendData(UNITY_SEND_DATA_TEST_CHANNEL, nodeName, CHORD_UNITY_MESSAGE_TYPE, bytes);
            }
            else if (choice == 2)
            {
                String encrypted = (isWiFiSecureEnabled) ? "Encrypted data" : "Non-Encrypted data";
                wifiPrivateDataHistory += "\n$$ SendData(" + nodeName + ", " + encrypted + ")";
                bytes [0] = Encoding.UTF8.GetBytes(encrypted); 
                mChordWrapper.sendData(privateChannelWiFi, nodeName, CHORD_UNITY_MESSAGE_TYPE, bytes);
            }
            scrollPosition1.y = Mathf.Infinity;
            wifiCount++;
            sendUdpPosition(nodeName, myWifiNodeCube.GetChild(0).position, ChordUnityConstants.INTERFACE_TYPE_WIFI);
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            wifiDirectPrivateDataHistory += "\n$$   onNodeJoined() : " + nodeName;
            if (choice == 0)
            {
                String encrypted = (isWiFiDirectSecureEnabled) ? "Encrypted data" : "Non-Encrypted data";
                wifiDirectPrivateDataHistory += "\n$$ SendData(" + nodeName + ", " + encrypted + ")";
                bytes [0] = Encoding.UTF8.GetBytes(encrypted); 
                mWiFiDirectChordWrapper.sendData(privateChannelWiFiDirect, nodeName, CHORD_UNITY_MESSAGE_TYPE, bytes);
            }
            scrollPosition2.y = Mathf.Infinity;
            wifiDirectCount++;
            sendUdpPosition(nodeName, myWifiDirectNodeCube.GetChild(0).position, ChordUnityConstants.INTERFACE_TYPE_WIFIP2P);
        }
        else
        {
            wifiAPPrivateDataHistory += "\n$$   onNodeJoined() : " + nodeName;
            if (choice == 0)
            {
                String encrypted = (isWiFiAPSecureEnabled) ? "Encrypted data" : "Non-Encrypted data";
                wifiAPPrivateDataHistory += "\n$$ SendData(" + nodeName + ", " + encrypted + ")";
                bytes [0] = Encoding.UTF8.GetBytes(encrypted); 
                mMobileAPChordWrapper.sendData(privateChannelWiFiAP, nodeName, CHORD_UNITY_MESSAGE_TYPE, bytes);
            }
            scrollPosition3.y = Mathf.Infinity;
            wifiAPCount++;
            sendUdpPosition(nodeName, myWifiAPNodeCube.GetChild(0).position, ChordUnityConstants.INTERFACE_TYPE_WIFIAP);
        }   
    }
    // Node left event 
    public void onNodeLeft(string nodeName, string fromChannel, int interfaceType)
    {
        CLog.Log("[Spin][onNodeLeft]: nodeName : " + nodeName + " channel : " + fromChannel);
        
        lock (mNodeList)
        {
            for (int i =0; i<mNodeList.Count; i++)
            {               
                if (mNodeList [i].nodeName.Equals(nodeName) && getInterfaceName(interfaceType).Equals(mNodeList [i].interfaceName))
                {
                    mNodeList.RemoveAt(i);
                    break;
                }
            }
        }
    
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            wifiPrivateDataHistory += "\n$$ onNodeLeft() : " + nodeName;
            scrollPosition1.y = Mathf.Infinity;
            wifiCount--;
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            wifiDirectPrivateDataHistory += "\n$$   onNodeLeft() : " + nodeName;
            scrollPosition2.y = Mathf.Infinity;
            wifiDirectCount--;
        }
        else
        {
            wifiAPPrivateDataHistory += "\n$$S  onNodeLeft() : " + nodeName;
            scrollPosition3.y = Mathf.Infinity;
            wifiAPCount--;
        }

        GameObject[] joinedNodeCubeList = GameObject.FindGameObjectsWithTag("Joined Node");
        for (int i = 0; i < joinedNodeCubeList.Length; i++)
        {
            if (joinedNodeCubeList [i].transform.name.Contains(nodeName))
            {
                Destroy(joinedNodeCubeList [i]);
                break;
            }
        }
    }
    // Data received event 
    public void onDataReceived(string fromNode, string fromChannel, string payloadType, byte[][] payload, int interfaceType)
    {
        
        int len = payload.Length;
        CLog.Log("[Spin][onDataReceived]: fromNode : " + fromNode + " channel : " + fromChannel + " payload type : " + payloadType + " payload length : " + len);
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Data received from: " + fromNode;       
                wifiPrivateDataHistory += "\n$$ Data :";            
                for (int i = 0; i < len; i++)
                {
                    wifiPrivateDataHistory += " " + System.Text.Encoding.Default.GetString(payload [i]);                
                }
            }
            scrollPosition1.y = Mathf.Infinity;
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Data received from: " + fromNode;     
                wifiDirectPrivateDataHistory += "\n$$ Data :";          
                for (int i = 0; i < len; i++)
                {
                    wifiDirectPrivateDataHistory += " " + System.Text.Encoding.Default.GetString(payload [i]);              
                }
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Data received from: " + fromNode;     
                wifiAPPrivateDataHistory += "\n$$ Data :";          
                for (int i = 0; i < len; i++)
                {
                    wifiAPPrivateDataHistory += " " + System.Text.Encoding.Default.GetString(payload [i]);              
                }
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }
    // Share file notification received 
    public void onFileWillReceive(String fromNode, String fromChannel, String fileName,
            String hash, String fileType, String exchangeId, long fileSize, int interfaceType)
    {

        CLog.Log("[Spin] [onFilesWillReceive]: fileName : " + fileName + " fromNode : " + fromNode + " channel : " + fromChannel + " fileSize : " + fileSize);
            
    
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            mChordWrapper.acceptFile(fromChannel, exchangeId, UNITY_FILE_CHUNK_TIMEOUT, UNITY_FILE_CHUNK_RETRY, UNITY_FILE_CHUNK_SIZE);
            
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Accepted file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition1.y = Mathf.Infinity;
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            mWiFiDirectChordWrapper.acceptFile(fromChannel, exchangeId, UNITY_FILE_CHUNK_TIMEOUT, UNITY_FILE_CHUNK_RETRY, UNITY_FILE_CHUNK_SIZE);
            
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Accepted file : " + fileName + ", from node : " + fromNode;   
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            mMobileAPChordWrapper.acceptFile(fromChannel, exchangeId, UNITY_FILE_CHUNK_TIMEOUT, UNITY_FILE_CHUNK_RETRY, UNITY_FILE_CHUNK_SIZE);
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Accepted file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // Filechunk received event 
    public void onFileChunkReceived(String fromNode, String fromChannel, String fileName,

                String hash, String fileType, String exchangeId, long fileSize, long offset, int interfaceType)
    {
        CLog.Log("[Spin][onFileChunkReceived]: fileName: " + fileName + " Offset : " + offset + " fileSize : " + fileSize + " fromNode : " + fromNode
            + " channel : " + fromChannel);
    }

    // File received event 

    public void onFileReceived(String fromNode, String fromChannel, String fileName,

                String hash, String fileType, String exchangeId, long fileSize, String tmpFilePath, int interfaceType)
    {
        CLog.Log("[Spin][onFileReceived]: fileName : " + fileName + " fromNode : " + fromNode + " channel : " + fromChannel);
        

        String savedName = fileName;

        int i = savedName.LastIndexOf(".");

        if (i < 0)
        {
            CLog.Log("[Spin][onFileReceived]: File received without extension : " + fileName);

            return;
        }

        String name = savedName.Substring(0, i);

        String ext = savedName.Substring(i);

        var targetFile = new FileInfo(Path.Combine(chordFilePath, savedName));

        int index = 0;

        while (targetFile.Exists)
        {
            savedName = name + "_" + index + ext;

            targetFile = new FileInfo(Path.Combine(chordFilePath, savedName));

            index++;

            CLog.Log("[Spin][onFileReceived]: saved name : " + savedName);
        }

        File.Move(tmpFilePath, Path.Combine(chordFilePath, savedName));

        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Received  file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition1.y = Mathf.Infinity;
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Received  file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Received  file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // File chunk sent event

    public void onFileChunkSent(String toNode, String toChannel, String fileName, String hash,

                String fileType, String exchangeId, long fileSize, long offset, long chunkSize, int interfaceType)
    {
        CLog.Log("[Spin][onFileChunkSent]: fileName : " + fileName + "Offset : " + offset + " fileSize : " + fileSize +
            " toNode : " + toNode + " channel : " + toChannel);
    }

    // File sent event

    public void onFileSent(String toNode, String toChannel, String fileName, String hash,

                String fileType, String exchangeId, int interfaceType)
    {
        
        CLog.Log("[Spin][onFileSent]: fileName : " + fileName + " toNode : " + toNode + " channel : " + toChannel);
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Sent file : " + fileName + ", to node : " + toNode;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Sent file : " + fileName + ", to node : " + toNode;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Sent file : " + fileName + ", to node : " + toNode;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // File failed event

    public void onFileFailed(String node, String channel, String fileName, String hash,

                String exchangeId, int reason, int interfaceType)
    {

        CLog.Log("[Spin][onFileFailed]: fileName : " + fileName + " node : " + node + " channel : " + channel + " reason : " + reason);
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ File transfer failed : " + fileName + ", Node : " + node;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ File transfer failed : " + fileName + ", Node : " + node;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ File transfer failed : " + fileName + ", Node : " + node;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // Notification for multiple share files received

    public void onMultiFilesWillReceive(String fromNode, String fromChannel, String fileName,

            String taskId, int count, String fileType, long fileSize, int interfaceType)
    {
        
        CLog.Log("[Spin] [onMultiFilesWillReceive]: fileName : " + fileName + " fromNode : " + fromNode + " channel : " + fromChannel + " fileSize : " + fileSize);
        
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            mChordWrapper.acceptMultiFiles(fromChannel, taskId, UNITY_FILE_CHUNK_TIMEOUT, UNITY_FILE_CHUNK_RETRY, UNITY_FILE_CHUNK_SIZE);
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Accepted multi files, file name : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            mWiFiDirectChordWrapper.acceptMultiFiles(fromChannel, taskId, UNITY_FILE_CHUNK_TIMEOUT, UNITY_FILE_CHUNK_RETRY, UNITY_FILE_CHUNK_SIZE);
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Accepted multi files, file name : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            mMobileAPChordWrapper.acceptMultiFiles(fromChannel, taskId, UNITY_FILE_CHUNK_TIMEOUT, UNITY_FILE_CHUNK_RETRY, UNITY_FILE_CHUNK_SIZE);
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Accepted multi files, file name : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // File chunk received event for mutishare file 

    public void onMultiFilesChunkReceived(String fromNode, String fromChannel, String fileName,

            String taskId, int index, String fileType, long fileSize, long offset, int interfaceType)
    {
        CLog.Log("[Spin][onMultiFilesChunkReceived]: fileName : " + fileName + " Offset : " + offset + " fileSize : " + fileSize 
            + " fromNode : " + fromNode + " channel : " + fromChannel);
    }

    // File received event for mutishare file 

    public void onMultiFilesReceived(String fromNode, String fromChannel, String fileName,

            String taskId, int index, String fileType, long fileSize, String tmpFilePath, int interfaceType)
    {
        
        CLog.Log("[Spin][onMultiFilesReceived]: fileName : " + fileName + " fromNode : " + fromNode + " channel : " + fromChannel
            + " index : " + index + " fileSize : " + fileSize);

        String savedName = fileName;

        int i = savedName.LastIndexOf(".");

        if (i < 0)
        {
            CLog.Log("[Spin][onMultiFilesReceived]: File received without extension : " + fileName);
            return;
        }

        String name = savedName.Substring(0, i);

        String ext = savedName.Substring(i);

        var targetFile = new FileInfo(Path.Combine(chordFilePath, savedName));

        int i2 = 0;


        while (targetFile.Exists)
        {


            savedName = name + "_" + i2 + ext;

            targetFile = new FileInfo(Path.Combine(chordFilePath, savedName));

            i2++;

            CLog.Log("[Spin][onMultiFilesReceived]: saved name : " + savedName);

        }

        File.Move(tmpFilePath, Path.Combine(chordFilePath, savedName));
    
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Multifile - Received file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Multifile - Received file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Multifile - Received file : " + fileName + ", from node : " + fromNode;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // File chunk sent event for mutishare file     

    public void onMultiFilesChunkSent(String toNode, String toChannel, String fileName, String taskId,

                int index, String fileType, long fileSize, long offset, long chunkSize, int interfaceType)
    {
        CLog.Log("[Spin][onMultiFilesChunkSent]: fileName : " + fileName + " Offset : " + offset + " fileSize : " + fileSize 
            + "toNode : " + toNode + " channel : " + toChannel);

    }

    // File  sent event for mutishare file 

    public void onMultiFilesSent(String toNode, String toChannel, String fileName, String taskId,

            int index, String fileType, int interfaceType)
    {

        CLog.Log("[Spin][onMultiFilesSent]: fileName" + fileName + " toNode : " + toNode + "channel : " + toChannel);
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Multifile - Sent file : " + fileName + ", to node : " + toNode;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {     
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Multifile - Sent file : " + fileName + ", to node : " + toNode;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Multifile - Sent file : " + fileName + ", to node : " + toNode;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // File failed event for mutishare file 

    public void onMultiFilesFailed(String node, String channel, String fileName, String taskId,

            int index, int reason, int interfaceType)
    {
        
        CLog.Log("[Spin][onMultiFilesFailed]: fileName : " + fileName + " node : " + node + " channel : " + channel + " reason : " + reason);
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Multifile transfer failed, file name :  " + fileName + ", node : " + node;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Multifile transfer failed, file name :  " + fileName + ", node : " + node;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Multifile transfer failed, file name :  " + fileName + ", node : " + node;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }

    // File send/receive finished event for mutishare file 

    public void onMultiFilesFinished(String node, String channel, String taskId, int reason, int interfaceType)
    {
        CLog.Log("[Spin] [onMultiFilesFinished]: " + taskId + "node : " + node + " channel : " + channel);
        
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ Multifile transfer finished for node : " + node;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ Multifile transfer finished for node : " + node;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ Multifile transfer finished for node : " + node;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }  
    }

    public void onUdpDataReceived(string node, string channel, string payloadType, byte[][] payload, string sessionName, int interfaceType)
    {
        CLog.Log("[Spin][onUdpDataReceived]:  fromNode : " + node + " channelName : " + channel + " sessionName : " + sessionName + " interfaceType : " + getInterfaceName(interfaceType));
        
        int len = payload.Length;
        String firstData = System.Text.Encoding.Default.GetString(payload [0]);
        if (firstData.Contains("CubePosition"))
        {
            CLog.Log("[Spin][onUdpDataReceived]:  Data : " + firstData);
            int posX1 = firstData.IndexOf("[") + 1;
            int posX2 = firstData.IndexOf("]");
            int posY1 = firstData.LastIndexOf("[") + 1;
            int posY2 = firstData.LastIndexOf("]");

            String posX = firstData.Substring(posX1, posX2 - posX1);
            String posY = firstData.Substring(posY1, posY2 - posY1);

            GameObject[] joinedNodeCubeList = GameObject.FindGameObjectsWithTag("Joined Node");
            for (int i = 0; i < joinedNodeCubeList.Length; i++)
            {
                if (joinedNodeCubeList [i].transform.name.Contains(node) && joinedNodeCubeList [i].transform.name.Contains("[" + getInterfaceName(interfaceType) + "]"))
                {
                    joinedNodeCubeList [i].transform.GetChild(0).position = new Vector3(float.Parse(posX), float.Parse(posY), 0);
                    joinedNodeCubeList [i].transform.GetChild(1).position = new Vector3(float.Parse(posX), float.Parse(posY) - 0.5f, 0);
                }
            }
        }
    }
          
    public void onUdpDataDelivered(string toNode, string toChannel, string reqId, int interfaceType)
    {       
        CLog.Log("[Spin][onUdpDataDelivered]:  " + " toNode : " + toNode + " toChannel : " + toChannel + " reqId : " + reqId);
        if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFI)
        {
            lock (wifiPrivateDataHistory)
            {
                wifiPrivateDataHistory += "\n$$ UdpDataDelivered,  toNode : " + toNode + " toChannel : " + toChannel + " requestId : " + reqId;
                
            }
            scrollPosition1.y = Mathf.Infinity;
            
        }
        else if (interfaceType == ChordUnityConstants.INTERFACE_TYPE_WIFIP2P)
        {
            lock (wifiDirectPrivateDataHistory)
            {
                wifiDirectPrivateDataHistory += "\n$$ UdpDataDelivered,  toNode : " + toNode + " toChannel : " + toChannel + " requestId : " + reqId;
                
            }
            scrollPosition2.y = Mathf.Infinity;
        }
        else
        {
            lock (wifiAPPrivateDataHistory)
            {
                wifiAPPrivateDataHistory += "\n$$ UdpDataDelivered,  toNode : " + toNode + " toChannel : " + toChannel + " requestId : " + reqId;
                
            }
            scrollPosition3.y = Mathf.Infinity;
        }   
    }
    // Get the error string from error code

    private String getErrorString(int errorCode)
    {

        switch (errorCode)
        {

            case ChordUnityConstants.ERROR_ILLEGAL_STATE:

                return "ERROR_ILLEGAL_STATE";

            case ChordUnityConstants.ERROR_ILLEGAL_ARGUMENT:

                return "ERROR_ILLEGAL_ARGUMENT";

            case ChordUnityConstants.ERROR_RUNTIME_EXCEPTION:

                return "RUNTIME_EXCEPTION";

            case ChordUnityConstants.ERROR_FILE_NOT_FOUND:

                return "FILE_NOT_FOUND_EXCEPTION";

            case ChordUnityConstants.ERROR_INVALID_INTERFACE:

                return "INVALID_INTERFACE_EXCEPTION";

            case ChordUnityConstants.ERROR_SSDK_UNSUPPORTED:

                return "SSDK_UNSUPPORTED_EXCEPTION";

            case ChordUnityConstants.ERROR_CHORD_UNINITIALIZED:

                return "ERROR_CHORD_UNINITIALIZED";

            default:

                return "UNKNOWN_ERROR";
        }
    }
}

class Listener : ChordUnityChannelListener,ChordUnityStatusListener
{
    private Spin mListener = null;
    private int mInterfaceType = -1;

    public Listener(Spin obj, int ifc)
    {
        mListener = obj;
        mInterfaceType = ifc;
    }

    public void onBrowseResult(String[] fileList)
    {           
        mListener .onBrowseResult(fileList, mInterfaceType);
    }
    
    public void onStarted(string nodeName, int reason)
    {
        CLog.Log("[Spin][WiFiListener][onStarted]: reason " + reason);
        mListener.onStarted(nodeName, reason, mInterfaceType);
    }

    public void onStopped(int reason)
    {
        CLog.Log("[Spin][onStopped]: reason " + reason);
        mListener.onStopped(reason, mInterfaceType);

    }

    public void onNodeJoined(String nodeName, String fromChannel)
    {
        CLog.Log("[Spin][onNodeJoined]: nodeName : " + nodeName + " channelName : " + fromChannel);
        mListener.onNodeJoined(nodeName, fromChannel, mInterfaceType);
           
    
    }
    
    public void onNodeLeft(string nodeName, string fromChannel)
    {
        CLog.Log("[Spin][onNodeLeft]: nodeName : " + nodeName + " channel : " + fromChannel);
        mListener.onNodeLeft(nodeName, fromChannel, mInterfaceType);
       
    }
    
    public void onDataReceived(string fromNode, string fromChannel, string payloadType, byte[][] payload)
    {
        mListener.onDataReceived(fromNode, fromChannel, payloadType, payload, mInterfaceType);
    }
   
    public void onFileWillReceive(String fromNode, String fromChannel, String fileName,
            String hash, String fileType, String exchangeId, long fileSize)
    {
        mListener.onFileWillReceive(fromNode, fromChannel, fileName,
            hash, fileType, exchangeId, fileSize, mInterfaceType);
    }

    public void onFileChunkReceived(String fromNode, String fromChannel, String fileName,

                String hash, String fileType, String exchangeId, long fileSize, long offset)
    {
        
        CLog.Log("[Spin][onFileChunkReceived]: fileName: " + fileName + " Offset : " + offset + " fileSize : " + fileSize + " fromNode : " + fromNode
            + " channel : " + fromChannel);
        mListener.onFileChunkReceived(fromNode, fromChannel, fileName,

               hash, fileType, exchangeId, fileSize, offset, mInterfaceType);
    }

    public void onFileReceived(String fromNode, String fromChannel, String fileName,

                String hash, String fileType, String exchangeId, long fileSize, String tmpFilePath)
    {
        
        CLog.Log("[Spin][onFileReceived]: fileName : " + fileName + " fromNode : " + fromNode + " channel : " + fromChannel);
        mListener.onFileReceived(fromNode, fromChannel, fileName,

                hash, fileType, exchangeId, fileSize, tmpFilePath, mInterfaceType);
       
    }

    public void onFileChunkSent(String toNode, String toChannel, String fileName, String hash,

                String fileType, String exchangeId, long fileSize, long offset, long chunkSize)
    {       

        CLog.Log("[Spin][onFileChunkSent]: fileName : " + fileName + "Offset : " + offset + " fileSize : " + fileSize +
            " toNode : " + toNode + " channel : " + toChannel);
        mListener.onFileChunkSent(toNode, toChannel, fileName, hash,

               fileType, exchangeId, fileSize, offset, chunkSize, mInterfaceType);
    }

    public void onFileSent(String toNode, String toChannel, String fileName, String hash,

                String fileType, String exchangeId)
    {
        
        CLog.Log("[Spin][onFileSent]: fileName : " + fileName + " toNode : " + toNode + " channel : " + toChannel);
        mListener.onFileSent(toNode, toChannel, fileName, hash,

                fileType, exchangeId, mInterfaceType);
        
    }

    public void onFileFailed(String node, String channel, String fileName, String hash,

                String exchangeId, int reason)
    {

        CLog.Log("[Spin][onFileFailed]: fileName : " + fileName + " node : " + node + " channel : " + channel + " reason : " + reason);

        mListener.onFileFailed(node, channel, fileName, hash,

                exchangeId, reason, mInterfaceType);
    }

    public void onMultiFilesWillReceive(String fromNode, String fromChannel, String fileName,

            String taskId, int count, String fileType, long fileSize)
    {
        
        CLog.Log("[Spin] [onMultiFilesWillReceive]: fileName : " + fileName + " fromNode : " + fromNode + " channel : " + fromChannel + " fileSize : " + fileSize);

        mListener.onMultiFilesWillReceive(fromNode, fromChannel, fileName,

            taskId, count, fileType, fileSize, mInterfaceType);

    }
   
    public void onMultiFilesChunkReceived(String fromNode, String fromChannel, String fileName,

            String taskId, int index, String fileType, long fileSize, long offset)
    {
        

        CLog.Log("[Spin][onMultiFilesChunkReceived]: fileName : " + fileName + " Offset : " + offset + " fileSize : " + fileSize 
            + " fromNode : " + fromNode + " channel : " + fromChannel);
        mListener.onMultiFilesChunkReceived(fromNode, fromChannel, fileName,

            taskId, index, fileType, fileSize, offset, mInterfaceType);
    }
   
    public void onMultiFilesReceived(String fromNode, String fromChannel, String fileName,

            String taskId, int index, String fileType, long fileSize, String tmpFilePath)
    {

        
        CLog.Log("[Spin][onMultiFilesReceived]: fileName : " + fileName + " fromNode : " + fromNode + " channel : " + fromChannel
            + " index : " + index + " fileSize : " + fileSize);
        mListener.onMultiFilesReceived(fromNode, fromChannel, fileName,

            taskId, index, fileType, fileSize, tmpFilePath, mInterfaceType);
        

    }

    public void onMultiFilesChunkSent(String toNode, String toChannel, String fileName, String taskId,

                int index, String fileType, long fileSize, long offset, long chunkSize)
    {
        
        CLog.Log("[Spin][onMultiFilesChunkSent]: fileName : " + fileName + " Offset : " + offset + " fileSize : " + fileSize 
            + "toNode : " + toNode + " channel : " + toChannel);
        mListener.onMultiFilesChunkSent(toNode, toChannel, fileName, taskId,

               index, fileType, fileSize, offset, chunkSize, mInterfaceType);
    }
   
    public void onMultiFilesSent(String toNode, String toChannel, String fileName, String taskId,

            int index, String fileType)
    {

        
        CLog.Log("[Spin][onMultiFilesSent]: fileName" + fileName + " toNode : " + toNode + "channel : " + toChannel);
        mListener.onMultiFilesSent(toNode, toChannel, fileName, taskId,

            index, fileType, mInterfaceType);
    }

    public void onMultiFilesFailed(String node, String channel, String fileName, String taskId,

            int index, int reason)
    {
        
        CLog.Log("[Spin][onMultiFilesFailed]: fileName : " + fileName + " node : " + node + " channel : " + channel + " reason : " + reason);
        mListener.onMultiFilesFailed(node, channel, fileName, taskId, index, reason, mInterfaceType);
        
    }

    public void onMultiFilesFinished(String node, String channel, String taskId, int reason)
    {
        
        CLog.Log("[Spin] [onMultiFilesFinished]: " + taskId + "node : " + node + " channel : " + channel);  
        mListener.onMultiFilesFinished(node, channel, taskId, reason, mInterfaceType);
    }

    public void onUdpDataReceived(string fromNode, string fromChannel, string payloadType, byte[][] payload, string sessionName)
    {
        mListener.onUdpDataReceived(fromNode, fromChannel, payloadType, payload, sessionName, mInterfaceType);      
    }
          
    public void onUdpDataDelivered(string fromNode, string channelName, string reqId)
    {       
        mListener.onUdpDataDelivered(fromNode, channelName, reqId, mInterfaceType);
    }

}

