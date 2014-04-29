 /*============================== FILE HEADER =================================
*
*           Copyright (c) 2013, Samsung Research Institute India , Bangalore
*                      Samsung Confidential Proprietary
*                            All Rights Reserved
*
==============================================================================
*                               Module Name : C# Chord Unity Interface
==============================================================================
* File                    : ChordUnity.cs
*
* Author(s)           : Ganesh VR , Dhanya R
*
* Description         : C# Interface Implementation  between  the C# Application & Java Chord Unity Service Interface.
*
==============================================================================
*                            Modification History
*-----------------------------------------------------------------------------
*    Date       |       Name                    |        Modification
*-----------------------------------------------------------------------------
*                  |                               		|
*  24-07-13  | Ganesh VR , Dhanya R        | Newly Added
*                  |                               		|
============================================================================*/

using UnityEngine;

using System;

using System.Collections;

using System.Collections.Generic;

using System.IO;

using System.Text;

/*public class ChordUnityManager
{
	private ChordUnity mInstance;
	private GameObject mChordUnityManager = null;
	private static int instanceId = 0;
	public ChordUnity getInstance()
	{
		instanceId++;
		String chordUnityManagerName = "_ChordUnityManager" + instanceId;
		if ((mChordUnityManager = GameObject.Find(chordUnityManagerName)) == null)
	        {
	
	            CLog.Log("[ChordUnity][getInstance]: creating manager game object.");
	
				mChordUnityManager = new GameObject(chordUnityManagerName);
	
	            int value = AndroidJNI.AttachCurrentThread();
	
	            if (value < 0)
	
	                CLog.Log("[ChordUnity][getInstance]: error in creating manager game object.");
	
	            AndroidJNIHelper.debug = true;
	
	        }
	
	        if ((mInstance = mChordUnityManager.GetComponent<ChordUnity>()) == null)
	        {
	
	            CLog.Log("[ChordUnity][getInstance]: adding component to manager game object.");
	
	            mInstance = mChordUnityManager.AddComponent<ChordUnity>();
	
	            mInstance.init(chordUnityManagerName);
	
	        }
			 CLog.Log("[ChordUnity][getInstance]: getInstance end");

        	return mInstance;	
		
	}
}*/
/*
 * c# wrapper class for applicaiton to interface with ChordUnity Service Java Interface.
 *  
 */

public class ChordUnity : MonoBehaviour
{

    private  static GameObject mChordUnityManager = null;
	private  static GameObject mChordUnityManager1 = null;
	private  static GameObject mChordUnityManager2 = null;

    private  AndroidJavaObject mChordUnityServiceJava = null;

    private static string ChordUnityServiceJavaClassName = "com.samsung.android.sdk.chordUnity.ChordUnityService";

    private static ChordUnity mInstance;
	private static ChordUnity mInstance1;
	private static ChordUnity mInstance2;	

	private ChordUnityChannelListener mCaller = null;	
	private ChordUnityNetworkListener mNtwkListener = null;
	private ChordUnityStatusListener mStatusListener = null;
	
	private static int instanceId = -1;
	private int mHandle=-1;	
	private static object thisLock = new object();
    void Start()
    {

        CLog.Log("[ChordUnity][Start]: ChordUnity Start");

    }

    /**
    * getInstance returns the ChordUnity object.
    * and create the _ChordUnityManager object for return path & get thpayload[i++] = obj.payloadType;e unityService java object for direct calls.
    */

	public static ChordUnity getObject()	
    {
        //CLog.Log("[ChordUnity][getInstance]: ChordUnity getInstance start");		
		int value = AndroidJNI.AttachCurrentThread();
        if (value < 0)
            CLog.Log("[ChordUnity][getInstance]: error in attaching the thread.");
        AndroidJNIHelper.debug = true;
		
		lock(thisLock)
		{
			instanceId++;		
		if(mChordUnityManager == null)
		{				
			if ((mChordUnityManager = GameObject.Find("_ChordUnityManager"+ instanceId)) == null)
	        {	
				mChordUnityManager = new GameObject("_ChordUnityManager"+ instanceId);
			}
	
	        if ((mInstance = mChordUnityManager.GetComponent<ChordUnity>()) == null)
	        {
	            mInstance = mChordUnityManager.AddComponent<ChordUnity>();	
	            mInstance.init();	
	        }			
        	return mInstance;	
		}
		else if(mChordUnityManager1 == null)
		{
			if ((mChordUnityManager1 = GameObject.Find("_ChordUnityManager" + instanceId)) == null)
	        {	
				mChordUnityManager1 = new GameObject("_ChordUnityManager" + instanceId);
	        }
	
	        if ((mInstance1 = mChordUnityManager1.GetComponent<ChordUnity>()) == null)
	        {		
	            mInstance1 = mChordUnityManager1.AddComponent<ChordUnity>();	
	            mInstance1.init();	
	        }			
        	return mInstance1;	
		}
		else if(mChordUnityManager2 == null)
		{
			if ((mChordUnityManager2 = GameObject.Find("_ChordUnityManager" + instanceId)) == null)
	        {	
				mChordUnityManager2 = new GameObject("_ChordUnityManager" + instanceId);

	        }
	
	        if ((mInstance2 = mChordUnityManager2.GetComponent<ChordUnity>()) == null)
	        {
	            mInstance2 = mChordUnityManager2.AddComponent<ChordUnity>();	
	            mInstance2.init();	
	        }			

        	return mInstance2;	
		}		
	  }
		return null;
    }

    // constructor

   	private ChordUnity()
    {

        CLog.Log("[ChordUnity][ChordUnity]:  private(!) constructor noarg");

    }

    /**
      * initialize the AndroidJavaClass and get the unityService java object for direct calls.
      */

    public void init()
    {lock(thisLock)
		mHandle = instanceId;
        CLog.Log("[ChordUnity][init]: width: ");
		
        AndroidJavaClass chordUnityServiceJavaClass = new AndroidJavaClass(ChordUnityServiceJavaClassName);
		 
        mChordUnityServiceJava = chordUnityServiceJavaClass.CallStatic<AndroidJavaObject>("CreateObject",mHandle);
		if(mChordUnityServiceJava==null)
			CLog.Log("[ChordUnity][init]:mChordUnityServiceJava is null.");	
        
    }

    /**
         * Set a directory path to save receiving file temporarily.
         * If it does not be specified, the file transfer will be failed.
         * A Directory should be set before starting Chord, otherwise, it will be ignored.
         * 
         */

    public void setTempDirectory(String tmpDirPath)
    {
        CLog.Log("[ChordUnity][setTempDirectory]: set path:  " + tmpDirPath);

		object[] mulVars = new object[2];
		
		mulVars[0] = tmpDirPath;
		mulVars[1] = mHandle;
	
		//mChordUnityServiceJava.Call("setTempDirectory", mulVars);
		mChordUnityServiceJava.Call("setTempDirectory", mulVars);
    }

    /**
        * Sets whether to use secured channel
        * It is false if you do not set
        * 
        * @param enabled whether to use secured channel
        */

    public void setSecureModeEnabled(bool enabled)
    {
        CLog.Log("[ChordUnity][setSecureModeEnabled]: setSecureMode" + enabled);

		object[] mulVars = new object[2];
		
		mulVars[0] = enabled;
		mulVars[1] = mHandle;
		
		mChordUnityServiceJava.Call("setSecureModeEnabled", mulVars);
    }

    /**
      * Set callbacks to listen for changes in network connection.
      *
      * @param listener This is called when connection info is changed. It allow null as no listener.
      */

    public void setNetworkListener(ChordUnityNetworkListener listener)
    {
			CLog.Log("ChordUnity][setNetworkListener]::setNetworkListener");
        mNtwkListener = listener;
		mChordUnityServiceJava.Call("setNetworkListener", mHandle);
		CLog.Log("ChordUnity][setNetworkListener]::setNetworkListener Exit");
    }

    /**
       * Start Chord. When chord is started successfully, it will be called onStarted of StatusListener.
       * @param interfaceType An interface type to communicate. It is a mandatory input.
       * @param cListener Listen to chord cahnnel events. It is a mandatory input.
       * @param sListener Listen to chord manager event. It is a mandatory input.
       * 
       * returns the success or failure error code any of below
       * 	- ChordUnityConstants.START_SUCCESS
       * 	- ChordUnityConstants.ERROR_ILLEGAL_STATE
       * 	- ChordUnityConstants.ERROR_ILLEGAL_ARGUMENT
       * 	- ChordUnityConstants.ERROR_INVALID_INTERFACE
       * 	- ChordUnityConstants.ERROR_RUNTIME_EXCEPTION
       */

    public int start(int interfaceType, ChordUnityChannelListener cListener, ChordUnityStatusListener sListener)
    {
        int result = ChordUnityConstants.START_SUCCESS;		
	
        CLog.Log("[ChordUnity][start]: ChordUnity start  ."+" interface: "+ interfaceType);

        if (mChordUnityServiceJava != null)
        {
			object[] mulVars = new object[2];
			
			mulVars[0] = interfaceType;

			mulVars[1] = mHandle;			
			
            result = mChordUnityServiceJava.Call<int>("start", mulVars);

            CLog.Log("[ChordUnity][start]: mChordUnityServiceJava.Call<int>(\"start\", interfaceType)");
			
        }

		this.mCaller = cListener;

      	this.mStatusListener = sListener;

        CLog.Log("[ChordUnity][start]: end.");

        return result;
    }

    /**
      * Send data message to the node.
      * 
      * @param toChannel The channel name that the message is sent to. It is mandatory.     
      * @param toNode The joined node name that the message is sent to. It is mandatory.
      * @param payloadType User defined message type. It is mandatory.
      * @param payload The package of data to send
      */

    public void sendData(String toChannel, String nodeName, String payloadType, byte[][] payload)
    {

        bool result;

        int i = 0;

        int len = payload.Length;

        CLog.Log("[ChordUnity][sendData]:  len." + len);

        String[] sendDataParam = new String[len + 4];		

        sendDataParam[0] = toChannel;

        sendDataParam[1] = payloadType;

        sendDataParam[2] = nodeName;
		
		sendDataParam[3] = mHandle.ToString ();

        for (i = 0; i < len; i++)
        {

            sendDataParam[i + 4] = System.Text.Encoding.Default.GetString(payload[i]);

        }

        object[] mulVars = new object[1];

        mulVars[0] = sendDataParam;

        result = mChordUnityServiceJava.Call<bool>("sendData", mulVars);

        if (result == false)

            CLog.Log("[ChordUnity][sendData]:  fail to send data.");

    }

    /**
      * Send data message to all the node on channel.
      * 
      * @param toChannel The channel name that the message is sent to. It is mandatory.     
      * @param payloadType User defined message type. It is mandatory.
      * @param payload The package of data to send
      */

    public void sendDataToAll(String toChannel, String payloadType, byte[][] payload)
    {

        bool result;

        int i = 0;

        int len = payload.Length;

        CLog.Log("[ChordUnity][sendDataToAll]: len." + len);

        String[] sendDataParam = new String[len + 3];		

        sendDataParam[0] = toChannel;

        sendDataParam[1] = payloadType;
		
		sendDataParam[2] = mHandle.ToString ();

        for (i = 0; i < len; i++)
        {

            sendDataParam[i + 3] = System.Text.Encoding.Default.GetString(payload[i]);

        }

        object[] mulVars = new object[1];

        mulVars[0] = sendDataParam;

        result = mChordUnityServiceJava.Call<bool>("sendDataToAll", mulVars);

        if (result == false)

            CLog.Log("[ChordUnity][sendDataToAll]:  fail to send data.");

        return;

    }
	
	public String  sendUdpData(String toChannel, String toNode, int reliableTime, bool respFlag, String payloadType, byte[][] payload,  String sessionName)
	{
		String reqId = null;

        int i = 0;

        int len = payload.Length;

        CLog.Log("[ChordUnity][udpSendData]:  len." + len);

        String[] sendDataParam = new String[len + 7];		

        sendDataParam[0] = toChannel;
		
		sendDataParam[1] = toNode;
		
		sendDataParam[2] = reliableTime.ToString();
		
		sendDataParam[3] = respFlag.ToString ().ToLower ();

        sendDataParam[4] = payloadType;

        sendDataParam[5] = sessionName;
		
		sendDataParam[6] = mHandle.ToString ();
		
        for (i = 0; i < len; i++)
        {

            sendDataParam[i + 7] = System.Text.Encoding.Default.GetString(payload[i]);

        }

        object[] mulVars = new object[1];

        mulVars[0] = sendDataParam;

        reqId = mChordUnityServiceJava.Call<String>("sendUdpData", mulVars);

        return reqId;
	}
	
	public String  sendUdpDataToAll(String toChannel,int reliableTime, bool respFlag, String payloadType, byte[][] payload, String  sessionName )
	{
		String reqId = null;

        int i = 0;

        int len = payload.Length;

        CLog.Log("[ChordUnity][sendDataToAll]: len." + len);

        String[] sendDataParam = new String[len + 6];		

        sendDataParam[0] = toChannel;
		
		sendDataParam[1] = reliableTime.ToString();
		
		sendDataParam[2] = respFlag.ToString().ToLower();

        sendDataParam[3] = payloadType;
		
		sendDataParam[4] = sessionName;		
		
		sendDataParam[5] = mHandle.ToString ();

        for (i = 0; i < len; i++)
        {

            sendDataParam[i + 6] = System.Text.Encoding.Default.GetString(payload[i]);

        }

        object[] mulVars = new object[1];

        mulVars[0] = sendDataParam;

        reqId = mChordUnityServiceJava.Call<String>("sendUdpDataToAll", mulVars);       

        return reqId;
	}
   
	/**
      * browseFiles opens the file exporere for selecting the files to send.
      * 
      */

    public void browseFiles()
    {

        if (mChordUnityServiceJava != null)
        {					
			mChordUnityServiceJava.Call("browseFiles",mHandle);

        }

    }

	public void viewLicense()
	{
		if (mChordUnityServiceJava != null)
		{					
			CLog.Log("[ChordUnity][viewLicense]");
			mChordUnityServiceJava.Call("viewLicense");
			
		}
	}
	
	/**
         * Send file to a specific node on the channel.
         * 
         * @param toChannel The channel name that the file is sent to. It is mandatory.    
         * @param toNode The node name that the file is sent to. It is mandatory.
         * @param fileType User defined file type.
         * @param filePath The absolute path of the file to be transferred. It is mandatory.
         * @param timeoutMsec The time to allow the receiver to accept the receiving
         *            data requests.
         */

    public String sendFile(String toChannel, String toNode, String fileType, String strFilePath, int timeoutMsec)
    {

        String exchangeId = null;

        CLog.Log("[ChordUnity][sendFile]:  " + toChannel + " :: " + strFilePath + " : " + toNode);

        if (mChordUnityServiceJava != null)
        {

            CLog.Log("[ChordUnity][sendFile]:  " + strFilePath + "to:" + toNode);

            object[] mulArg = new object[6];

            mulArg[0] = toChannel;

            mulArg[1] = toNode;

            mulArg[2] = fileType;

            mulArg[3] = strFilePath;

            mulArg[4] = timeoutMsec;

			mulArg[5] = mHandle;

            exchangeId = mChordUnityServiceJava.Call<String>("sendFile", mulArg);

        }

        CLog.Log("[ChordUnity][sendFile]:  end.");

        return exchangeId;

    }

    /**
         * Send multiple fielse to a specific node on the channel.
         * 
         * @param toChannel The channel name that the multifiles sent to. It is mandatory.    
         * @param toNode The node name that the file is sent to. It is mandatory.
         * @param fileType User defined file type.
         * @param listFilePath The absolute path of the files to be transferred. It is mandatory.
         * @param timeout The time to allow the receiver to accept the receiving
         *            data requests.
         */

    public String sendMultiFiles(String toChannel, String toNode, String fileType, List<String> listFilePath, int timeout)
    {

        String trId = "";

        object[] mulArg = new object[6];

        mulArg[0] = toChannel;

        mulArg[1] = toNode;

        mulArg[2] = fileType;

        mulArg[3] = listFilePath.ToArray();

        mulArg[4] = timeout;

		mulArg[5] = mHandle;

        CLog.Log("[ChordUnity][sendMultiFiles]: Before sendMultiFiles  .");

        trId = mChordUnityServiceJava.Call<String>("sendMultiFiles", mulArg);

        CLog.Log("[ChordUnity][sendMultiFiles]: After sendMultiFiles  .");

        return trId;

    }

    /**
         * Accept to receive file.
         * Can be called once an onFileWillReceive() notification is received.
         * 
         * @param fromChannel The channel name that file request is received. It is mandatory.  
         * @param exchangeId Transmission identifier.
         * @param chunkTimeoutMsec The timeout to request the chunk data.
         * @param chunkRetries The count that allow to retry to request chunk data.
         * @param chunkSize Data size to invoke onFileChunkReceived.
         */

    public bool acceptFile(String fromChannel, String exchangeId, int chunkTimeoutMsec, int chunkRetries, long chunkSize)
    {

        bool result = false;

        CLog.Log("[ChordUnity][acceptFile]: ." + exchangeId + "from:" + fromChannel);

        object[] mulArg = new object[6];

        mulArg[0] = fromChannel;

        mulArg[1] = exchangeId;

        mulArg[2] = chunkTimeoutMsec;

        mulArg[3] = chunkRetries;

        mulArg[4] = chunkSize;

		mulArg[5] = mHandle;

        result = mChordUnityServiceJava.Call<bool>("acceptFile", mulArg);

        return result;

    }

    /**
         * Cancel file transfer.
         * Cancel file can be called before the receiver accepts or rejects the file transfer.
         * 
         * @param toChannel The channel name. It is mandatory.    
         * @param exchangeId Transmission identifier.
         */

    public bool cancelFile(String toChannel, String exchangeId)
    {

        bool result = false;

        CLog.Log("[ChordUnity][cancelFile]:  ." + exchangeId + "from:" + toChannel);

        object[] mulArg = new object[3];

        mulArg[0] = toChannel;

        mulArg[1] = exchangeId;

		mulArg[2] = mHandle;

        result = mChordUnityServiceJava.Call<bool>("cancelFile", mulArg);

        return result;

    }

    /**
        * Reject to receive file.
        * Can be called once an onFileWillReceive() notification is received.
        * 
        * @param toChannel The channel name. It is mandatory.         
        * @param exchangeId Transmission identifier.
        */

    public bool rejectFile(String toChannel, String exchangeId)
    {

        bool result = false;

        CLog.Log("[ChordUnity][rejectFile]:  ." + exchangeId + "from:" + toChannel);

        object[] mulArg = new object[3];

        mulArg[0] = toChannel;

        mulArg[1] = exchangeId;

		mulArg[2] = mHandle;

        result = mChordUnityServiceJava.Call<bool>("rejectFile", mulArg);

        return result;

    }

    /**
        * Accept to receive files.
        * Can be called once an onFileWillReceive() notification is received. is
        * called.
        * 
        * @param toChannel The channel name. It is mandatory.          
        * @param taskId Transmission identifier.
        * @param chunkTimeoutMsec The timeout to request the chunk data.
        * @param chunkRetries The count that allow to retry to request chunk data.
        * @param chunkSize Data size to invoke onMultiFilesReceived.
        */

    public bool acceptMultiFiles(String fromChannel, String taskId, int chunkTimeout, int chunkRetries, long chunkSize)
    {

        bool result = false;

        CLog.Log("[ChordUnity][acceptMultiFiles]:  ." + taskId + "from:" + fromChannel);

        object[] mulArg = new object[6];

        mulArg[0] = fromChannel;

        mulArg[1] = taskId;

        mulArg[2] = chunkTimeout;

        mulArg[3] = chunkRetries;

        mulArg[4] = chunkSize;

		mulArg[5] = mHandle;

        result = mChordUnityServiceJava.Call<bool>("acceptMultiFiles", mulArg);

        return result;

    }

    /**
     * Cancel transfer of multiple files.
     * Cancel files can be called before the receiver accepts or rejects the file transfer.
     * 
     * @param toChannel The channel name. It is mandatory.        
     * @param taskId Transmission identifier.

     */

    public bool cancelMultiFiles(String toChannel, String taskId)
    {

        CLog.Log("[ChordUnity][cancelMultiFiles]:  ." + taskId + "from:" + toChannel);

        bool result = false;

        object[] mulArg = new object[3];

        mulArg[0] = toChannel;

        mulArg[1] = taskId;

		mulArg[2] = mHandle;

        result = mChordUnityServiceJava.Call<bool>("cancelMultiFiles", mulArg);

        return result;

    }

    /**
     * Reject to transfer of multiple files.
     * Can be called once an onFileWillReceive() notification is received.
     * 
     * @param toChannel The channel name. It is mandatory.          
     * @param taskId Transmission identifier.
     */

    public bool rejectMultiFiles(String toChannel, String taskId)
    {

        CLog.Log("[ChordUnity][rejectMultiFiles]:  ." + taskId + "from:" + toChannel);

        bool result = false;

        object[] mulArg = new object[3];

        mulArg[0] = toChannel;

        mulArg[1] = taskId;

		mulArg[2] = mHandle;

        result = mChordUnityServiceJava.Call<bool>("rejectMultiFiles", mulArg);

        return result;

    }

    // onMultiFilesWillReceive listener callback

    private void onMultiFilesWillReceive(String hkey)
    {

        CLog.Log("[ChordUnity][onMultiFilesWillReceive]: hkey " + hkey);
		
		object[] mulArg = new object[2];		
		mulArg[0] = hkey;		
		mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onMultiFilesWillReceive]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onMultiFilesWillReceive]: data Length is zero for : " + hkey);
            return;
		}
        String fromNode = retValues[0];

        String fromChannel = retValues[1];

        String fileName = retValues[3];

        //String fileHash = retValues[5];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        int fileCount = Convert.ToInt32(retValues[11]);

        long fileSize = Convert.ToInt64(retValues[6]);

        mCaller.onMultiFilesWillReceive(fromNode, fromChannel, fileName,

                 exchangeId, fileCount, fileType, fileSize);

    }

    // onMultiFilesChunkReceived listener callback

    private void onMultiFilesChunkReceived(String hkey)
    {

        CLog.Log("[ChordUnity][onMultiFilesChunkReceived]: " + hkey);
		
		object[] mulArg = new object[2];		
		mulArg[0] = hkey;		
		mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onMultiFilesChunkReceived]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onMultiFilesChunkReceived]: data Length is zero for : " + hkey);
            return;
		}
		
        String fromNode = retValues[0];

        String fromChannel = retValues[1];

        String fileName = retValues[3];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        long fileSize = Convert.ToInt64(retValues[6]);

        long fileOffset = Convert.ToInt64(retValues[7]);

        int index = Convert.ToInt32(retValues[9]);

        mCaller.onMultiFilesChunkReceived(fromNode, fromChannel, fileName,

                 exchangeId, index, fileType, fileSize, fileOffset);

    }

    // onMultiFilesReceived listener callback

    private void onMultiFilesReceived(String hkey)
    {

        CLog.Log("[ChordUnity][onMultiFilesReceived]: hkey" + hkey);
		
		object[] mulArg = new object[2];		
		mulArg[0] = hkey;		
		mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onMultiFilesReceived]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onMultiFilesReceived]: data Length is zero for : " + hkey);
            return;
		}
        String fromNode = retValues[0];

        String fromChannel = retValues[1];

        String fileName = retValues[3];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        long fileSize = Convert.ToInt64(retValues[6]);

        String tempFilePath = retValues[12];

        int index = Convert.ToInt32(retValues[9]);

        mCaller.onMultiFilesReceived(fromNode, fromChannel, fileName,

                 exchangeId, index, fileType, fileSize, tempFilePath);

    }

    // onMultiFilesChunkSent listener callback

    private void onMultiFilesChunkSent(String hkey)
    {

        CLog.Log("[ChordUnity][onMultiFilesChunkSent]: hkey " + hkey);
		
		object[] mulArg = new object[2];		
		mulArg[0] = hkey;		
		mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);

		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onMultiFilesChunkSent]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onMultiFilesChunkSent]: data Length is zero for : " + hkey);
            return;
		}
        String toNode = retValues[0];

        String toChannel = retValues[1];

        String fileName = retValues[3];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        long fileSize = Convert.ToInt64(retValues[6]);

        long fileOffset = Convert.ToInt64(retValues[7]);

        long chunkSize = Convert.ToInt64(retValues[8]);

        int index = Convert.ToInt32(retValues[9]);

        mCaller.onMultiFilesChunkSent(toNode, toChannel, fileName,

                 exchangeId, index, fileType, fileSize, fileOffset, chunkSize);

    }

    // onMultiFilesSent listener callback

    private void onMultiFilesSent(String hkey)
    {

        CLog.Log("[ChordUnity][onMultiFilesSent]: " + hkey);
		
		object[] mulArg = new object[2];		
		mulArg[0] = hkey;		
		mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);

		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onMultiFilesSent]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onMultiFilesSent]: data Length is zero for : " + hkey);
            return;
		}
        String toNode = retValues[0];

        String toChannel = retValues[1];

        String fileName = retValues[3];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        int index = Convert.ToInt32(retValues[9]);

        mCaller.onMultiFilesSent(toNode, toChannel, fileName,

                 exchangeId, index, fileType);

    }

    // onMultiFilesFailed listener callback

    private void onMultiFilesFailed(String hkey)
    {

        CLog.Log("[ChordUnity][onMultiFilesFailed]: hkey " + hkey);
		
		object[] mulArg = new object[2];		
		mulArg[0] = hkey;		
		mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onMultiFilesFailed]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onMultiFilesFailed]: data Length is zero for : " + hkey);
            return;
		}
        String node = retValues[0];

        String channel = retValues[1];

        String fileName = retValues[3];

       // String fileType = retValues[4];

        String exchangeId = retValues[2];

        int index = Convert.ToInt32(retValues[9]);

        int reason = Convert.ToInt32(retValues[10]);

        mCaller.onMultiFilesFailed(node, channel, fileName,

                 exchangeId, index, reason);

    }

    // onMultiFilesFinished listener callback

    private void onMultiFilesFinished(String hkey)
    {

        CLog.Log("[ChordUnity][onMultiFilesFinished]: hkey " + hkey);
		
		object[] mulArg = new object[2];		
		mulArg[0] = hkey;		
		mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);

		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onMultiFilesFinished]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onMultiFilesFinished]: data Length is zero for : " + hkey);
            return;
		}
        
        String node = retValues[0];

        String channel = retValues[1];

        String exchangeId = retValues[2];

        int reason = Convert.ToInt32(retValues[10]);

        mCaller.onMultiFilesFinished(node, channel,

                 exchangeId, reason);

    }
	
	public String[] getNICList(int handle) 
	{	
		CLog.Log("[ChordUnity][getNICList]: Entry  ");		

		String[] nicList = mChordUnityServiceJava.Call<String[]>("getNICList",mHandle);
		
		return nicList;
	}
	
    /**
     * Get a list of available network interface types.
     * 
     * @return A list of available network interface types. return empty list, if no interfaces are available.
     *         {INTERFACE_TYPE_WIFI}
     *         {INTERFACE_TYPE_WIFI_AP}
     *         {INTERFACE_TYPE_WIFI_P2P}
     */

    public List<int> getAvailableInterfaceTypes()
    {

        List<int> InterfacesList = new List<int>();

        CLog.Log("[ChordUnity][getAvailableInterfaceTypes]: Entry  ");

        String[] interfaces = mChordUnityServiceJava.Call<String[]>("getAvailableInterfaceTypes",mHandle);

        if (interfaces != null)
        {
            int len = interfaces.Length;

            CLog.Log("[ChordUnity][getAvailableInterfaceTypes]: Interfaces Len  " + len);

            for (int i = 0; i < len; i++)
            {

                CLog.Log("[ChordUnity][getAvailableInterfaceTypes]: interface type ::  " + interfaces[i]);

                InterfacesList.Add(Convert.ToInt32(interfaces[i]));

            }
        }
        else
        {
            CLog.Log("[ChordUnity][getAvailableInterfaceTypes]: No interfaces available  ");
        }

        return InterfacesList;

    }

    /**
        * Get all joined channel names.
        * 
        * @return A list of names for joined channels. It returns an empty list, there is no joined channel.

        */

    public List<String> getJoinedChannelList()
    {

        List<String> channelNames = new List<String>();

        CLog.Log("[ChordUnity][getJoinedChannelList]: Entry  ");
		

		String[] channels = mChordUnityServiceJava.Call<String[]>("getJoinedChannelList",mHandle);

        if (channels == null)
        {
			CLog.Log("[ChordUnity][getJoinedChannelList]: No Channel  available  ");

			// return empty list
            return channelNames;
        }

        int len = channels.Length;

        for (int i = 0; i < len; i++)
        {

            channelNames.Add(channels[i]);

        }

        return channelNames;

    }

    /**
         * Get nodes on the channel.
         * 
         * @return A list of nodes names in the channel. It returns empty list, there is no joined node.
         */

    public List<String> getJoinedNodeList(String channelName)
    {

        List<String> nodeList = new List<String>();

        CLog.Log("[ChordUnity][getJoinedNodeList]: Entry  ");

		object[] mulArg = new object[3];
		
		mulArg[0] = channelName;	

		mulArg[1] = mHandle;

		String[] nodeNames = mChordUnityServiceJava.Call<String[]>("getJoinedNodeList", mulArg);

        if (nodeNames == null)
        {
			CLog.Log("[ChordUnity][getJoinedNodeList]: No Node  available  ");

			// return empty list
            return nodeList;
        }
		

        int len = nodeNames.Length;

        CLog.Log("[ChordUnity][getJoinedNodeList]: nodename Len  " + len);

        for (int i = 0; i < len; i++)
        {

            CLog.Log("[ChordUnity][getJoinedNodeList]: type ::  " + nodeNames[i]);

            nodeList.Add(nodeNames[i]);

        }

        return nodeList;

    }

    /**
         * Get an IPv4 address that the node has.
         * 
         * @param channelName The channelName name to find IPv4 address.     
         * @param nodeName The node name to find IPv4 address.
         * @return An IPv4 Address.When there is not the node name in the
         *         channel, null is returned.
         */

    public String getNodeIpAddress(String channelName, String nodeName)
    {

        CLog.Log("[ChordUnity][getNodeIpAddress]: Entry  ");

        object[] mulArg = new object[3];

        mulArg[0] = channelName;

        mulArg[1] = nodeName;

		mulArg[2] = mHandle;

        String ipAddress = mChordUnityServiceJava.Call<String>("getNodeIpAddress", mulArg);

        CLog.Log("[ChordUnity][getNodeIpAddress]: ip ::  " + ipAddress);

        return ipAddress;

    }

    /**
     * Stop chord. When chord is stopped successfully, it will be called onStopped of StatusListener
     * 
     */

    public void stop()
    {

        mChordUnityServiceJava.Call("stop",mHandle);

    }

    /**
        * Join a desired channel with a given listener.
        * In order to use secure mode, the feature must be set via setSecureModeEnabled(boolean enabled) and 
        * SECURE_PREFIX prefix to channelName
        * 
        * @param channelName Channel name. It should always begin with alphanumeric character. It is a mandatory input. 
        * return true if succesfully joined else false.
        */

    public bool joinChannel(String channelName)
    {
		object[] mulArg = new object[2];		
		mulArg[0] = channelName;		
		mulArg[1] = mHandle;
		return mChordUnityServiceJava.Call<bool>("joinChannel", mulArg);

    }

    /**
        * Leave a given channel.
        * 
        * @param channelName Channel name to leave.
        */

    public void leaveChannel(String channelName)
    {
		object[] mulArg = new object[2];		
		mulArg[0] = channelName;		
		mulArg[1] = mHandle;
		mChordUnityServiceJava.Call("leaveChannel", mulArg);

    }

    /**
     * Check whether this channel is secured or not.
     * 
     * @return True if this channel is secured.
     */

    bool isSecureChannel(String channelName)
    {
		object[] mulArg = new object[2];		
		mulArg[0] = channelName;		
		mulArg[1] = mHandle;
		return mChordUnityServiceJava.Call<bool>("isSecureChannel", mulArg);
    }

    /**
        * Get own node name.
        * 
        * @return An own node name. When it is not started, null is
        *         returned
        */

    public String getName()
    {		
		return mChordUnityServiceJava.Call<String>("getName",mHandle);

    }

    /**
     * Get own IPv4 address.
     * 
     * @return IPv4 Address corresponding to the interface type given by start.
     *          When it is not started, null is returned.
     */

    public String getIp()
    {
		return mChordUnityServiceJava.Call<String>("getIp",mHandle);

    }

    /**
         * Release resources to be used.
         * 
         */

    public void close()
    {

        mChordUnityServiceJava.Call("close",mHandle);

    }

    /**
        * Set a keep-alive timeout.
        * Node has keep-alive timeout.
        * The timeoutMsec determines the maximum keep-alive time to wait to leave
        * when there is no data from the nodes.
        * Default time is 34000 millisecond.
        * 
        * @param timeoutMsec Timeout with millisecond.
        */

    public void setNodeKeepAliveTimeout(int timeoutMsec)
    {
		object[] mulArg = new object[2];		
		mulArg[0] = timeoutMsec;		
		mulArg[1] = mHandle;
        mChordUnityServiceJava.Call("setNodeKeepAliveTimeout", mulArg);

    }

    /**
       * Set the number of files to send simultaneously using sendMultiFiles().
       * 
       * @param count The number of files to send simultaneously.
       */

    public void setSendMultiFilesLimitCount(int count)
    {
		object[] mulArg = new object[2];		
		mulArg[0] = count;		
		mulArg[1] = mHandle;
        mChordUnityServiceJava.Call("setSendMultiFilesLimitCount", mulArg);

    }


    /**
        * Set whether smart discovery is enabled.
        * It is true if you do not set.
        * The discovery time period is adapted by Chord. 
        * The period is reset to the default value in case of certain triggers 
        * such as a new node joining the network or when the device LCD is turned on. 
        * It is also possible to reinitialize this period explicitly by calling the resetSmartDiscoveryPeriod()
        * 
        * @param enabled Whether to use default discovery period or not
        */

    public void setSmartDiscoveryEnabled(bool enabled)
    {
		object[] mulArg = new object[2];		
		mulArg[0] = enabled;		
		mulArg[1] = mHandle;
        mChordUnityServiceJava.Call("setSmartDiscoveryEnabled", mulArg);

    }

    /**
     * Check whether smart discovery is enabled
     * 
     * @return True if it is enabled. 
     */

    public bool isSmartDiscoveryEnabled()
    {			
		
        return mChordUnityServiceJava.Call<bool>("isSmartDiscoveryEnabled",mHandle);

    }

    /**
     * Reset period for sending nodes
     * 
     */

    public void resetSmartDiscoveryPeriod()
    {

        mChordUnityServiceJava.Call("resetSmartDiscoveryPeriod",mHandle);

    }

    /**
        * Get the version of Chord, relative to other versions 
        * 
        * @return The version of Chord, relative to other versions.
        */

    public int getVersionCode()
    {

        return mChordUnityServiceJava.Call<int>("getVersionCode");

    }

    /**
         * Get the version name of Chord
         * 
         * @return The version name

         */

    public String getVersionName()
    {

        return mChordUnityServiceJava.Call<String>("getVersionName");

    }

    public int getLastError()
    {
        return mChordUnityServiceJava.Call<int>("getLastError");

    }

    // onConnected listener call back

    private void onConnected(String interfaceType)
    {

        CLog.Log("[ChordUnity][onConnected]: interfaceType " + interfaceType);

        if (mNtwkListener != null)

            mNtwkListener.onConnected(Convert.ToInt32(interfaceType));

    }

    // onDisconnected listener call back

    private void onDisconnected(String interfaceType)
    {

        CLog.Log("[ChordUnity][onDisconnected]: interfaceType " + interfaceType);

        if (mNtwkListener != null)

            mNtwkListener.onDisconnected(Convert.ToInt32(interfaceType));

    }

    // ChordUnityChannelServiceListener

    // onNodeJoinedEvent listener call back

    private void onNodeJoinedEvent(String hkey)
    {

        CLog.Log("[ChordUnity][onNodeJoinedEvent]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;
        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityNodeInfo", mulArg);

        if (retValues == null)
        {

            CLog.Log("[ChordUnity][onNodeJoinedEvent]: no data for : " + hkey);

            return;
        }
		
	if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onNodeJoinedEvent]: data Length is zero for : " + hkey);
            return;
        }
		
	    String nodeName = retValues[0];

        String fromChannel = retValues[1];

        CLog.Log("[ChordUnity][onNodeJoinedEvent]: received: nodeName : " + nodeName + " and channel: " + fromChannel);

        onNodeJoined(nodeName, fromChannel);

        CLog.Log("[ChordUnity][onNodeJoinedEvent]: end");
    }

    // onNodeJoined listener call back

    private void onNodeJoined(String fromNode, String fromChannel)
    {

        CLog.Log("[ChordUnity][onNodeJoined]: start: " + fromNode + "channel : " + fromChannel);

        mCaller.onNodeJoined(fromNode, fromChannel);

    }

    // onNodeLeftEvent listener call back

    private void onNodeLeftEvent(String hkey)
    {

        CLog.Log("[ChordUnity][onNodeLeftEvent]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;
        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityNodeInfo", mulArg);

        if (retValues == null)
        {

            CLog.Log("[ChordUnity][onNodeLeftEvent]: no data for : " + hkey);

            return;
        }
		
	if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onNodeLeftEvent]: data Length is zero for : " + hkey);
            return;
        }
	

        String nodeName = retValues[0];

        String fromChannel = retValues[1];

        CLog.Log("[ChordUnity][onNodeLeftEvent]: received: nodeName : " + nodeName + " and channel: " + fromChannel);

        onNodeLeft(nodeName, fromChannel);

        CLog.Log("[ChordUnity][onNodeLeftEvent]: end");
    }

    // onNodeLeft listener call back

    private void onNodeLeft(String fromNode, String fromChannel)
    {

        CLog.Log("[ChordUnity][onNodeLeft]: nodename: " + fromNode);

        mCaller.onNodeLeft(fromNode, fromChannel);

        CLog.Log("[ChordUnity][onNodeLeft]: end");

    }

    // onDataReceiveEvent listener call back

    private void onDataReceiveEvent(String hkey)
    {

        CLog.Log("[ChordUnity][onDataReceiveEvent]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityDataReceivedInfo", mulArg);

        if (retValues == null)
        {
            CLog.Log("[ChordUnity][onDataReceiveEvent]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onDataReceiveEvent]: data Length is zero for : " + hkey);
            return;
        }
		
        int len = retValues.Length;

        CLog.Log("[ChordUnity][onDataReceiveEvent]: len of retValues : " + len);

        String nodeName = retValues[0];

        String fromChannel = retValues[1];

        String payloadType = retValues[2];

        CLog.Log("[ChordUnity][onDataReceiveEvent]: received: nodeName : " + nodeName + " and channel: " + fromChannel + "type:" + payloadType);

        byte[][] payload = new byte[len - 3][];

        for (int j = 3; j < len; j++)
        {

            CLog.Log("[ChordUnity][onDataReceiveEvent]: received: payload1 : " + retValues[j]);

            payload[j - 3] = Encoding.UTF8.GetBytes(retValues[j]);

        }

        onDataReceived(nodeName, fromChannel, payloadType, payload);

        CLog.Log("[ChordUnity][onDataReceiveEvent]: end");

    }

    // onFileWillReceive listener call back

    private void onFileWillReceive(String hkey)
    {

        CLog.Log("[ChordUnity][onFileWillReceive]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;
        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);

        if (retValues == null)
        {
            CLog.Log("[ChordUnity][onFileWillReceive]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onFileWillReceive]: data Length is zero for : " + hkey);
            return;
        }
        
        String fromNode = retValues[0];

        CLog.Log("\n" + fromNode);

        String fromChannel = retValues[1];

        CLog.Log("\n" + fromChannel);

        String fileName = retValues[3];

        CLog.Log("\n" + fileName);

        String fileHash = retValues[5];

        CLog.Log("\n" + fileHash);

        String fileType = retValues[4];

        CLog.Log("\n" + fileType);

        String exchangeId = retValues[2];

        CLog.Log("\n" + exchangeId);

        long fileSize = Convert.ToInt64(retValues[6]);

        CLog.Log("\n" + fileSize);

        CLog.Log("[ChordUnity][onFileWillReceive]: from_node, from_channel, file_name,fhash,  file_type, exchange_id,  file_size=" + fromNode + fromChannel + fileName +

                 fileHash + fileType + exchangeId + fileSize);

        mCaller.onFileWillReceive(fromNode, fromChannel, fileName,

                 fileHash, fileType, exchangeId, fileSize);

    }

    // onFileChunkReceived listener call back

    private void onFileChunkReceived(String hkey)
    {

        CLog.Log("[ChordUnity][onFileChunkReceived]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onFileChunkReceived]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onFileChunkReceived]: data Length is zero for : " + hkey);
            return;
		}
        String fromNode = retValues[0];

        String fromChannel = retValues[1];

        String fileName = retValues[3];

        String fileHash = retValues[5];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        long fileSize = Convert.ToInt64(retValues[6]);

        long fileOffset = Convert.ToInt64(retValues[7]);


        mCaller.onFileChunkReceived(fromNode, fromChannel, fileName,

                 fileHash, fileType, exchangeId, fileSize, fileOffset);

    }

    // onFileReceived listener call back

    private void onFileReceived(String hkey)
    {

        CLog.Log("[ChordUnity][onFileReceived]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onFileReceived]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onFileReceived]: data Length is zero for : " + hkey);
            return;
		}
        String fromNode = retValues[0];

        String fromChannel = retValues[1];

        String fileName = retValues[3];

        String fileHash = retValues[5];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        long fileSize = Convert.ToInt64(retValues[6]);

        String filePath = retValues[12];

        mCaller.onFileReceived(fromNode, fromChannel, fileName,

                 fileHash, fileType, exchangeId, fileSize, filePath);

    }

    // onFileChunkSent listener call back

    private void onFileChunkSent(String hkey)
    {

        CLog.Log("[ChordUnity][onFileChunkSent]: hkey " + hkey);
		
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;


        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onFileChunkSent]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onFileChunkSent]: data Length is zero for : " + hkey);
            return;
		}
        String toNode = retValues[0];

        String toChannel = retValues[1];

        String fileName = retValues[3];

        String fileHash = retValues[5];

        String fileType = retValues[4];

        String exchangeId = retValues[2];

        long fileSize = Convert.ToInt64(retValues[6]);

        long fileOffset = Convert.ToInt64(retValues[7]);

        long chunkSize = Convert.ToInt64(retValues[8]);

        mCaller.onFileChunkSent(toNode, toChannel, fileName,

                 fileHash, fileType, exchangeId, fileSize, fileOffset, chunkSize);

    }

    // onFileSent listener call back

    private void onFileSent(String hkey)
    {

        CLog.Log("[ChordUnity][onFileSent]: hkey " + hkey);
		
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onFileSent]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onFileSent]: data Length is zero for : " + hkey);
            return;
		}
        String toNode = retValues[0];

        String toChannel = retValues[1];

        String fileName = retValues[3];

        String fileHash = retValues[5];

        String fileType = retValues[4];

        String exchangeId = retValues[2];


        mCaller.onFileSent(toNode, toChannel, fileName,

                 fileHash, fileType, exchangeId);

    }

    // onFileFailed listener call back

    private void onFileFailed(String hkey)
    {

        CLog.Log("[ChordUnity][onFileFailed]: hkey " + hkey);
		
		
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;


        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityFileTransferInfo",  mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onFileFailed]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onFileFailed]: data Length is zero for : " + hkey);
            return;
		}
        String node = retValues[0];

        String channel = retValues[1];

        String fileName = retValues[3];

        String fileHash = retValues[5];

        //String fileType = retValues[4];

        String exchangeId = retValues[2];

        int reason = Convert.ToInt32(retValues[10]);

        mCaller.onFileFailed(node, channel, fileName,

                 fileHash, exchangeId, reason);

    }

    // onDataReceived listener call back

    private void onDataReceived(String fromNode, String fromChannel, String payloadType, byte[][] payload)
    {

        CLog.Log("[ChordUnity][onDataReceived]: nodeName : " + fromNode + " and channel: " + fromChannel + "type:" + payloadType);

        mCaller.onDataReceived(fromNode, fromChannel, payloadType, payload);

    }

    // onStartedEvent listener call back

    private void onStartedEvent(String hkey)
    {

        CLog.Log("[ChordUnity][onStartedEvent]: hkey " + hkey);
		
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityNodeInfo", mulArg);

        if (retValues == null)
        {

            CLog.Log("[ChordUnity][onStartedEvent]: no data for : " + hkey);

            return;

        }
		
	if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onStartedEvent]: data Length is zero for : " + hkey);
            return;
        }

		        
        String nodeName = retValues[0];

        int reason = Convert.ToInt32(retValues[1]);

        CLog.Log("[ChordUnity][onStartedEvent]: nodeName : " + nodeName + " and reason: " + reason);

        onStarted(nodeName, reason);

        CLog.Log("[ChordUnity][onStartedEvent]: end");

    }

    // onStoppedEvent listener call back

    private void onStoppedEvent(String hkey)
    {

        CLog.Log("[ChordUnity][onStoppedEvent]: hkey " + hkey);
		
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityNodeInfo", mulArg);

        if (retValues == null)
        {

            CLog.Log("[ChordUnity][onStoppedEvent]: no data for : " + hkey);

            return;

        }
		
	if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onStoppedEvent]: data Length is zero for : " + hkey);
            return;
        }
	

        //String dummy = retValues[0];

        int reason = Convert.ToInt32(retValues[1]);

        CLog.Log("[ChordUnity][onStoppedEvent]: reason: " + reason);

        onStopped(reason);

        CLog.Log("[ChordUnity][onStoppedEvent]: end");

    }

    // onStarted listener call back

    private void onStarted(String nodeName, int reason)
    {

        CLog.Log("[ChordUnity][onStarted]:");

        mStatusListener.onStarted(nodeName, reason);

    }

    // onStopped listener call back

    public void onStopped(int reason)
    {

        CLog.Log("[ChordUnity][onStopped]:");

        mStatusListener.onStopped(reason);

    }
	private void onUdpDataReceivedEvent(String hkey)
	{
		CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityUdpDataReceivedInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: data Length is zero for : " + hkey);
            return;
        }
		
        int len = retValues.Length;

        CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: len of retValues : " + len);

        String nodeName = retValues[0];

        String fromChannel = retValues[1];

        String payloadType = retValues[2];
		
		String sessionName = retValues[3];

        CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: received: nodeName : " + nodeName + " and channel: " + fromChannel + "type:" + payloadType);

        byte[][] payload = new byte[len - 4][];

        for (int j = 4; j < len; j++)
        {

            CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: received: payload1 : " + retValues[j]);

            payload[j - 4] = Encoding.UTF8.GetBytes(retValues[j]);

        }

        onUdpDataReceived(nodeName, fromChannel, payloadType,payload,sessionName);

        CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: end");
		
	}
	      
	private void onUdpDataDeliveredEvent(String hkey)
	{
		CLog.Log("[ChordUnity][onUdpDataDeliveredEvent]: hkey " + hkey);
		object[] mulArg = new object[2];

        mulArg[0] = hkey;

        mulArg[1] = mHandle;

        String[] retValues = mChordUnityServiceJava.Call<String[]>("getUnityUdpDataDeliveredInfo", mulArg);
		
		if (retValues == null)
        {
            CLog.Log("[ChordUnity][onUdpDataDeliveredEvent]: NULL data for : " + hkey);
            return;
        }
        if (retValues.Length == 0)
        {
            CLog.Log("[ChordUnity][onUdpDataReceiveEvent]: data Length is zero for : " + hkey);
            return;
        }
		
        int len = retValues.Length;

        CLog.Log("[ChordUnity][onUdpDataDeliveredEvent]: len of retValues : " + len);

        String nodeName = retValues[0];

        String toChannel = retValues[1];

        String reqId = retValues[2];
		
        onUdpDataDelivered(nodeName, toChannel, reqId);

        CLog.Log("[ChordUnity][onUdpDataDeliveredEvent]: end");
	}
	private void onUdpDataReceived(string fromNode, string fromChannel, string payloadType,byte[][] payload,string sessionName)
	{
		mCaller.onUdpDataReceived(fromNode, fromChannel, payloadType, payload, sessionName);
		
	}
	      
	private void onUdpDataDelivered(string toNode, string toChannel, string reqId)
	{
		mCaller.onUdpDataDelivered(toNode, toChannel, reqId);
	}
    // onBrowseResult listener call back

    public void onBrowseResult(String fileName)
    {
        CLog.Log("[ChordUnity][onBrowseResult]:");

        String[] SelectedFiles = mChordUnityServiceJava.Call<String[]>("getSelectedFiles");

        mCaller.onBrowseResult(SelectedFiles);

    }


}

