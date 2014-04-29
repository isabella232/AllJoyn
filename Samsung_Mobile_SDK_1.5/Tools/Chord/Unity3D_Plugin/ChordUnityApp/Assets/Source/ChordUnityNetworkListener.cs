 /*============================== FILE HEADER =================================**           Copyright (c) 2013, Samsung Research Institute India , Bangalore*                      Samsung Confidential Proprietary*                            All Rights Reserved*==============================================================================*                               Module Name : C# Chord Unity Interface==============================================================================* File                    : ChordUnityNetworkListener.cs** Author(s)           : Ganesh VR , Dhanya R** Description         : C# Interface Implementation  between  the C# Application & Java Chord Unity Service Interface. 				   *==============================================================================*                            Modification History*-----------------------------------------------------------------------------*    Date       |       Name                    |        Modification*-----------------------------------------------------------------------------*                  |                               		|*  24-07-13  | Ganesh VR , Dhanya R        | Newly Added*                  |                               		|============================================================================*/

// ***************************************************
// ChordUnityNetworkListener
// ***************************************************   
/** * This interface defines a listener for network status change events. * Can be used for performing operations that depend on the network connectivity. * For instance, the application UI can be updated to reflect the changes in connectivity. *   * Listens for the status of the network for different interfaces, regardless of whether Chord starts or not. */

public interface ChordUnityNetworkListener
{

    /**         * It is called a when specific interface is connected         * @param interfaceType interfaceType connected         *      { INTERFACE_TYPE_WIFI}         *      { INTERFACE_TYPE_WIFI_AP}         *      { INTERFACE_TYPE_WIFI_P2P}         */
    void onConnected(int interfaceType);

    /**         * It is called when a specific interface is disconnected         * @param interfaceType interfaceType disconnected         *      { INTERFACE_TYPE_WIFI}         *      { INTERFACE_TYPE_WIFI_AP}         *      { INTERFACE_TYPE_WIFI_P2P}         */
    void onDisconnected(int interfaceType);

}
