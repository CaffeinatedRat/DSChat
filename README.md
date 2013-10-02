DSChat
===========

This is a .NET distributed UDP chat tool written completely in C#.  It is designed for Intranet network communication only.  It works as a client and a server, where one instance of the application should be launched as the server, and all other instances as clients.  The other clients can be launched on the same machine or on other machines on the network.  The instance that is running as a server simply needs a port to be hosted on, while the each client must specify the port that the client will be listening to, and the host & port of the server.  An optional passphrase can be used to encrypt each message, but all clients and the server must have the exact passphrase.

Origin:

This application started as a class project in Distributed Computing to mimic a carwash that was composed of three independent parts.  The queueing mechanism, the actual car wash, and the finalizer (element used to log events).  From there it was modified to support chat on a local Intranet.

If you find any bugs please notify me.
* Author's Website: [http://www.caffeinatedrat.com](http://www.caffeinatedrat.com)
* Bugs/Suggestions: CaffeinatedRat at gmail dot com

Requirements
------------

Requires a Microsoft Visual Studio or an IDE that is capable of compiling C# with .NET framework 4.0, such as Mono.

* [MonoDevelop] (http://monodevelop.com/)
* [Visual Studio Express 2012] (http://www.microsoft.com/visualstudio/eng/products/visual-studio-express-for-windows-8)
