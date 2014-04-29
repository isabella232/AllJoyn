HOST_RELAY_SERVER Eclipse plugin needs to be run in windows machine, 
which will act as a relay server between the emulator network and outside network

# Steps to Install and run the Chord Plugin
A. Steps to Install Plugin in Eclipse
   1) Open Eclipse . 
      Go to Help -> Install New Software
      Press Add Button
      Select Local and browse to the  'Chord SDK Plugin' directory
      Give name as Chord Plugin
      Press Ok
   2) It wil show Chord_Eclipse_Plugin in the Name box
      Select the appeared item
      Press Next
      It will show the details
      Click Next
      Accept the terms of Agreement
      Then Press Finish
   3) If it asks for Security Warning, Press Ok
      Then it will prompt you to restart the Eclipse
      Press Ok
B. Steps to Run Plugin in Eclipse
   You can see "Run Chord Relay" in the menu bar
   Click that menu and Press "Host Relay Server" 
   It will open a text box window where you can see the logs
C. Steps to run the Test application using the Emulator
   The AVD Emulator can be launched directly from Eclipse. 
   The emulator is launched by default when there are no devices attached to the system and 
   the application is launched by choosing ¡°Run As¡± -> ¡°Android Application". 
   This behavior can be changed using ¡°Run Configurations¡±. 
   Ensure that no other instance of AVD Emulator is already running on the system. 
   Also, the Emulator needs to be configured to contain an SD card with sufficient storage for testing file transfer functionality using Chord.
   Once the emulator starts running and the application is installed, 
   Chord can be initialized and operations such as join channel and data/file transfer can be performed


# Steps to Uninstall the Plugin from eclipse 
    Go to Help -> Install New Software -> already installed(link).
    Select the Chord Plugin from the list. 
    Click Uninstall. 
    It will show the dialog box with details. Press Finish