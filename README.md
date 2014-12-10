MusicBee Remote (Plugin)
====================

About
-------
This is a plugin for [MusicBee](http://getmusicbee.com/) that is required for [MusicBee Remote](https://github.com/kelsos/mbrc) android application to function. The lasted version of the plugin uses TCP Sockets to pass notifications to the client (Android Application). The data are available through an HTTP RESTlike API in JSON format.

The current implementation of the protocol is the third. The first implemenation was Socket based and the message format was XML, this was used during the 0.2 versions of the application (remote/plugin). The second implementation was also Socket based and the data format was JSON, the second implemenation was used during the 0.9 versions. The third implementation uses the socket to only pass small notifications of changes on the player. The data are available through an HTTP RESTlike API that returns returns JSON formatted messages, and it will be avaiblable with the release of version 1.x.

Currently there is no documenation of the API Available but it is planned as soon as the version 1.x features are finalized.

Building
-------
To build the plugin you have to open it with Visual Studio 2013. After opening the project you will probably have to restore the required packages with NuGet.

Credits
-------

*   [ServiceStack v3](https://github.com/ServiceStackV3/ServiceStackV3)

    ServiceStack.Text is used for JSON parsing
    
    ServiceStack.OrmLite.Sqlite is used for the internal cache.
    
    [BSD LICENCE](https://github.com/ServiceStack/ServiceStack/blob/v3/LICENSE)
    
*   [SQLite](https://www.sqlite.org/)

    [Public Domain](https://www.sqlite.org/copyright.html)
    
*   [NLOG](https://github.com/NLog/NLog)
    
    [BSD LICENCE](https://github.com/NLog/NLog/blob/master/LICENSE.txt)

*   [Ninject](https://github.com/ninject/ninject)

    [Apache v2](https://github.com/ninject/Ninject/blob/master/LICENSE.txt)


License
------


    MusicBee Remote (Plugin for MusicBee)
    Copyright (C) 2013  Konstantinos Paparas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
