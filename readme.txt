Conway's Game of Life

Problem: the implementation of the game "Conway's Game of Life"
Description: https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life

Source code: https://github.com/akovrigin/Life

Deployment:
Get source code and build project in Visual Studio.
This is self-host application and it has to be executed with administration privileges.
$Life\LifeHost\bin\Debug\LifeHost.exe
Or it can be executed directly from Visual Studio. In this case Visual Studio itself has to be executed with administration privileges.

Configuration:
File of configuration: $Life\LifeHost\bin\Debug\LifeHost.exe.config
Key "server" contains the URL (by default http://localhost:5555)
Key "IsAlive". If set to "true", than world of the game will be initially filled with about 2000 objects. If set to "false", than world will be clean.

Reason for the technical choice.
Deployment has to be very easy on a computer with Windows OS.
Is has to be small but all-sufficient application with web-client UI.

