 /*============================== FILE HEADER =================================**           Copyright (c) 2013, Samsung Research Institute India , Bangalore*                      Samsung Confidential Proprietary*                            All Rights Reserved*==============================================================================*                               Module Name : C# Chord Unity Interface==============================================================================* File                    : ChordUnityStatusListener.cs** Author(s)           : Ganesh VR , Dhanya R** Description         : C# Interface Implementation  between  the C# Application & Java Chord Unity Service Interface. 				   *==============================================================================*                            Modification History*-----------------------------------------------------------------------------*    Date       |       Name                    |        Modification*-----------------------------------------------------------------------------*                  |                               		|*  24-07-13  | Ganesh VR , Dhanya R        | Newly Added*                  |                               		|============================================================================*/


// ***************************************************
// ChordUnityStatusListener
// ***************************************************
/** * This interface defines a listener for chord unity service related to chord status events. * Listens for the connection status of the node by start & stop events. */


public interface ChordUnityStatusListener
{
    /**         * Called when the Chord has started successfully. Also it can be invoked upon a reconnection.         * After this method is called, channel operations can be performed.         *          * @param nodeName The own node name         * @param reason The reason for start could be one of         *    {STARTED_BY_USER} Started by calling start.         *    {STARTED_BY_RECONNECTION} Started by reconnection         */

    void onStarted(string nodeName, int reason);

    /**         * Called when the Chord has stopped.         *          * @param reason The reason for stop could be one of         *   {STOPPED_BY_USER}         *   {NETWORK_DISCONNECTED},         *   {ERROR_UNEXPECTED_STOP}         *   {ERROR_START_FAIL}         */

    void onStopped(int reason);

}

