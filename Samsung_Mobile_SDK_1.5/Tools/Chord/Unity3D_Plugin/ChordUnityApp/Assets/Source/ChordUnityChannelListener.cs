 /*============================== FILE HEADER =================================
*
*           Copyright (c) 2013, Samsung Research Institute India , Bangalore
*                      Samsung Confidential Proprietary
*                            All Rights Reserved
*
==============================================================================
*                               Module Name : C# Chord Unity Interface
==============================================================================
* File                    : ChordUnityChannelListener.cs
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



/**********************************************************************
 * ChordUnityChannelListener
 *********************************************************************/

/**
 * This interface defines a listener for chord channel events.
 * Listens for joining and leaving channels, and data and file transfers, browse path.
 * A listener implementation can be registered using the start() API called when starting the Chord Unity.
 */
public interface ChordUnityChannelListener
{

    /**
             * Called when a node join event is raised on the channel.
             * 
             * @param fromNode The node name corresponding to which the join event has
             *            been raised.
             * @param fromChannel The channel on which the join event has been raised.
         
             */

    void onNodeJoined(string fromNode, string fromChannel);

    /**
            * Called when a node leave event is raised on the channel.
            * 
            * @param fromNode The node name corresponding to which the leave event has
            *            been raised.
            * @param fromChannel The channel on which the leave event has been raised.
         
            */

    void onNodeLeft(string fromNode, string fromChannel);

    /**
            * Called when the data message received from the node.
            * 
            * @param fromNode The node name that the message is sent from.
            * @param fromChannel The channel name that is raised event.
            * @param payloadType User defined message type
            * @param payload Array of payload.
         
            */

    void onDataReceived(string fromNode, string fromChannel, string payloadType, byte[][] payload);


    /**
             * Called when the Share file notification is received. User can decide to
             * receive or reject the file.
             * 
             * @param fromNode The node name that the file transfer is requested by.
             * @param fromChannel The channel name that is raised event.
             * @param fileName Original file name will be received.
             * @param hash Hash value of file  it can be used to detect source data changes.
             * @param fileType User defined file type
             * @param exchangeId Transmission identifier.
             * @param fileSize The size of file to receive.
         
             */

    void onFileWillReceive(string fromNode, string fromChannel, string fileName,

                string hash, string fileType, string exchangeId, long fileSize);

    /**
             * Called when an individual chunk of the file is received.
             * 
             * @param fromNode The node name that the file transfer is requested by.
             * @param fromChannel The channel name that is raised event.
             * @param fileName Original file name will be received.
             * @param hash Hash value of file  it can be used to detect source data changes.
             * @param fileType User defined file type
             * @param exchangeId Transmission identifier.
             * @param fileSize The size of file to receive.
             * @param offset size of received chunk.
         
             */

    void onFileChunkReceived(string fromNode, string fromChannel, string fileName,

                string hash, string fileType, string exchangeId, long fileSize, long offset);


    /**
           * Called when the file transfer is completed from the node.
           * It need to rename tmpFilePath with fileName or move to appropriate path
           * 
           * @param fromNode The node name that the file transfer is requested by.
           * @param fromChannel The channel name that is raised event.
           * @param fileName Original file name will be received.
           * @param hash Hash value of file  it can be used to detect source data changes.
           * @param fileType User defined file type
           * @param exchangeId Transmission identifier.
           * @param fileSize The size of file to receive.
           * @param tmpFilePath Temporarily stored file path it is assigned with
           *            {            setTempDirectory()}.
         
           */
    void onFileReceived(string fromNode, string fromChannel, string fileName,

                string hash, string fileType, string exchangeId, long fileSize, string tmpFilePath);


    /**
            * Called when an individual chunk of the file is sent.
            * 
            * @param toNode The node name to which the file is sent.
            * @param toChannel The channel name that is raised event.
            * @param fileName Original file name will be received.
            * @param hash Hash value of file  it can be used to detect source data changes.
            * @param fileType User defined file type
            * @param exchangeId Transmission identifier.
            * @param fileSize The size of file to receive.
            * @param offset size of received chunk.
            * @param chunkSize Chunk size
         
            */
    void onFileChunkSent(string toNode, string toChannel, string fileName, string hash,

                string fileType, string exchangeId, long fileSize, long offset, long chunkSize);


    /**
             * Called when the file transfer is completed to the node.
             * 
             * @param toNode The node name that the file is sent.
             * @param toChannel The channel name that is raised event.
             * @param fileName Original file name will be received.
             * @param hash Hash value of file  it can be used to detect source data changes.
             * @param fileType User defined file type
             * @param exchangeId Transmission identifier.
         
             */
    void onFileSent(string toNode, string toChannel, string fileName, string hash,

                string fileType, string exchangeId);


    /**
             * Called when the error is occurred while the file transfer is in progress.
             * 
             * @param node The node name that the error is occurred by.
             * @param channel The channel name that is raised event.
             * @param fileName Original file name will be received.
             * @param hash Hash value of file  it can be used to detect source data changes.
             * @param exchangeId Transmission identifier.
             * @param reason The reason for failure could be one of
             *            {ERROR_FILE_CANCELED}
             *            {ERROR_FILE_CREATE_FAILED},
             *            {ERROR_FILE_NO_RESOURCE}
             *            {ERROR_FILE_REJECTED}
             *            {ERROR_FILE_SEND_FAILED}
             *            {ERROR_FILE_TIMEOUT}
         
             */
    void onFileFailed(string node, string channel, string fileName, string hash,

                string exchangeId, int reason);

    /**
     * Called when the Share files notification is received. User can decide to
     * receive or reject the file.
     * 
     * @param fromNode The node name that the file transfer is requested by.
     * @param fromChannel The channel name that is raised event.
     * @param fileName Original file name will be received.
     * @param taskId Transmission identifier.
     * @totalCount The number of files to received
     * @param fileType User defined file type
     * @param fileSize The size of file to receive.
     * 
     */
    void onMultiFilesWillReceive(string fromNode, string fromChannel, string fileName,

                string taskId, int totalCount, string fileType, long fileSize);


    /**
     * Called when an individual chunk of the file is received.
     * 
     * @param fromNode The node name that the file transfer is requested by.
     * @param fromChannel The channel name that is raised event.
     * @param fileName Original file name will be received.
     * @param taskId Transmission identifier.
     * @param index Index of file
     * @param fileType User defined file type
     * @param fileSize The size of file to receive.
     * @param offset size of received chunk.
     */
    void onMultiFilesChunkReceived(string fromNode, string fromChannel, string fileName,

                string taskId, int index, string fileType, long fileSize, long offset);


    /**
     * Called when the receiving a file is completed from the node.
     * It need to rename tmpFilePath with fileName or move to appropriate path.
     * 
     * @param fromNode The node name that the file transfer is requested by.
     * @param fromChannel The channel name that is raised event.
     * @param fileName Original file name will be received.
     * @param taskId Transmission identifier.
     * @param index Index of file
     * @param fileType User defined file type
     * @param fileSize The size of file to receive.
     * @param tmpFilePath Temporarily stored file path it is assigned with
     *            {  setTempDirectory()}.
     */
    void onMultiFilesReceived(string fromNode, string fromChannel, string fileName,

                string taskId, int index, string fileType, long fileSize, string tmpFilePath);


    /**
     * Called when an individual chunk of the file is sent.
     * 
     * @param toNode The node name to which the file is sent.
     * @param toChannel The channel name that is raised event.
     * @param fileName Original file name will be received.
     * @param taskId Transmission identifier.
     * @param index Index of file
     * @param fileType User defined file type
     * @param fileSize The size of file to receive.
     * @param offset size of received chunk.
     * @param chunkSize Chunk size
     */
    void onMultiFilesChunkSent(string toNode, string toChannel, string fileName, string taskId,

                int index, string fileType, long fileSize, long offset, long chunkSize);


    /**
     * Called when the sending a file is completed.
     * 
     * @param toNode The node name that the file is sent.
     * @param toChannel The channel name that is raised event.
     * @param fileName Original file name will be received.
     * @param taskId Transmission identifier.
     * @param index Index of file
     * @param fileType User defined file type
     */
    void onMultiFilesSent(string toNode, string toChannel, string fileName, string taskId,

                int index, string fileType);


    /**
     * Called when the error is occurred while the file transfer is in progress.
     * 
     * @param node The node name that the error is occurred by.
     * @param channel The channel name that is raised event.
     * @param fileName Original file name will be received.
     * @param taskId Transmission identifier.
     * @param index Index of file
     * @param reason The reason for failure could be one of
     *            {ERROR_FILE_CANCELED}
     *            {ERROR_FILE_CREATE_FAILED},
     *            {ERROR_FILE_NO_RESOURCE}
     *            {ERROR_FILE_SEND_FAILED}
     *            {ERROR_FILE_TIMEOUT}
     */
    void onMultiFilesFailed(string node, string channel, string fileName, string taskId,

                int index, int reason);

    /**
     *  Called when the transfer of multiple files is finished to the node.
     * 
     * @param node The node name that the file is sent.
     * @param channel The channel name that is raised event.
     * @param taskId Transmission identifier.
     * @param reason The reason for finish could be one of
     *            {ERROR_NONE}
     *            {ERROR_FILE_CANCELED}
     *            {ERROR_FILE_REJECTED}
     *            {ERROR_FILE_NO_RESOURCE}
     *            {ERROR_FILE_REJECTED}
     *            {ERROR_FILE_SEND_FAILED}
     *            {ERROR_FILE_TIMEOUT}
     */

    void onMultiFilesFinished(string node, string channel, string taskId, int reason);
	
	void onUdpDataReceived(string fromNode, string fromChannel, string payloadType,byte[][] payload,string sessionName);
	      
	void onUdpDataDelivered(string toNode, string toChannel, string reqId);

    /**
     *  Called when the files are selected by user after browsing file explorer.
     * 
     * @param fileList List of file selected by the user.
         
     */
    void onBrowseResult(string[] fileList);
	
	

}

