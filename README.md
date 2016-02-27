MusicBee Remote (Plugin)
====================

## About
-------
This is a plugin for [MusicBee](http://getmusicbee.com/) that is required for [MusicBee Remote](https://github.com/kelsos/mbrc) android application to function.


### Current release
If you want to access the code, or binary of the current releases check the [Releases Page](https://github.com/kelsos/mbrc-plugin/releases). The current stable version is **v0.13.0**

### ServiceStack based
There is an experimental implementation based on [ServiceStack](https://github.com/kelsos/mbrc-plugin/tree/6d321749347a38c281d22baa9a928d14cd8eaab3).
ServiceStack provides a REST API and websockets (Fleck) are used to push messages about changes on the player.

For the ServiceStack version there is no public documentation of the API Available but it is planned as soon as the version 1.x features are finalized, however there is a metadata page available when the plugin running that has information on most of the calls. The API is somewhat documented and build with ServiceStack.Api.Swagger, so if you put the *swagger-ui* folder in the MusicBee Plugins folder you should be able to access the documentation through *http://localhost:port/swagger-ui/index.html* (where port is the port marked as http at the plugin settings page). Please keep in mind that the resource list should be available under *http://localhost:port/resources*, and you will probably need to insert it manually if you downloaded and extracted the swagger-ui zip file.

### NancyFx rewrite.

At the moment the project reworked and it is broken down to a number of sub projects.

The **mbrc-data** module provides access to the embedded database that is used for caching the player's data.

The **mbrc-core** module implements the basic plugin functionality and hosts the nancyfx based REST server. It also includes the proper abstraction layer (ApiAdapter Interfaces) that the host must implement.

The **mbrc-plugin** module is the plugin that runs in MusicBee. The plugin interfaces with **mbrc-core**, provides implementations for the ApiAdapter abstraction.

The **firewall-utility** is used to make it easier for the plugin to open the required for communication firewall ports.

I am currently in the process of adding Unit Tests to the plugin code using **NUnit**.

### Building

To build the plugin you have to open it with Visual Studio 2015. After opening the project you will probably have to restore the required packages with NuGet.

### Credits

*   [Dapper](https://github.com/StackExchange/dapper-dot-net)

    [Apache v2](http://www.apache.org/licenses/LICENSE-2.0)

*   [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD)

    [Apache v2](http://www.apache.org/licenses/LICENSE-2.0)

*   [Reactive Extensions](https://rx.codeplex.com/)

    [Apache v2](http://www.apache.org/licenses/LICENSE-2.0)

*   [SQLite](https://www.sqlite.org/)

    [Public Domain](https://www.sqlite.org/copyright.html)

*   [NLOG](https://github.com/NLog/NLog)

    [BSD LICENCE](https://github.com/NLog/NLog/blob/master/LICENSE.txt)

*   [Ninject](https://github.com/ninject/ninject)

    [Apache v2](https://github.com/ninject/Ninject/blob/master/LICENSE.txt)


License
------


    MusicBee Remote (Plugin for MusicBee)
    Copyright (C) 2011-2016  Konstantinos Paparas

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
