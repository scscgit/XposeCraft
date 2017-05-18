# XposeCraft/API

If you either don't have Unity installed, or don't want to see unavailable internal classes during the bot development, you will have to include this XposeCraft_API.dll as a reference in your C# project, which consists of source codes located in Assets/BotScripts directory.

The Sources directory contains empty classes meant to be as mirrors to the real API, located in Assets/Game/Scripts/Game directory. Every time the Game API changes, they should be updated correspondingly and a new Class Library in a DLL format should be re-created.
