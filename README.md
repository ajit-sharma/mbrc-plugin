MusicBee Remote (Plugin) [![Build Status](https://travis-ci.org/kelsos/mbrc-plugin.svg?branch=development)](https://travis-ci.org/kelsos/mbrc-plugin)
====================

## About
-------
This is a plugin for [MusicBee](http://getmusicbee.com/) that is required for [MusicBee Remote](https://github.com/kelsos/mbrc) android application to function.

### Current release
If you want to access the code, or binary of the current releases check the [Releases Page](https://github.com/kelsos/mbrc-plugin/releases). The current stable version is [**v1.0.0**](https://github.com/kelsos/mbrc-plugin/releases/tag/v1.0.0)

### Legacy socket implementation
The version 1.0.0 released is based on the old plugin codebase that uses plain old TCP sockets in order to communicate.
The plugin has been partially improved to support some extra functionality.

There are a couple of previous protocol implementations available with the very first ones using an XML based protocol over TCP.
The protocol has been changed to use JSON formatted messages in the later versions (up to 1.x.x).

### ServiceStack based
There is an experimental implementation based on [ServiceStack](https://github.com/kelsos/mbrc-plugin/tree/6d321749347a38c281d22baa9a928d14cd8eaab3).
ServiceStack provides a REST API and websockets (Fleck) are used to push messages about changes on the player.

For the ServiceStack version there is no public documentation of the API Available but it is planned as soon as the version 1.x features are finalized, however there is a metadata page available when the plugin running that has information on most of the calls. The API is somewhat documented and build with ServiceStack.Api.Swagger, so if you put the *swagger-ui* folder in the MusicBee Plugins folder you should be able to access the documentation through *http://localhost:port/swagger-ui/index.html* (where port is the port marked as http at the plugin settings page). Please keep in mind that the resource list should be available under *http://localhost:port/resources*, and you will probably need to insert it manually if you downloaded and extracted the swagger-ui zip file.

### NancyFx rewrite (should be v2.0.0)

At the moment the project rewritten and it is separated to a number of sub projects.
The final **v2.0.0** REST API will be pretty close to the current ServiceStack API.

The API documentation is currently work in progress and it should reside under the [documentation](documentation) folder

The **mbrc-data** module provides access to the embedded database that is used for caching the player's data.

The **mbrc-core** module implements the basic plugin functionality and hosts the NancyFX based REST server. It also includes the proper abstraction layer (ApiAdapter Interfaces) that the host must implement.

The **mbrc-plugin** module is the plugin that runs in MusicBee. The plugin interfaces with **mbrc-core**, provides implementations for the ApiAdapter abstraction.

The **firewall-utility** is used to make it easier for the plugin to open the required for communication firewall ports.

I am currently in the process of adding Unit Tests to the plugin code using **NUnit**.

### After version 2.0.0

At some point after the rewrite of the client I plan on implementing an embedded single page web client based on *AngularJS*.

### Building

To build the plugin you have to open it with Visual Studio 2015. After opening the project you will probably have to restore the required packages with NuGet.

### Credits

*   [Dapper](https://github.com/StackExchange/dapper-dot-net)

    [Apache v2](http://www.apache.org/licenses/LICENSE-2.0)

*   [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD)

    [Apache v2](http://www.apache.org/licenses/LICENSE-2.0)

*   [Reactive Extensions](https://rx.codeplex.com/)

    [Apache v2](http://www.apache.org/licenses/LICENSE-2.0)

*   [Fleck](https://github.com/statianzo/Fleck)

    [MIT](https://github.com/statianzo/Fleck#license)

*   [Nancy](https://github.com/NancyFx/Nancy)

    [MIT](https://raw.githubusercontent.com/NancyFx/Nancy/master/license.txt)

*   [Nancy.Bootstrappers.Ninject](https://raw.githubusercontent.com/NancyFx/Nancy.Bootstrappers.Ninject)

    [MIT](https://raw.githubusercontent.com/NancyFx/Nancy.Bootstrappers.Ninject/master/license.txt)

*   [Nancy.Serialization.JsonNet](https://github.com/NancyFx/Nancy.Serialization.JsonNet)

    [MIT](https://raw.githubusercontent.com/NancyFx/Nancy.Serialization.JsonNet/master/license.txt)

*   [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

    [MIT](https://raw.githubusercontent.com/JamesNK/Newtonsoft.Json/master/LICENSE.md)

*   [SQLite](https://www.sqlite.org/)

    [Public Domain](https://www.sqlite.org/copyright.html)

*   [NLOG](https://github.com/NLog/NLog)

    [BSD LICENCE](https://github.com/NLog/NLog/blob/master/LICENSE.txt)

*   [Ninject](https://github.com/ninject/ninject)

    [Apache v2](https://github.com/ninject/Ninject/blob/master/LICENSE.txt)

*   [NUnit](https://github.com/nunit/nunit)

    [MIT](https://github.com/nunit/nunit/blob/master/LICENSE.txt)

*   [AutoFixture](https://github.com/AutoFixture/AutoFixture)

    [MIT](https://github.com/AutoFixture/AutoFixture/blob/master/LICENCE.txt)

*   [WebSocketProxy](https://github.com/lifeemotions/websocketproxy)
    [MIT](https://raw.githubusercontent.com/lifeemotions/websocketproxy/master/LICENSE)


License
------


    MusicBee Remote (Plugin for MusicBee)
    Copyright (C) 2011-2017  Konstantinos Paparas

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
