/*============================== FILE HEADER ================================= * *           Copyright (c) 2013, Samsung Research Institute India , Bangalore *                      Samsung Confidential Proprietary *                            All Rights Reserved *============================================================================== *                               Module Name : Java Chord Unity Interface============================================================================== * File                    : ChordUnityService.java * * Author(s)           : Ganesh VR , Dhanya R * * Description         : Java Interface Implementation  between  the C# ChordUnityInterface & Java Chord Service layer. *============================================================================== *                            Modification History *----------------------------------------------------------------------------- *    Date       |       Name                    |        Modification *----------------------------------------------------------------------------- *                  |                               		| *  24-07-13  | Ganesh VR , Dhanya R        | Newly Added *                  |                               		|============================================================================*/
package com.samsung.android.sdk.chordUnity;
import com.samsung.android.sdk.SsdkUnsupportedException;import com.samsung.android.sdk.chord.InvalidInterfaceException;import com.samsung.android.sdk.chord.Schord;import com.samsung.android.sdk.chord.SchordChannel;import com.samsung.android.sdk.chord.SchordManager;import com.unity3d.player.UnityPlayer;import android.app.Service;import android.content.Intent;import android.os.Binder;import android.os.Handler;import android.os.IBinder;import android.os.Message;import android.os.StatFs;import android.util.Log;import android.widget.Toast;import java.io.File;import java.io.FileNotFoundException;import java.util.Arrays;import java.util.HashMap;import java.util.List;//import com.samsung.android.sdk.chord.example.R;
public class ChordUnityService extends Service {
    public static String UNITY_TEST_CHANNEL = "UNITY_TEST_CHANNEL";
    private static final String TAG = "[Chord][ChordUnityService]";
    private static final String TAGClass = "ChordUnityService : ";
    private static final String CHORD_UNITY_MESSAGE_TYPE = "CHORD_UNITY_MESSAGE_TYPE";
    // private static final String PUBLIC_CHANNEL = "Chord";
    // public static String chordFilePath =    // Environment.getExternalStorageDirectory()
    // .getAbsolutePath() + "/ChordUnity";    private Schord sChord = null;
    private String[] selectedFiles = null;    private int selectedFilesHandle = -1;
    public static ChordUnityService mChordUnityService = null;
    private IChordUnityFileBrowseListener mUnityFileBrowseListener;    private IChordUnityCloseListener mUnityCloseListener;    private IChordUnityViewLicenseListener mUnityViewLicenseListener;

    private static class ChordUnityEvent {
        public String fromNode;
        public String channel_reason; // channel or reason
        public String payloadType;        public String sessionNameUdp;        public String reqIdUdp;
        public byte[][] payLoad;
    };    public class unityINterfaceInfo {        public int Handle;        public String UnityGameObject;        public SchordManager mChord;        public HashMap<String, ChordUnityEvent> mChordUnityEventMap;        public HashMap<String, ChordUnityFileTransferEvent> mChordUnityFileTransferEventMap;        public derivedChannelListener cListner;// = null;        public derivedStatusListener sListner;// = null;        public derivedNetworkListener nwListner;// = null;        public String tempDirPath;    }    // hashmap    private HashMap<Integer, unityINterfaceInfo> handleInterfaceInfoMap = null;
    private static int hashKey = 0;
    private static int ftHashKey = 0;

    // for notifying file transfer event to Activity
    private static class ChordUnityFileTransferEvent {
        public String node;
        public String channel;        public String fileName;
        public String hash;        public String fileType;
        public String tempFilePath;
        public long fileSize;
        public long offset;
        public long chunkSize;
        public String exchangeId;
        public int totalCount;
        public int reason;
        public int index;
    };    private static int ERROR_ILLEGAL_STATE = 3001;    private static int ERROR_ILLEGAL_ARGUMENT = 3002;    private static int ERROR_RUNTIME_EXCEPTION = 3003;    private static int ERROR_FILE_NOT_FOUND = 3004;    private static int ERROR_INVALID_INTERFACE = 3005;    private static int ERROR_SSDK_UNSUPPORTED = 3006;    private static int ERROR_CHORD_UNINITIALIZED = 3007;    public static int SUCCESS = 0;    private static int lastError = 0;
    public interface IChordUnityFileBrowseListener {
        public void onFileSelect();
    }    public interface IChordUnityCloseListener {        public void onCloseApp();    }
    public interface IChordUnityViewLicenseListener {        public void onRequestViewLicense();    }    public IChordUnityFileBrowseListener getChordUnityFileBrowseListener() {
        return mUnityFileBrowseListener;    }
    public void setChordUnityFileBrowseListener(IChordUnityFileBrowseListener mUnityListener) {
        this.mUnityFileBrowseListener = mUnityListener;    }
    public void setChordUnityViewLicenseListener(IChordUnityViewLicenseListener mUnityListener) {        this.mUnityViewLicenseListener = mUnityListener;    }    @Override
    public IBinder onBind(Intent intent) {
        Log.d(TAG, TAGClass + "onBind()");
        return mBinder;    }
    @Override
    public void onCreate() {
        Log.d(TAG, TAGClass + "onCreate()");
        super.onCreate();    }
    @Override
    public void onDestroy() {
        Log.d(TAG, TAGClass + "onDestroy()");
        super.onDestroy();
        try {
            release();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
    @Override
    public void onRebind(Intent intent) {
        Log.d(TAG, TAGClass + "onRebind()");
        super.onRebind(intent);
    }
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        Log.d(TAG, TAGClass + "onStartCommand()");
        return super.onStartCommand(intent, START_NOT_STICKY, startId);
    }
    @Override
    public boolean onUnbind(Intent intent) {
        Log.d(TAG, TAGClass + "onUnbind()");
        return super.onUnbind(intent);
    }
    public class ChordServiceBinder extends Binder {
        public ChordUnityService getService() {
            return ChordUnityService.this;
        }    }
    private final IBinder mBinder = new ChordServiceBinder();
    // Gets ChordUnityService object    public static ChordUnityService CreateObject(int Handle) {
        int trail = 0;
        if (mChordUnityService == null) {
            Log.d(TAG, TAGClass + "Service is not started , UnityPlayer Activity has to wait");
            while (true) {
                if (trail > 3) {
                    Log.d(TAG, TAGClass + "Service is not started still ERROR ");                    setLastError(ERROR_CHORD_UNINITIALIZED);
                    return null;
                }
                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
                if (mChordUnityService == null) {
                    Log.d(TAG, TAGClass + "Service is not started , wait again ");
                    trail++;
                }
                else {
                    Log.d(TAG, TAGClass + "Service is  started now ");
                    // return mChordUnityService;                    break;
                }
            }
        }
        Log.d(TAG, TAGClass + "Service is  started already ");        mChordUnityService.createCordManager(Handle);
        return mChordUnityService;
    }    private void createCordManager(int Handle) {        SchordManager tempChordManager = null;        try {            tempChordManager = new SchordManager(this);        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "SchordManager : " + e);            setLastError(ERROR_ILLEGAL_STATE);        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "SchordManager : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);        }        Log.d(TAG, TAGClass + "[Initialize] Chord Initialized");        // #2. set some values before start        tempChordManager.setLooper(getMainLooper());        // Optional.        // If you need listening network changed, you can set callback before        // starting chord.        // save the service, which can be called by other activity.        Log.d(TAG, TAGClass + "Service is  started: set the object ");        unityINterfaceInfo tempInfo = new unityINterfaceInfo();        tempInfo.cListner = null;        tempInfo.sListner = null;        tempInfo.nwListner = null;        tempInfo.Handle = Handle;        tempInfo.mChord = tempChordManager;        tempInfo.mChordUnityEventMap = new HashMap<String, ChordUnityEvent>();        tempInfo.mChordUnityFileTransferEventMap = new HashMap<String, ChordUnityFileTransferEvent>();        tempInfo.UnityGameObject = "_ChordUnityManager" + Handle;        handleInterfaceInfoMap.put(Handle, tempInfo);    }
    // Initialize chord
    public void initialize() throws Exception {
        handleInterfaceInfoMap = new HashMap<Integer, unityINterfaceInfo>();
        // #1. GetInstance
        sChord = new Schord();
        try {
            sChord.initialize(this);
        } catch (SsdkUnsupportedException e) {
            if (e.getType() == SsdkUnsupportedException.VENDOR_NOT_SUPPORTED) {
                // Vendor is not SAMSUNG            }            setLastError(ERROR_SSDK_UNSUPPORTED);
        }
        mChordUnityService = this;
    }
    // starts chord
    public int start(int interfaceType, int handle) {
        Log.d(TAG, TAGClass + " : start called from Unity 3D with no-security");        try {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return ERROR_ILLEGAL_ARGUMENT;            SchordManager chordManager = infoObj.mChord;            if (chordManager == null)                return ERROR_ILLEGAL_ARGUMENT;            if (null == infoObj.sListner) {                derivedStatusListener sListener = new derivedStatusListener(handle, mStatusListener);                infoObj.sListner = sListener;            }            chordManager.start(interfaceType, infoObj.sListner);            return SUCCESS;        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "start : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return ERROR_ILLEGAL_STATE;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "start : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return ERROR_ILLEGAL_ARGUMENT;        } catch (InvalidInterfaceException e) {            Log.d(TAG, TAGClass + "start : " + e);            setLastError(ERROR_INVALID_INTERFACE);            return ERROR_INVALID_INTERFACE;        } catch (RuntimeException e) {            Log.e(TAG, TAGClass + "start : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);            return ERROR_RUNTIME_EXCEPTION;        }    }    public void setNetworkListener(int handle) {        Log.d(TAG, TAGClass + "setNetworkListener ");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            Log.d(TAG, TAGClass + "setNetworkListener ");            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        if (infoObj.mChord == null) {            Log.d(TAG, TAGClass + "setNetworkListener infoObj.mChord");            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        if (null == infoObj.nwListner) {            Log.d(TAG, TAGClass + "setNetworkListener infoObj.nwListner is null");            infoObj.nwListner = new derivedNetworkListener(handle, mNetworkListener);            // infoObj.nwListner=nListener;        }        Log.d(TAG, TAGClass + "setNetworkListener infoObj.nwListner is not null");        infoObj.mChord.setNetworkListener(infoObj.nwListner);        Log.d(TAG, TAGClass + "setNetworkListener - succesfully registered");    }    /**     * NetworkListener     */    private class derivedNetworkListener implements SchordManager.NetworkListener {        private int mHandle = 0;        private UnityNetworkListener nwListener = null;        public derivedNetworkListener(int handle, UnityNetworkListener listener) {            mHandle = handle;            nwListener = listener;        }        @Override        public void onConnected(int interfaceType) {            Log.d(TAG, TAGClass + "ConConnected - Interface :" + interfaceType);            nwListener.onConnected(mHandle, interfaceType);        }        @Override        public void onDisconnected(int interfaceType) {            Log.d(TAG, TAGClass + "onDisConnected - Interface :" + interfaceType);            nwListener.onDisconnected(mHandle, interfaceType);        }    }    private class UnityNetworkListener {        public void onDisconnected(int handle, int interfaceType) {            Log.e("NETTEST", interfaceType + " : onDisconnected");            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onDisconnected",                    String.valueOf(interfaceType));        }        public void onConnected(int handle, int interfaceType) {            Log.e("NETTEST", interfaceType + " : onConnected");            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onConnected",                    String.valueOf(interfaceType));        }    }    private UnityNetworkListener mNetworkListener = new UnityNetworkListener();    // ***************************************************    // ChordManagerListener    // ***************************************************    private class derivedStatusListener implements SchordManager.StatusListener {        int mHandle = 0;        UnityStatusListener sListener = null;        public derivedStatusListener(int handle, UnityStatusListener listener) {            mHandle = handle;            sListener = listener;        }        @Override        public void onStarted(String name, int reason) {            sListener.onStarted(mHandle, name, reason);        }        @Override        public void onStopped(int reason) {            sListener.onStopped(mHandle, reason);        }    }    /*     * SchordManager.StatusListener mManagerListener = new     * SchordManager.StatusListener() {     */    private class UnityStatusListener {        public void onStarted(int handle, String name, int reason) {            Log.d(TAG, TAGClass + "onStarted chord");            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;            ChordUnityEvent obj = new ChordUnityEvent();            obj.fromNode = name;            obj.channel_reason = String.valueOf(reason);            obj.payloadType = null;            obj.payLoad = null;            synchronized (infoObj.mChordUnityEventMap) {                hashKey++;                hKey = "name" + hashKey;                infoObj.mChordUnityEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + "onStarted(), hKey : " + hKey + ", name : " + obj.fromNode                    + " reason:" + obj.channel_reason);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onStartedEvent", hKey);            if (SchordManager.StatusListener.STARTED_BY_RECONNECTION == reason) {                Log.e(TAG, TAGClass + "STARTED_BY_RECONNECTION");                return;            }            mHandler.sendEmptyMessageDelayed(MSG_SENDDATA, 200);        }        public void onStopped(int handle, int reason) {            Log.e(TAG, TAGClass + "onStopped - " + reason);            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;            ChordUnityEvent obj = new ChordUnityEvent();            obj.fromNode = " ";            obj.channel_reason = String.valueOf(reason);            obj.payloadType = null;            obj.payLoad = null;            Log.v(TAG, TAGClass + "onStppped(), reason:" + obj.channel_reason);            synchronized (infoObj.mChordUnityEventMap) {                hashKey++;                hKey = "name" + hashKey;                infoObj.mChordUnityEventMap.put(hKey, obj);            }            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onStoppedEvent", hKey);        }    }    private UnityStatusListener mStatusListener = new UnityStatusListener();    // private class UnityChannelListener;    // This interface defines a listener for chord channel events.    private class derivedChannelListener implements SchordChannel.StatusListener {        private int mHandle = 0;        private UnityChannelListener mUnityListener = null;        public derivedChannelListener(int handle, UnityChannelListener unityListener) {            mHandle = handle;            mUnityListener = unityListener;        }        @Override        public void onDataReceived(String arg0, String arg1, String arg2, byte[][] arg3) {            mUnityListener.onDataReceived(mHandle, arg0, arg1, arg2, arg3);        }        @Override        public void onFileChunkReceived(String fromNode, String fromChannel, String fileName,                String hash, String fileType, String exchangeId, long fileSize, long offset) {            mUnityListener.onFileChunkReceived(mHandle, fromNode, fromChannel, fileName, hash,                    fileType, exchangeId, fileSize, offset);        }        @Override        public void onFileChunkSent(String toNode, String toChannel, String fileName, String hash,                String fileType, String exchangeId, long fileSize, long offset, long chunkSize) {            mUnityListener.onFileChunkSent(mHandle, toNode, toChannel, fileName, hash, fileType,                    exchangeId, fileSize, offset, chunkSize);        }        @Override        public void onFileFailed(String node, String channel, String fileName, String hash,                String exchangeId, int reason) {            mUnityListener.onFileFailed(mHandle, node, channel, fileName, hash, exchangeId, reason);        }        @Override        public void onFileReceived(String fromNode, String fromChannel, String fileName,                String hash, String fileType, String exchangeId, long fileSize, String tmpFilePath) {            mUnityListener.onFileReceived(mHandle, fromNode, fromChannel, fileName, hash, fileType,                    exchangeId, fileSize, tmpFilePath);        }        @Override        public void onFileSent(String toNode, String toChannel, String fileName, String hash,                String fileType, String exchangeId) {            mUnityListener.onFileSent(mHandle, toNode, toChannel, fileName, hash, fileType,                    exchangeId);        }        @Override        public void onFileWillReceive(String fromNode, String fromChannel, String fileName,                String hash, String fileType, String exchangeId, long fileSize) {            mUnityListener.onFileWillReceive(mHandle, fromNode, fromChannel, fileName, hash,                    fileType, exchangeId, fileSize);        }        @Override        public void onMultiFilesChunkReceived(String fromNode, String fromChannel, String fileName,                String taskId, int index, String fileType, long fileSize, long offset) {            mUnityListener.onMultiFilesChunkReceived(mHandle, fromNode, fromChannel, fileName,                    taskId, index, fileType, fileSize, offset);        }        @Override        public void onMultiFilesChunkSent(String toNode, String toChannel, String fileName,                String taskId, int index, String fileType, long fileSize, long offset,                long chunkSize) {            mUnityListener.onMultiFilesChunkSent(mHandle, toNode, toChannel, fileName, taskId,                    index, fileType, fileSize, offset, chunkSize);        }        @Override        public void onMultiFilesFailed(String node, String channel, String fileName, String taskId,                int index, int reason) {            mUnityListener.onMultiFilesFailed(mHandle, node, channel, fileName, taskId, index,                    reason);        }        @Override        public void onMultiFilesFinished(String node, String channel, String taskId, int reason) {            mUnityListener.onMultiFilesFinished(mHandle, node, channel, taskId, reason);        }        @Override        public void onMultiFilesReceived(String fromNode, String fromChannel, String fileName,                String taskId, int index, String fileType, long fileSize, String tmpFilePath) {            mUnityListener.onMultiFilesReceived(mHandle, fromNode, fromChannel, fileName, taskId,                    index, fileType, fileSize, tmpFilePath);        }        @Override        public void onMultiFilesSent(String toNode, String toChannel, String fileName,                String taskId, int index, String fileType) {            mUnityListener.onMultiFilesSent(mHandle, toNode, toChannel, fileName, taskId, index,                    fileType);        }        @Override        public void onMultiFilesWillReceive(String fromNode, String fromChannel, String fileName,                String taskId, int count, String fileType, long fileSize) {            mUnityListener.onMultiFilesWillReceive(mHandle, fromNode, fromChannel, fileName,                    taskId, count, fileType, fileSize);        }        @Override        public void onNodeJoined(String arg0, String arg1) {            mUnityListener.onNodeJoined(mHandle, arg0, arg1);        }        @Override        public void onNodeLeft(String arg0, String arg1) {            mUnityListener.onNodeLeft(mHandle, arg0, arg1);        }        @Override        public void onUdpDataDelivered(String toNode, String toChannel, String reqId) {            mUnityListener.onUdpDataDelivered(mHandle, toNode, toChannel, reqId);        }        @Override        public void onUdpDataReceived(String fromNode, String fromChannel, String payloadType,                byte[][] payload, String sessionName) {            mUnityListener.onUdpDataReceived(mHandle, fromNode, fromChannel, payloadType, payload,                    sessionName);        }    }
    private class UnityChannelListener {

        public void onNodeJoined(int handle, String fromNode, String fromChannel) {
            Log.v(TAG, TAGClass + "onNodeJoined(), fromNode : " + fromNode + ", fromChannel : "
                    + fromChannel);            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            SchordManager mChordManager = infoObj.mChord;            String hKey;            SchordChannel channel = mChordManager.getJoinedChannel(fromChannel);
            String ipAddr = null;
            if (null != channel) {
                try {
                    ipAddr = channel.getNodeIpAddress(fromNode);                    Toast.makeText(getApplicationContext(), fromNode + " " + ipAddr,                            Toast.LENGTH_LONG).show();                    Log.d(TAG, TAGClass + "onNodeJoined() : " + ipAddr);
                } catch (IllegalArgumentException e) {
                    Log.d(TAG, TAGClass + e.getMessage());                }
            }
            ChordUnityEvent obj = new ChordUnityEvent();
            obj.fromNode = fromNode;
            obj.channel_reason = fromChannel;
            obj.payloadType = null;
            obj.payLoad = null;            synchronized (infoObj.mChordUnityEventMap) {                hashKey++;                hKey = "fromNode" + hashKey;                infoObj.mChordUnityEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + "onNodeJoined(), hKey : " + hKey + ", fromChannel : "
                    + obj.fromNode + obj.channel_reason);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onNodeJoinedEvent", hKey);
        }
        public void onNodeLeft(int handle, String fromNode, String fromChannel) {
            Log.v(TAG, TAGClass + "onNodeLeft(), fromNode : " + fromNode + ", fromChannel : "
                    + fromChannel);            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;
            ChordUnityEvent obj = new ChordUnityEvent();
            obj.fromNode = fromNode;
            obj.channel_reason = fromChannel;
            obj.payloadType = null;            obj.payLoad = null;            synchronized (infoObj.mChordUnityEventMap) {                hashKey++;                hKey = "fromNode" + hashKey;                infoObj.mChordUnityEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + "onNodeLeft(), hKey : " + hKey + ", fromChannel : "
                    + obj.fromNode + obj.channel_reason);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onNodeLeftEvent", hKey);
        }

        public void onDataReceived(int handle, String fromNode, String fromChannel,
                String payloadType, byte[][] payload) {            Log.v(TAG, TAGClass + "onDataReceived()");            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;
            ChordUnityEvent obj = new ChordUnityEvent();
            obj.fromNode = fromNode;            obj.channel_reason = fromChannel;
            obj.payloadType = payloadType;            obj.sessionNameUdp = "";            obj.reqIdUdp = "0";
            obj.payLoad = payload;
            if (obj.payLoad.length != payload.length) {                Log.v(TAG, TAGClass + "onDataReceived: payload not copied corrctly");
            }            synchronized (infoObj.mChordUnityEventMap) {                hashKey++;                hKey = "fromNode" + hashKey;                infoObj.mChordUnityEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + "onDataReceived(), hKey : " + hKey + ", fromChannel : "
                    + obj.fromNode + obj.channel_reason);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onDataReceiveEvent", hKey);        }

        public void onFileWillReceive(int handle, String fromNode, String fromChannel,
                String fileName, String hash, String fileType, String exchangeId, long fileSize) {            String hKey;            Log.d(TAG, TAGClass + "[originalName : " + fileName + " from : " + fromNode
                    + " exchangeId : " + exchangeId + " fileSize : " + fileSize + "]");
            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String tempFilePath = infoObj.tempDirPath;            File targetdir = new File(tempFilePath);
            if (!targetdir.exists()) {
                targetdir.mkdirs();
            }
            // Because the external storage may be unavailable,
            // you should verify that the volume is available before accessing            // it.            // But also, onFileFailed with ERROR_FILE_SEND_FAILED will be called
            // while Chord got failed to write file.            StatFs stat = new StatFs(tempFilePath);
            long blockSize = stat.getBlockSize();
            long totalBlocks = stat.getAvailableBlocks();            long availableMemory = blockSize * totalBlocks;
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = fromNode;
            obj.channel = fromChannel;
            obj.fileName = fileName;
            obj.hash = hash;
            obj.fileType = fileType;
            obj.fileSize = fileSize;
            obj.exchangeId = exchangeId;
            obj.offset = 0;
            obj.chunkSize = 0;            obj.index = 0;
            obj.reason = 0;
            obj.tempFilePath = "";
            obj.totalCount = 1;
            if (availableMemory < fileSize) {
                rejectFile(fromChannel, exchangeId, SchordManager.INTERFACE_TYPE_WIFI);
                obj.reason = SchordChannel.StatusListener.ERROR_FILE_NO_RESOURCE;
                Log.v(TAG, TAGClass + "onFileWillReceive, hKey : " + obj.exchangeId);                synchronized (infoObj.mChordUnityFileTransferEventMap) {                    ftHashKey++;                    hKey = exchangeId + ftHashKey;                    infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
                }                UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileFailed", hKey);
                return;
            }
            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = exchangeId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
            }            Log.d(TAG, TAGClass + "ChordUnityService.java- onFileWillReceive");
            Log.v(TAG, TAGClass + "onFileWillReceive, hKey : " + hKey);
            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileWillReceive", hKey);
        }

        public void onFileChunkReceived(int handle, String fromNode, String fromChannel,
                String fileName, String hash, String fileType, String exchangeId, long fileSize,
                long offset) {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey = exchangeId + "FileChunkRecv";            boolean bKeyExists = false;            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = fromNode;
            obj.channel = fromChannel;            obj.fileName = fileName;
            obj.hash = hash;
            obj.fileType = fileType;
            obj.fileSize = fileSize;
            obj.offset = offset;
            obj.exchangeId = exchangeId;
            obj.chunkSize = 0;
            obj.index = 0;
            obj.reason = 0;
            obj.tempFilePath = "";
            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                if (infoObj.mChordUnityFileTransferEventMap.containsKey(hKey)) {                    bKeyExists = true;                }                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + " onFileChunkReceived, hKey : " + hKey);            if (!bKeyExists) {                UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileChunkReceived", hKey);            }        }

        public void onFileReceived(int handle, String fromNode, String fromChannel,
                String fileName, String hash, String fileType, String exchangeId, long fileSize,
                String tmpFilePath) {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = fromNode;
            obj.channel = fromChannel;
            obj.fileName = fileName;
            obj.hash = hash;
            obj.fileType = fileType;
            obj.fileSize = fileSize;
            obj.exchangeId = exchangeId;
            obj.tempFilePath = tmpFilePath;
            obj.offset = 0;
            obj.chunkSize = 0;
            obj.index = 0;
            obj.reason = 0;
            obj.totalCount = 1;
            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = exchangeId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
            }            Log.v(TAG, TAGClass + " onFileReceived, hKey : " + hKey);
            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileReceived", hKey);
        }

        public void onFileChunkSent(int handle, String toNode, String toChannel, String fileName,
                String hash, String fileType, String exchangeId, long fileSize, long offset,
                long chunkSize) {            boolean bKeyExists = false;            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey = exchangeId + "FileChunkSent";
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();            obj.node = toNode;            obj.channel = toChannel;
            obj.fileName = fileName;            obj.fileType = fileType;
            obj.hash = hash;
            obj.exchangeId = exchangeId;
            obj.fileSize = fileSize;
            obj.offset = offset;
            obj.chunkSize = chunkSize;
            obj.index = 0;
            obj.reason = 0;
            obj.tempFilePath = "";
            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                // ftHashKey++;                // hKey = exchangeId+ftHashKey;                if (infoObj.mChordUnityFileTransferEventMap.containsKey(hKey)) {                    bKeyExists = true;                }                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
            }            if (!bKeyExists) {                UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileChunkSent", hKey);
            }        }

        public void onFileSent(int handle, String toNode, String toChannel, String fileName,
                String hash, String fileType, String exchangeId) {
            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = toNode;            obj.channel = toChannel;
            obj.fileName = fileName;
            obj.fileType = fileType;
            obj.hash = hash;
            obj.exchangeId = exchangeId;
            obj.fileSize = 0;
            obj.offset = 0;
            obj.chunkSize = 0;
            obj.index = 0;
            obj.reason = 0;
            obj.tempFilePath = "";
            obj.totalCount = 1;
            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = exchangeId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
            }            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileSent", hKey);        }

        public void onFileFailed(int handle, String node, String channel, String fileName,
                String hash, String exchangeId, int reason) {
            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = node;
            obj.channel = channel;
            obj.fileName = fileName;
            obj.hash = hash;            obj.exchangeId = exchangeId;
            obj.reason = reason;
            obj.fileType = "";
            obj.fileSize = 0;
            obj.offset = 0;
            obj.chunkSize = 0;            obj.index = 0;            obj.tempFilePath = "";            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = exchangeId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + " onFileFailed, hKey : " + hKey);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileFailed", hKey);        }

        public void onMultiFilesWillReceive(int handle, String fromNode, String fromChannel,                String fileName, String taskId, int count, String fileType, long fileSize) {            Log.d(TAG, TAGClass + "[originalName : " + fileName + " from : " + fromNode
                    + " taskId : " + taskId + " count : " + count + " fileSize : " + fileSize + "]");
            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String tempFilePath = infoObj.tempDirPath;            File targetdir = new File(tempFilePath);
            if (!targetdir.exists()) {                targetdir.mkdirs();            }
            // Because the external storage may be unavailable,
            // you should verify that the volume is available before accessing
            // it.            // But also, onFileFailed with ERROR_FILE_SEND_FAILED will be called
            // while Chord got failed to write file.            StatFs stat = new StatFs(tempFilePath);
            long blockSize = stat.getBlockSize();
            long totalBlocks = stat.getAvailableBlocks();
            long availableMemory = blockSize * totalBlocks;
            String hKey;
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = fromNode;
            obj.channel = fromChannel;
            obj.exchangeId = taskId;
            obj.fileName = fileName;
            obj.fileType = fileType;
            obj.totalCount = count;
            obj.fileSize = fileSize;
            obj.hash = "";
            obj.offset = 0;
            obj.chunkSize = 0;
            obj.index = 0;
            obj.reason = 0;
            obj.tempFilePath = "";
            if (availableMemory < fileSize) {
                rejectFile(fromChannel, taskId, SchordManager.INTERFACE_TYPE_WIFI);
                obj.reason = SchordChannel.StatusListener.ERROR_FILE_NO_RESOURCE;
                synchronized (infoObj.mChordUnityFileTransferEventMap) {                    ftHashKey++;                    hKey = taskId + ftHashKey;                    infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
                }                Log.v(TAG, TAGClass + " onFileFailed, hKey : " + hKey);
                UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onFileFailed", hKey);
                return;            }            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = taskId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
            }            Log.v(TAG, TAGClass + " onMultiFilesWillReceive, hKey : " + hKey);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onMultiFilesWillReceive", hKey);        }

        public void onMultiFilesChunkReceived(int handle, String fromNode, String fromChannel,
                String fileName, String taskId, int index, String fileType, long fileSize,                long offset) {            boolean bKeyExists = false;            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey = taskId + "MultiFileChunkRecv";
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = fromNode;            obj.channel = fromChannel;            obj.fileName = fileName;            obj.fileType = fileType;
            obj.exchangeId = taskId;            obj.index = index;            obj.fileSize = fileSize;            obj.offset = offset;            obj.hash = "";            obj.chunkSize = 0;            obj.reason = 0;            obj.tempFilePath = "";            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                if (infoObj.mChordUnityFileTransferEventMap.containsKey(hKey)) {                    bKeyExists = true;                }                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
            }            Log.v(TAG, TAGClass + " onMultiFilesChunkReceived, hKey : " + hKey);
            if (!bKeyExists) {                UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onMultiFilesChunkReceived",
                        hKey);            }        }
        public void onMultiFilesReceived(int handle, String fromNode, String fromChannel,                String fileName, String taskId, int index, String fileType, long fileSize,                String tmpFilePath) {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();            obj.node = fromNode;            obj.channel = fromChannel;            obj.fileName = fileName;            obj.fileType = fileType;            obj.exchangeId = taskId;            obj.index = index;
            obj.fileSize = fileSize;            obj.tempFilePath = tmpFilePath;            obj.hash = "";            obj.offset = 0;            obj.chunkSize = 0;            obj.reason = 0;            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = taskId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + " onMultiFilesReceived, hKey : " + hKey);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onMultiFilesReceived", hKey);        }
        public void onMultiFilesChunkSent(int handle, String toNode, String toChannel,
                String fileName, String taskId, int index, String fileType, long fileSize,
                long offset, long chunkSize) {            boolean bKeyExists = false;
            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey = taskId + "MultiFilesChunkSent";
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = toNode;
            obj.channel = toChannel;
            obj.fileName = fileName;            obj.exchangeId = taskId;            obj.index = index;            obj.fileType = fileType;            obj.fileSize = fileSize;
            obj.offset = offset;            obj.chunkSize = chunkSize;            obj.hash = "";
            obj.reason = 0;            obj.tempFilePath = "";            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                if (infoObj.mChordUnityFileTransferEventMap.containsKey(hKey)) {                    bKeyExists = true;                }                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + " onMultiFilesChunkSent, hKey : " + hKey);            if (!bKeyExists) {                UnityPlayer                        .UnitySendMessage(infoObj.UnityGameObject, "onMultiFilesChunkSent", hKey);
            }        }

        public void onMultiFilesSent(int handle, String toNode, String toChannel, String fileName,                String taskId, int index, String fileType) {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();
            obj.node = toNode;
            obj.channel = toChannel;
            obj.fileName = fileName;
            obj.exchangeId = taskId;
            obj.index = index;
            obj.fileType = fileType;
            obj.hash = "";
            obj.fileSize = 0;
            obj.offset = 0;
            obj.chunkSize = 0;
            obj.reason = 0;
            obj.tempFilePath = "";
            obj.totalCount = 1;
            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = taskId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);
            }            Log.v(TAG, TAGClass + " onMultiFilesSent, hKey : " + hKey);
            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onMultiFilesSent", hKey);
        }

        public void onMultiFilesFailed(int handle, String node, String channel, String fileName,
                String taskId, int index, int reason) {
            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;
            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();            obj.node = node;            obj.channel = channel;
            obj.exchangeId = taskId;
            obj.fileName = fileName;
            obj.index = index;
            obj.reason = reason;
            obj.hash = "";
            obj.fileType = "";
            obj.fileSize = 0;
            obj.offset = 0;
            obj.chunkSize = 0;
            obj.tempFilePath = "";            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = taskId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + " onMultiFilesFailed, hKey : " + hKey);
            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onMultiFilesFailed", hKey);        }

        public void onMultiFilesFinished(int handle, String node, String channel, String taskId,                int reason) {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;            ChordUnityFileTransferEvent obj = new ChordUnityFileTransferEvent();            obj.node = node;            obj.channel = channel;            obj.exchangeId = taskId;            obj.reason = reason;            obj.fileName = "";            obj.hash = "";            obj.fileType = "";            obj.fileSize = 0;            obj.offset = 0;            obj.chunkSize = 0;            obj.index = 0;            obj.tempFilePath = "";            obj.totalCount = 1;            synchronized (infoObj.mChordUnityFileTransferEventMap) {                ftHashKey++;                hKey = taskId + ftHashKey;                infoObj.mChordUnityFileTransferEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + " onMultiFilesFinished, hKey : " + hKey);
            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onMultiFilesFinished", hKey);        }        public void onUdpDataReceived(int handle, String fromNode, String fromChannel,                String payloadType, byte[][] payload, String sessionName) {            Log.v(TAG, TAGClass + "onUdpDataReceived()");            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;            ChordUnityEvent obj = new ChordUnityEvent();            obj.fromNode = fromNode;            obj.channel_reason = fromChannel;            obj.payloadType = payloadType;            obj.sessionNameUdp = sessionName;            obj.reqIdUdp = "";            obj.payLoad = payload;            if (obj.payLoad.length != payload.length) {                Log.v(TAG, TAGClass + "onnUdpDataReceived: payload not copied corrctly");            }            synchronized (infoObj.mChordUnityEventMap) {                hashKey++;                hKey = "fromNode" + hashKey;                infoObj.mChordUnityEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + "onnUdpDataReceived(), hKey : " + hKey + ", fromChannel : "                    + obj.fromNode + obj.channel_reason);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onUdpDataReceivedEvent", hKey);        }        public void onUdpDataDelivered(int handle, String toNode, String toChannel, String reqId) {            Log.v(TAG, TAGClass + "onUdpDataDelivered()");            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);            if (infoObj == null)                return;            String hKey;            ChordUnityEvent obj = new ChordUnityEvent();            obj.fromNode = toNode;            obj.channel_reason = toChannel;            obj.payloadType = "";            obj.sessionNameUdp = "";            obj.reqIdUdp = reqId;            synchronized (infoObj.mChordUnityEventMap) {                hashKey++;                hKey = "fromNode" + hashKey;                infoObj.mChordUnityEventMap.put(hKey, obj);            }            Log.v(TAG, TAGClass + "onUdpDataDelivered(), hKey : " + hKey + ", fromChannel : "                    + obj.fromNode + obj.channel_reason);            UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onUdpDataDeliveredEvent", hKey);        }
    };
    private UnityChannelListener mChannelListener = new UnityChannelListener();    public String[] getUnityNodeInfo(String hKey, int handle) {
        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null)            return new String[0];
        ChordUnityEvent obj = null;        synchronized (infoObj.mChordUnityEventMap) {            obj = infoObj.mChordUnityEventMap.remove(hKey);        }        if (obj == null) {

            Log.v(TAG, TAGClass + "getUnityNodeInfo failed for hasKey: " + hKey);            setLastError(ERROR_ILLEGAL_ARGUMENT);
            // return empty string Array as, UnityScript wont allow null return            return new String[0];
        }
        String[] values = new String[2];
        values[0] = obj.fromNode;
        values[1] = obj.channel_reason; // channel or reason
        Log.v(TAG, TAGClass + "getUnityNodeInfo(), hKey : " + hKey + ", name : " + obj.fromNode
                + " channel/reason:" + obj.channel_reason);        return values;
    }
    public String[] getUnityDataReceivedInfo(String hKey, int handle) {
        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null)            return new String[0];        int i = 0;
        ChordUnityEvent obj = null;        synchronized (infoObj.mChordUnityEventMap) {            obj = infoObj.mChordUnityEventMap.remove(hKey);        }        if (obj == null) {

            Log.v(TAG, TAGClass + "getUnityDataReceivedInfo failed for hasKey: " + hKey);            setLastError(ERROR_ILLEGAL_ARGUMENT);
            // return empty string Array as, UnityScript wont allow null return            return new String[0];
        }
        int len = obj.payLoad.length;
        Log.v(TAG, TAGClass + "getUnityDataReceivedInfo  for len: " + len);
        String[] payload = new String[len + 3];
        payload[i++] = obj.fromNode;
        payload[i++] = obj.channel_reason; // channel
        payload[i++] = obj.payloadType;
        for (int j = 0; j < len; j++) {

            payload[i++] = new String(obj.payLoad[j]);
            Log.v(TAG, TAGClass + "after copied::  " + payload[i - 1] + ":: before:"
                    + new String(obj.payLoad[j]));        }
        Log.v(TAG, TAGClass + "getUnityDataReceivedInfo(), hKey : " + hKey + ", name : "
                + obj.fromNode + " channel/ip:" + obj.channel_reason);        return payload;
    }    public String[] getUnityUdpDataReceivedInfo(String hKey, int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null)            return new String[0];        int i = 0;        ChordUnityEvent obj = null;        synchronized (infoObj.mChordUnityEventMap) {            obj = infoObj.mChordUnityEventMap.remove(hKey);        }        if (obj == null) {            Log.v(TAG, TAGClass + "getUnityDataReceivedInfo failed for hasKey: " + hKey);            setLastError(ERROR_ILLEGAL_ARGUMENT);            // return empty string Array as, UnityScript wont allow null return            return new String[0];        }        int len = obj.payLoad.length;        Log.v(TAG, TAGClass + "getUnityDataReceivedInfo  for len: " + len);        String[] payload = new String[len + 4];        payload[i++] = obj.fromNode;        payload[i++] = obj.channel_reason; // channel        payload[i++] = obj.payloadType;        payload[i++] = obj.sessionNameUdp;        for (int j = 0; j < len; j++) {            payload[i++] = new String(obj.payLoad[j]);            Log.v(TAG, TAGClass + "after copied::  " + payload[i - 1] + ":: before:"                    + new String(obj.payLoad[j]));        }        Log.v(TAG, TAGClass + "getUnityDataReceivedInfo(), hKey : " + hKey + ", name : "                + obj.fromNode + " channel/ip:" + obj.channel_reason);        return payload;    }    public String[] getUnityUdpDataDeliveredInfo(String hKey, int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null)            return new String[0];        int i = 0;        ChordUnityEvent obj = null;        synchronized (infoObj.mChordUnityEventMap) {            obj = infoObj.mChordUnityEventMap.remove(hKey);        }        if (obj == null) {            Log.v(TAG, TAGClass + "getUnityDataDeliveredInfo failed for hasKey: " + hKey);            setLastError(ERROR_ILLEGAL_ARGUMENT);            // return empty string Array as, UnityScript wont allow null return            return new String[0];        }        String[] payload = new String[3];        payload[i++] = obj.fromNode;        payload[i++] = obj.channel_reason; // channel        payload[i++] = obj.reqIdUdp;        Log.v(TAG, TAGClass + "getUnityDataDeliveredInfo(), hKey : " + hKey + ", name : "                + obj.fromNode + " channel/ip:" + obj.channel_reason);        return payload;    }
    public String[] getUnityFileTransferInfo(String hKey, int handle) {
        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null)            return new String[0];
        ChordUnityFileTransferEvent obj = null;        synchronized (infoObj.mChordUnityFileTransferEventMap) {            obj = infoObj.mChordUnityFileTransferEventMap.remove(hKey);        }        String values[] = new String[13];
        if (null != obj) {

            values[0] = obj.node;
            values[1] = obj.channel;
            values[2] = obj.exchangeId;
            values[3] = obj.fileName;
            values[4] = obj.fileType;
            values[5] = obj.hash;
            values[6] = String.valueOf(obj.fileSize);
            values[7] = String.valueOf(obj.offset);
            values[8] = String.valueOf(obj.chunkSize);
            values[9] = String.valueOf(obj.index);
            values[10] = String.valueOf(obj.reason);
            values[11] = String.valueOf(obj.totalCount);
            values[12] = obj.tempFilePath;
            obj = null;            return values;
        } else {
            Log.d(TAG, "mChordUnityFileTransferEventMap return null, key =" + hKey);
            return new String[0];        }    }
    // Release chord
    public void release() {        for (Integer key : handleInterfaceInfoMap.keySet()) {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.remove(key);            if (infoObj == null)                return;
            synchronized (infoObj.mChordUnityEventMap) {                infoObj.mChordUnityEventMap.clear();            }            synchronized (infoObj.mChordUnityFileTransferEventMap) {                infoObj.mChordUnityFileTransferEventMap.clear();            }            if (infoObj.mChord != null) {
                infoObj.mChord.stop();
                infoObj.mChord.close();
                infoObj.mChord = null;            }            infoObj = null;        }
        sChord = null;
        Log.d(TAG, "[UNREGISTER] Chord unregistered");

    }
    // Set a directory path to save receiving file temporarily
    public void setTempDirectory(String tmpDirPath, int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null)            return;        infoObj.tempDirPath = tmpDirPath;        SchordManager chordMgr = infoObj.mChord;        if (chordMgr != null) {            chordMgr.setTempDirectory(tmpDirPath);        }
    }    public String[] getNICList(int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null)            return new String[0];        SchordManager chordMgr = infoObj.mChord;        if (chordMgr != null) {            String NICList[] = chordMgr.getNICList();            return NICList;        }        return new String[0];    }

    public void browseFiles(int handle) {        selectedFilesHandle = handle;
        Log.d(TAG, TAGClass + "browseFiles() ");        // browseGameObject = unityGameObject;
        mUnityFileBrowseListener.onFileSelect();
    }    public void onBrowseResult(String[] filesList) {

        Log.d(TAG, TAGClass + "onBrowseResult() ");
        selectedFiles = filesList;        if (selectedFilesHandle != -1) {            unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(selectedFilesHandle);            if (infoObj != null)
                UnityPlayer.UnitySendMessage(infoObj.UnityGameObject, "onBrowseResult",                        selectedFiles[0]);        }
    }
    public void viewLicense() {        Log.d(TAG, TAGClass + "viewLicense() ");        mUnityViewLicenseListener.onRequestViewLicense();    }    // to get the files selected using browseFiles()
    public String[] getSelectedFiles() {
        return selectedFiles;
    }
    // Send file to the node on the channel.
    public String sendFile(String toChannel, String toNode, String fileType, String strFilePath,
            int timeoutMsec, int handle) {        Log.d(TAG, TAGClass + "sendFile() " + strFilePath + " in Channel" + toChannel + "toNode:"
                + toNode);        // Request the channel interface for the specific channel name.        SchordChannel channel = null;        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "sendFile() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return null;
        }
        String exchangeId = null;
        try {
            exchangeId = channel.sendFile(toNode, fileType, strFilePath,
            timeoutMsec);
        } catch (FileNotFoundException e) {
            Log.d(TAG, TAGClass + "sendFile : " + e);            setLastError(ERROR_FILE_NOT_FOUND);
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "sendFile : " + e);            setLastError(ERROR_ILLEGAL_STATE);        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "sendFile : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);        } catch (RuntimeException e) {
            Log.d(TAG, TAGClass + "sendFile : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
        }
        return exchangeId;
    }
    // Send multiple files to a specific node on the channel
    public String sendMultiFiles(String toChannel, String toNode, String fileType,
            String[] strFilePath, int timeout, int handle) throws FileNotFoundException {        Log.d(TAG, TAGClass + "multisendFile() ");        SchordChannel channel = null;        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "multisendFile() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return null;
        }
        for (int i = 0; i < strFilePath.length; i++) {

            Log.d(TAG, TAGClass + "multisendFile() file: " + i + " " + strFilePath[i]);
        }
        String trId = null;
        List<String> listFilePath = Arrays.asList(strFilePath);
        try {
            trId = channel.sendMultiFiles(toNode, fileType, listFilePath, timeout);
        } catch (FileNotFoundException e) {            Log.d(TAG, TAGClass + "multisendFile : " + e);            setLastError(ERROR_FILE_NOT_FOUND);        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "multisendFile : " + e);            setLastError(ERROR_ILLEGAL_STATE);        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "multisendFile : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);        } catch (RuntimeException e) {
            Log.d(TAG, TAGClass + "multisendFile : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
        }
        return trId;
    }
    // Accept to receive file.
    public boolean acceptFile(String fromChannel, String exchangeId, int chunkTimeoutMsec,
            int chunkRetries, long chunkSize, int handle) {        Log.d(TAG, TAGClass + "acceptFile()");
        // Request the channel interface for the specific channel name.        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }
        SchordChannel channel = null;        channel = chordMgr.getJoinedChannel(fromChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "acceptFile() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        try {
            channel.acceptFile(exchangeId, chunkTimeoutMsec, chunkRetries, chunkSize);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "acceptFile : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "acceptFile : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "acceptFile : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
            return false;
        }
    }
    // Cancel file transfer while it is in progress.
    public boolean cancelFile(String toChannel, String exchangeId, int handle) {
        Log.d(TAG, TAGClass + "cancelFile()");
        // Request the channel interface for the specific channel name.        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }
        SchordChannel channel = null;        channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "cancelFile() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        try {
            channel.cancelFile(exchangeId);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "cancelFile : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "cancelFile : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "cancelFile : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
            return false;
        }
    }
    // Reject to receive file
    public boolean rejectFile(String toChannel, String exchangeId, int handle) {
        Log.d(TAG, TAGClass + "rejectFile()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }
        // Request the channel interface for the specific channel name.
        SchordChannel channel = null;        channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "cancelFile() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        try {
            channel.rejectFile(exchangeId);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "rejectFile : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "rejectFile : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "rejectFile : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
            return false;
        }
    }
    // Accept to receive files.
    public boolean acceptMultiFiles(String fromChannel, String taskId, int chunkTimeout,
            int chunkRetries, long chunkSize, int handle) {        Log.d(TAG, TAGClass + "acceptMultiFiles()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }
        // Request the channel interface for the specific channel name.        SchordChannel channel = null;        channel = chordMgr.getJoinedChannel(fromChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "acceptMultiFiles() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        try {
            channel.acceptMultiFiles(taskId, chunkTimeout, chunkRetries, chunkSize);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "acceptMultiFiles : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "acceptMultiFiles : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "acceptMultiFiles : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
            return false;
        }
    }
    // Cancel file transfer while it is in progress.
    public boolean cancelMultiFiles(String toChannel, String taskId, int handle) {
        Log.d(TAG, TAGClass + "cancelMultiFiles()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }
        // Request the channel interface for the specific channel name.
        SchordChannel channel = null;        channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "cancelMultiFiles() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;
        }
        try {
            channel.cancelMultiFiles(taskId);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "cancelMultiFiles : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "cancelMultiFiles : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "cancelMultiFiles : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);            return false;
        }
    }
    // Reject to receive file.
    public boolean rejectMultiFiles(String toChannel, String taskId, int handle) {
        Log.d(TAG, TAGClass + "rejectMultiFiles()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }
        // Request the channel interface for the specific channel name.
        SchordChannel channel = null;        channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "rejectMultiFiles() : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        try {
            channel.rejectMultiFiles(taskId);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "rejectMultiFiles : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "rejectMultiFiles : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "rejectMultiFiles : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
            return false;
        }
    }
    // Requests for nodes on the channel.
    public String[] getJoinedNodeList(String channelName, int handle) {
        int i = 0;        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return new String[0];        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            // return empty string Array as, UnityScript wont allow null return            return new String[0];        }
        Log.d(TAG, TAGClass + "getJoinedNodeList()");
        // Request the channel interface for the specific channel name.
        SchordChannel channel = chordMgr.getJoinedChannel(channelName);
        if (null == channel) {
            Log.e(TAG, TAGClass + "getJoinedNodeList() : invalid channel instance-" + channelName);            setLastError(ERROR_ILLEGAL_ARGUMENT);
            // return empty string Array as, UnityScript wont allow null return            return new String[0];
        }        List<String> nodeList = channel.getJoinedNodeList();        if (null == nodeList) {            // return empty string Array as, UnityScript wont allow null return            return new String[0];        }
        int len = nodeList.size();
        String NodeValues[] = new String[len];
        for (String node : nodeList) {

            Log.d(TAG, TAGClass + "getJoinedNodeList() node ::  " + node);
            NodeValues[i++] = node;
        }
        return NodeValues;
    }
    // Send data message to the node.
    public boolean sendData(String[] object) {
        boolean error = false;
        int len = object.length;
        if (len < 4) // 4 is minimum no of arguments
        {
            Log.e(TAG, TAGClass + "sendData : wrong inputs : ");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        String toChannel = object[0];
        String type = object[1];
        String nodeName = object[2];        String ifc = object[3];        int handle = Integer.parseInt(ifc);
        Log.e(TAG, TAGClass + "sendData : inputs: " + toChannel + " :: " + type + " :: " + nodeName);
        byte[][] data = new byte[len - 4][];
        for (int i = 4; i < len; i++) {
            data[i - 4] = object[i].getBytes();
            // Log.d( TAG, TAGClass + "sendData : " + data[i-3].toString() );
        }        // call sendData down
        error = sendData(toChannel, type, nodeName, data, handle);
        return error;
    }
    private boolean sendData(String toChannel, String type, String nodeName, byte[][] data,            int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.v(TAG, "sendData : mChord IS NULL  !!");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return false;
        }
        // Request the channel interface for the specific channel name.
        SchordChannel channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "sendData : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        if (nodeName == null) {
            Log.v(TAG, "sendData : NODE Name IS NULL !!");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        try {
            channel.sendData(nodeName, type, data);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "sendData : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "sendData : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "sendData : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
            return false;
        }
    }
    // Send data message to the all nodes on the channel
    public boolean sendDataToAll(String[] object) {

        int len = object.length;
        if (len < 4) // 3 is minimum no of arguments
        {
            Log.e(TAG, TAGClass + "sendDataToAll : wrong inputs : ");            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        String type = object[0];
        String nodeName = object[1];        String ifc = object[2];        int handle = Integer.parseInt(ifc);
        Log.e(TAG, TAGClass + "sendDataToAll : inputs: " + type + " :: " + nodeName);
        byte[][] data = new byte[len - 3][];
        for (int i = 3; i < len; i++) {
            data[i - 3] = object[i].getBytes();
            // Log.d( TAG, TAGClass + "sendDataToAll : " + data[i-2].toString()
            // );        }
        // call sendDataToAll down
        boolean error = sendDataToAll(type, nodeName, data, handle);
        return error;
    }
    public boolean sendDataToAll(String toChannel, String messageType, byte[][] buf, int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.v(TAG, "sendDataToAll : mChord IS NULL  !!");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return false;
        }
        // Request the channel interface for the specific channel name.
        SchordChannel channel = chordMgr.getJoinedChannel(toChannel);
        if (null == channel) {
            Log.e(TAG, TAGClass + "sendDataToAll : invalid channel instance");
            return false;
        }
        Log.v(TAG, TAGClass + "sendDataToAll : " + new String(buf[0]));
        String type = null;
        if (null == messageType)
            type = CHORD_UNITY_MESSAGE_TYPE;
        else
            type = messageType;
        try {
            channel.sendDataToAll(type, buf);
            return true;
        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "sendDataToAll : " + e);            setLastError(ERROR_ILLEGAL_STATE);            return false;        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "sendDataToAll : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        } catch (RuntimeException e) {
            Log.e(TAG, TAGClass + "sendDataToAll : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);
            return false;
        }
    }    public String sendUdpData(String[] object) {        int len = object.length;        if (len < 7) // 3 is minimum no of arguments        {            Log.e(TAG, TAGClass + "sendDataToAll : wrong inputs : ");            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        String toChannel = object[0];        String toNode = object[1];        String mod = object[2];        int reliableTime = Integer.parseInt(mod);        String flag = object[3];        boolean bRespFlag = Boolean.valueOf(flag);        String payloadType = object[4];        String sessionName = object[5];        String ifc = object[6];        int handle = Integer.parseInt(ifc);        Log.e(TAG, TAGClass + "udpsendData : inputs: " + sessionName + " :: " + toNode + " :: "                + reliableTime + " :: " + bRespFlag + " flag ::" + flag);        byte[][] data = new byte[len - 7][];        for (int i = 7; i < len; i++) {            data[i - 7] = object[i].getBytes();            // Log.d( TAG, TAGClass + "udpSendDataToAll : " +            // data[i-2].toString() );        }        // call udpSendDataToAll down        String reqId = sendUdpData(toChannel, toNode, reliableTime, bRespFlag, payloadType, data,                sessionName, handle);        return reqId;    }    private String sendUdpData(String toChannel, String toNode, int reliableTime,            boolean bRespFlag, String payloadType, byte[][] payload, String sessionName, int handle) {        String reqId = null;        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            Log.v(TAG, "sendDataToAll : mChord IS NULL  !!");            setLastError(ERROR_CHORD_UNINITIALIZED);        }        // Request the channel interface for the specific channel name.        SchordChannel channel = chordMgr.getJoinedChannel(toChannel);        if (null == channel) {            Log.e(TAG, TAGClass + "udpSendData : invalid channel instance");        }        Log.v(TAG, TAGClass + "udpSendData : " + new String(payload[0]));        String type = null;        if (null == payloadType)            type = CHORD_UNITY_MESSAGE_TYPE;        else            type = payloadType;        try {            reqId = channel                    .sendUdpData(toNode, reliableTime, bRespFlag, type, payload, sessionName);            return reqId;        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "udpSendData : " + e);            setLastError(ERROR_ILLEGAL_STATE);        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "udpSendData : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);        } catch (RuntimeException e) {            Log.e(TAG, TAGClass + "sendData : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);        }        return reqId;    }    public String sendUdpDataToAll(String[] object) {        int len = object.length;        if (len < 6) // 3 is minimum no of arguments        {            Log.e(TAG, TAGClass + "udpSendDataToAll : wrong inputs : ");            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        String toChannel = object[0];        String mod = object[1];        int reliableTime = Integer.parseInt(mod);        String flag = object[2];        boolean bRespFlag = Boolean.valueOf(flag);        String payloadType = object[3];        String sessionName = object[4];        String ifc = object[5];        int handle = Integer.parseInt(ifc);        Log.e(TAG, TAGClass + "udpSendDataToAll : inputs: " + payloadType + " :: " + toChannel);        byte[][] data = new byte[len - 6][];        for (int i = 6; i < len; i++) {            data[i - 6] = object[i].getBytes();            // Log.d( TAG, TAGClass + "udpSendDataToAll : " +            // data[i-2].toString() );        }        // call sendDataToAll down        String reqId = sendUdpDataToAll(toChannel, reliableTime, bRespFlag, payloadType, data,                sessionName, handle);        return reqId;    }    private String sendUdpDataToAll(String toChannel, int reliableTime, boolean bRespFlag,            String payloadType, byte[][] payload, String sessionName, int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return null;        }        SchordManager chordMgr = infoObj.mChord;        String reqId = null;        if (chordMgr == null) {            Log.v(TAG, "udpSendDataToAll : mChord IS NULL  !!");            setLastError(ERROR_CHORD_UNINITIALIZED);        }        // Request the channel interface for the specific channel name.        SchordChannel channel = chordMgr.getJoinedChannel(toChannel);        if (null == channel) {            Log.e(TAG, TAGClass + "udpSendDataToAll : invalid channel instance");        }        Log.v(TAG, TAGClass + "udpSendDataToAll : " + new String(payload[0]));        String type = null;        if (null == payloadType)            type = CHORD_UNITY_MESSAGE_TYPE;        else            type = payloadType;        try {            reqId = channel.sendUdpDataToAll(reliableTime, bRespFlag, type, payload, sessionName);            return reqId;        } catch (IllegalStateException e) {            Log.d(TAG, TAGClass + "sendDataToAll : " + e);            setLastError(ERROR_ILLEGAL_STATE);        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + "sendDataToAll : " + e);            setLastError(ERROR_ILLEGAL_ARGUMENT);        } catch (RuntimeException e) {            Log.e(TAG, TAGClass + "sendDataToAll : " + e);            setLastError(ERROR_RUNTIME_EXCEPTION);        }        return reqId;    }
    // Set a keep-alive timeout
    public void setNodeKeepAliveTimeout(int timeout, int handle) {
        Log.d(TAG, TAGClass + "setNodeKeepAliveTimeout()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.e(TAG, TAGClass + "setNodeKeepAliveTimeout() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return;
        }
        // @param timeoutMsec Timeout with millisecond.
        chordMgr.setNodeKeepAliveTimeout(timeout);
    }
    // Set whether smart discovery is enabled
    public void setSmartDiscoveryEnabled(boolean enabled, int handle) {        Log.d(TAG, TAGClass + "setSmartDiscoveryEnabled()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.e(TAG, TAGClass + "setSmartDiscoveryEnabled() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return;
        }
        chordMgr.setSmartDiscoveryEnabled(enabled);
    }
    // Check whether smart discovery is enabled
    public boolean isSmartDiscoveryEnabled(int handle) {        Log.d(TAG, TAGClass + "isSmartDiscoveryEnabled()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.e(TAG, TAGClass + "isSmartDiscoveryEnabled() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return false;
        }
        return chordMgr.isSmartDiscoveryEnabled();
    }    // Reset period for sending nodes
    public void resetSmartDiscoveryPeriod(int handle) {        Log.d(TAG, TAGClass + "resetSmartDiscoveryPeriod()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }
        chordMgr.resetSmartDiscoveryPeriod();
    }
    // Get an IPv4 address that the node has.
    public String getNodeIpAddress(String channelName, String nodeName, int handle) {        Log.d(TAG, TAGClass + "getNodeIpAddress() channelName : " + channelName + ", nodeName : "        + nodeName);        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return "";        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return "";        }
        SchordChannel channel = chordMgr.getJoinedChannel(channelName);
        if (null == channel)            return "";        String ipAddr = null;        try {            ipAddr = channel.getNodeIpAddress(nodeName);        } catch (IllegalArgumentException e) {            Log.d(TAG, TAGClass + e.getMessage());            setLastError(ERROR_ILLEGAL_ARGUMENT);        }
        return ipAddr;
    }
    // Get a list of available network interface types.
    public String[] getAvailableInterfaceTypes(int handle) {        Log.d(TAG, TAGClass + "getAvailableInterfaceTypes()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return new String[0];        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return new String[0];        }
        int i = 0;
        List<Integer> InterfacesList = chordMgr.getAvailableInterfaceTypes();
        if (InterfacesList.size() == 0) {

            Log.d(TAG, TAGClass + "getAvailableInterfaceTypes() : No Interfaces Available ");
            // return empty string Array, as UnityScript wont allow null return            return new String[0];        }
        int len = InterfacesList.size();
        String Interfacevalues[] = new String[len];
        for (int interfaceValue : InterfacesList) {

            Log.d(TAG, TAGClass + "getAvailableInterfaceTypes() Interface type ::  "
                    + interfaceValue);            Interfacevalues[i++] = String.valueOf(interfaceValue);
        }
        return Interfacevalues;
    }
    // Request for joined channel Names.
    public String[] getJoinedChannelList(int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return new String[0];        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return new String[0];        }
        int i = 0;
        Log.d(TAG, TAGClass + "getJoinedChannelList()");
        List<SchordChannel> channelList = chordMgr.getJoinedChannelList();
        if (channelList.size() == 0) {

            Log.d(TAG, TAGClass + "getJoinedChannelList() : No Channel Available ");
            // return empty string Array, as UnityScript wont allow null return            return new String[0];
        }
        int len = channelList.size();
        String ChannelNames[] = new String[len];
        for (SchordChannel channel : channelList) {
            Log.d(TAG, TAGClass + "getJoinedChannelList() channel type ::  " + channel);
            ChannelNames[i++] = channel.getName();
        }        return ChannelNames;
    }
    // Get own node name
    public String getName(int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return "";        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.e(TAG, TAGClass + "getName() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return "";
        }
        return chordMgr.getName();
    }
    // Get own IPv4 address.
    public String getIp(int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return "";        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.e(TAG, TAGClass + "getIp() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return "";
        }
        return chordMgr.getIp();
    }
    // Join a desired channel with a given listener.
    public boolean joinChannel(String channelName, int handle) {        Log.d(TAG, TAGClass + "joinChannel()" + channelName);        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;
        if (channelName == null || channelName.equals("")) {            setLastError(ERROR_ILLEGAL_ARGUMENT);
            return false;
        }
        if (chordMgr == null) {
            Log.e(TAG, TAGClass + "joinChannel() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return false;
        }        if (null == infoObj.cListner) {            derivedChannelListener cListener = new derivedChannelListener(handle, mChannelListener);            infoObj.cListner = cListener;        }
        try {
            chordMgr.joinChannel(channelName, infoObj.cListner);
            return true;
        } catch (IllegalArgumentException e) {
            e.printStackTrace();            setLastError(ERROR_ILLEGAL_ARGUMENT);
        } catch (RuntimeException e) {
            e.printStackTrace();            setLastError(ERROR_RUNTIME_EXCEPTION);
        }
        return false;
    }
    // Leave a given channel.
    public void leaveChannel(String channelName, int handle) {        Log.d(TAG, TAGClass + "leaveChannel()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;
        if (chordMgr == null) {
            Log.e(TAG, TAGClass + "leaveChannel() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);
            return;
        }
        // @param channelName Channel name        try {            chordMgr.leaveChannel(channelName);        } catch (IllegalArgumentException e) {            e.printStackTrace();            setLastError(ERROR_ILLEGAL_ARGUMENT);        } catch (RuntimeException e) {            e.printStackTrace();            setLastError(ERROR_RUNTIME_EXCEPTION);        }
    }
    // Check whether this channel is secured or not
    public boolean isSecureChannel(String channelName, int handle) {        Log.d(TAG, TAGClass + "isSecureChannel()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        SchordManager chordMgr = infoObj.mChord;
        SchordChannel channel = null;        if (chordMgr == null) {            Log.e(TAG, TAGClass + "isSecureChannel() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);            return false;        }        // Request the channel interface for the specific channel name.        channel = chordMgr.getJoinedChannel(channelName);        if (null == channel) {            Log.e(TAG, TAGClass + "isSecureChannel : invalid channel instance");            setLastError(ERROR_ILLEGAL_ARGUMENT);            return false;        }        return channel.isSecureChannel();
    }
    // Stop chord
    public void stop(int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }
        Log.d(TAG, TAGClass + "stop()");
        if (chordMgr != null) {
            chordMgr.stop();
        }
    }
    // Release resources to be used
    public void close(int handle) {        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        mUnityCloseListener.onCloseApp();
        if (chordMgr != null) {
            chordMgr.close();
            chordMgr = null;
        }
    }
    // Get the version of Chord, relative to other versions
    public int getVersionCode() {

        return sChord.getVersionCode();
    }
    // Get the version name of Chord
    public String getVersionName() {

        return sChord.getVersionName();
    }
    // Set the number of files to send simultaneously
    public void setSendMultiFilesLimitCount(int count, int handle) {
        Log.d(TAG, TAGClass + "setSendMultiFilesLimitCount()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            Log.e(TAG, TAGClass + "setSendMultiFilesLimitCount() , chord is null");            setLastError(ERROR_CHORD_UNINITIALIZED);            return;        }        chordMgr.setSendMultiFilesLimitCount(count);
    }
    // Sets whether to use secured channel
    public void setSecureModeEnabled(boolean enabled, int handle) {
        Log.d(TAG, TAGClass + "setSecureModeEnabled()");        unityINterfaceInfo infoObj = handleInterfaceInfoMap.get(handle);        if (infoObj == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        SchordManager chordMgr = infoObj.mChord;        if (chordMgr == null) {            setLastError(ERROR_ILLEGAL_ARGUMENT);            return;        }        chordMgr.setSecureModeEnabled(enabled);
    }
    public IChordUnityCloseListener getmUnityCloseListener() {        return mUnityCloseListener;    }    public void setmUnityCloseListener(IChordUnityCloseListener mUnityCloseListener) {        this.mUnityCloseListener = mUnityCloseListener;    }    private static final int MSG_SENDDATA = 10000;
    Handler mHandler = new Handler(new SendDataHandlerCallback());
    class SendDataHandlerCallback implements Handler.Callback {        @Override
        public boolean handleMessage(Message msg) {

            if (msg.what == MSG_SENDDATA) {
                byte[][] payload = new byte[1][];
                payload[0] = "TEST".getBytes();
                try {
                    // sendDataToAll(PUBLIC_CHANNEL, CHORD_UNITY_MESSAGE_TYPE,
                    // payload);                } catch (RuntimeException e) {
                    Log.e(TAG, TAGClass + "sendDataToAll : " + e);                    setLastError(ERROR_RUNTIME_EXCEPTION);
                                        return false;                }
                                return true;            }
                        return false;        }
    };    private static void setLastError(int errorCode) {        lastError = errorCode;    }    public int getLastError() {        Log.e(TAG, TAGClass + "getLastError : " + lastError);        return lastError;    }
}
