/*============================== FILE HEADER =================================
*
*           Copyright (c) 2013, Samsung Research Institute India , Bangalore
*                      Samsung Confidential Proprietary
*                            All Rights Reserved
*
==============================================================================
*                               Module Name : C# Chord Unity Interface
==============================================================================
* File                    : CLog.cs
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



/**
 * A wrapper class for Debug logging, with debug level support. if chordDebugLevel > 0, Logs are enabled, else disabled.
 */

public class CLog
{

    // by default enabled

    private static int chordDebugLevel = 1;


    /**
     * Sets the debug level. Currently debug level > 0 means message will be logged.
     *
     * @param userLevel New debug level.
     */

    public static void setDebugLevel(int userLevel)
    {

        chordDebugLevel = userLevel;

    }


    /**
     * Returns the current debug level ( if >0, Logs are enabled, else disabled).
     *
     * @return Current debug level chordDebugLevel.
     */

    public static int getDebugLevel()
    {

        return chordDebugLevel;

    }


    /**
     * Wrapper around Debug.Log
     *
     * @param msg Message to be logged if debug level allows it.
     */

    public static void Log(String msg)
    {

        if (chordDebugLevel > 0)

            Debug.Log(msg);

    }


    /**
     * Wrapper around Debug.LogWarning
     *
     * @param msg Message to be logged as warning  if debug level allows it.
     */

    public static void LogWarning(String msg)
    {

        if (chordDebugLevel > 0)

            Debug.LogWarning(msg);

    }


    /**
     * Wrapper around Debug.LogError
     *
     * @param msg Message to be logged as error if debug level allows it.
     */

    public static void LogError(String msg)
    {

        if (chordDebugLevel > 0)

            Debug.LogError(msg);

    }


    /**
     * Wrapper around Debug.LogException
     *
     * @param msg Message to be logged as exception if debug level allows it.
     */

    public static void LogException(Exception exception)
    {

        if (chordDebugLevel > 0)

            Debug.LogException(exception);

    }

}
