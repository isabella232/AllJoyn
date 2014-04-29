 /*============================== FILE HEADER =================================
*
*           Copyright (c) 2013, Samsung Research Institute India , Bangalore
*                      Samsung Confidential Proprietary
*                            All Rights Reserved
*
==============================================================================
*                               Module Name : C# Chord Unity Interface
==============================================================================
* File                    : ChordUnityConstants.cs
*
* Author(s)           : Ganesh VR , Dhanya R
*
* Description         : C# Interface Implementation  between  the C# Application & Java Chord Unity Service Interface. 
*				   This file contains Chord Unity related Constants & errors values.
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
 
public class ChordUnityConstants

{

    // Interface types

    public const int INTERFACE_TYPE_WIFI = 0;

    public const int INTERFACE_TYPE_WIFIP2P = 1;

    public const int INTERFACE_TYPE_WIFIAP = 2;


    // network status constants

    public const int STARTED_BY_USER = 0;

    public const int STARTED_BY_RECONNECTION = 1;

    public const int ERROR_START_FAIL = 1000;

    public const int ERROR_UNEXPECTED_STOP = 1001;

    public const int STOPPED_BY_USER = 1002;

    public const int NETWORK_DISCONNECTED = 1003;



    // file Failure errorCodes

    public const int ERROR_FILE_SEND_FAILED = 2000;

    public const int ERROR_FILE_REJECTED = 2001;

    public const int ERROR_FILE_CANCELED = 2002;

    public const int ERROR_FILE_CREATE_FAILED = 2003;

    public const int ERROR_FILE_NO_RESOURCE = 2004;

    public const int ERROR_FILE_TIMEOUT = 2005;

    public const int ERROR_NONE = 2006;



    // prefix a channel name with #, for secure channel creation

    public const string SECURE_PREFIX = "#";
	
	
	//start result
	
	public const int START_SUCCESS = 0;
	
	//   Error codes
	
	public const int ERROR_ILLEGAL_STATE = 3001;
	
	public const int ERROR_ILLEGAL_ARGUMENT = 3002;
	
	public const int ERROR_RUNTIME_EXCEPTION = 3003;
	
	public const int ERROR_FILE_NOT_FOUND = 3004;
	
	public const int ERROR_INVALID_INTERFACE = 3005;
	
	public const int ERROR_SSDK_UNSUPPORTED = 3006;
	
	public const int ERROR_CHORD_UNINITIALIZED = 3007;
	
	// udp send data modes
	/**
     * Mode - UnReliable Mode (Reliability of data transfer is not guaranteed_  
     */
     public const int MODE_UNRELIABLE = 0;
    /**
     * Mode - SemiReliable Mode (Reliability of data transfer is partially guaranteed until TTL time )
     */
     public const int MODE_SEMIRELIABLE = 10;
    /**
     * Mode - Reliable Mode (Reliability of data transfer is  guaranteed)     
     */
     public const int MODE_RELIABLE = -1;

}