/*============================== FILE HEADER =================================
 *
 *           Copyright (c) 2013, Samsung Research Institute India , Bangalore
 *                      Samsung Confidential Proprietary
 *                            All Rights Reserved
 *
==============================================================================
 *                               Module Name : Java Chord Unity Interface
==============================================================================
 * File                    : ChordUnityActivity.java
 *
 * Author(s)           : Ganesh VR , Dhanya R
 *
 * Description         : Java Interface Implementation  between  the C# ChordUnityInterface & Java Chord Service layer.
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

package com.samsung.android.sdk.chordUnity;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.InputStream;
import java.util.ArrayList;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.util.Log;

import com.samsung.android.sdk.chordUnity.ChordUnityService.ChordServiceBinder;import com.samsung.android.sdk.chordUnity.ChordUnityService.IChordUnityCloseListener;
import com.samsung.android.sdk.chordUnity.ChordUnityService.IChordUnityViewLicenseListener;

import com.samsung.android.sdk.chordUnity.ChordUnityService.IChordUnityFileBrowseListener;

import com.unity3d.player.UnityPlayerActivity;

public class ChordUnityActivity extends UnityPlayerActivity implements
        IChordUnityFileBrowseListener, IChordUnityCloseListener, IChordUnityViewLicenseListener {

    private static final String TAG = "[Chord][ChordUnity]";

    private static final String TAGClass = "ChordUnityActivity :";

    public static final String MYFILES_ACTION = "com.sec.android.app.myfiles.PICK_DATA_MULTIPLE";

    private ChordUnityActivity chordUnityActivity = this;

    public static final String APP_EXTERNAL_STORAGE_PATH = Environment
            .getExternalStorageDirectory().getPath();

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);

        Log.v(TAG, TAGClass + "onCreate");

        startService();

        bindChordService();

        Log.v(TAG, TAGClass + "onCreate - exit");

    }

    @Override
    protected void onResume() {

        super.onResume();

        Log.v(TAG, TAGClass + "onResume");

    }

    @Override
    protected void onDestroy() {        super.onDestroy();

        Log.v(TAG, TAGClass + "onDestroy");

    }

    // **********************************************************************

    // Using Service

    // **********************************************************************

    private ChordUnityService mChordUnityService = null;

    private ServiceConnection mConnection = new ServiceConnection() {

        @Override
        public void onServiceConnected(ComponentName name, IBinder service) {

            Log.d(TAG, TAGClass + "onServiceConnected()");

            ChordServiceBinder binder = (ChordServiceBinder) service;

            mChordUnityService = binder.getService();

            try {

                mChordUnityService.initialize();

                // listener

                mChordUnityService.setChordUnityFileBrowseListener(chordUnityActivity);                mChordUnityService.setmUnityCloseListener(chordUnityActivity);
                mChordUnityService.setChordUnityViewLicenseListener(chordUnityActivity);

            } catch (Exception e) {

                e.printStackTrace();

            }

        }

        @Override
        public void onServiceDisconnected(ComponentName name) {

            Log.i(TAG, TAGClass + "onServiceDisconnected()");

            mChordUnityService = null;
        }

    };
    public void bindChordService() {

        Log.i(TAG, TAGClass + "bindChordService()");

        if (mChordUnityService == null) {

            Intent intent = new Intent(
                    "com.samsung.android.sdk.chordUnity.ChordUnityService.SERVICE_BIND");

            bindService(intent, mConnection, Context.BIND_AUTO_CREATE);

        }

    }

    private void unbindChordService() {

        Log.i(TAG, TAGClass + "unbindChordService()");

        if (null != mChordUnityService) {

            unbindService(mConnection);

        }

        mChordUnityService = null;

    }

    private void startService() {

        Log.i(TAG, TAGClass + "startService()");

        Intent intent = new Intent(
                "com.samsung.android.sdk.chordUnity.ChordUnityService.SERVICE_START");

        startService(intent);

    }

    private void stopService() {

        Log.i(TAG, TAGClass + "stopService()");

        Intent intent = new Intent(
                "com.samsung.android.sdk.chordUnity.ChordUnityService.SERVICE_STOP");

        stopService(intent);

    }

    @Override
    public void onFileSelect() {

        Intent i2 = new Intent();

        i2.setAction(MYFILES_ACTION);

        i2.putExtra("FOLDERPATH", APP_EXTERNAL_STORAGE_PATH);

        startActivityForResult(i2, 100);

    }

    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {

        if (resultCode == Activity.RESULT_OK) {

            String[] fileList = null;

            if (intent != null) {

                ArrayList<String> arrUploadListData = new ArrayList<String>();

                Bundle bundle = intent.getExtras();

                if (bundle == null)
                    return;
                String[] receivedFiles = (String[]) bundle.get("FILE");

                for (int i = 0; i < receivedFiles.length; i++) {

                    File f = new File(receivedFiles[i].toString());

                    arrUploadListData.add(f.getAbsolutePath());

                }
                fileList = new String[arrUploadListData.size()];

                arrUploadListData.toArray(fileList);

                mChordUnityService.onBrowseResult(fileList);

            }
        }

    }    @Override    public void onCloseApp() {        Log.i(TAG, TAGClass + "onCloseApp()");        unbindChordService();        stopService();        finish();    }

    @Override
    public void onRequestViewLicense() {
        mHandler.sendEmptyMessage(VIEW_LICENSE);        
    }
    
    final int VIEW_LICENSE = 10001;
    
    private final Handler mHandler = new Handler(new IncomingHandlerCallback());
    class IncomingHandlerCallback implements Handler.Callback {
        public boolean handleMessage(Message msg) {
            if(msg.what == VIEW_LICENSE) {
                String license;
                InputStream is = getResources().openRawResource(R.raw.license);
                ByteArrayOutputStream bs = new ByteArrayOutputStream();

                try {
                    int i = is.read();
                    while (i != -1) {
                        bs.write(i);
                        i = is.read();
                    }

                } catch (Exception e) {

                } finally {
                    try {
                        is.close();
                    } catch (Exception e) {

                    }
                }

                license = bs.toString();

                AlertDialog alertDialog = new AlertDialog.Builder(ChordUnityActivity.this).setTitle("License")
                        .setMessage(license).create();
                alertDialog.show();
                return true;
            }
            return false;
        }
    };

}
