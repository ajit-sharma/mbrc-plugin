MusicBee Remote Protocol
=====================

About
-----
This file tries to be a plan for the protocol implementation for MusicBee Remote.

The version 1.x of the protocol as supported by the older plugin versions was based on XML formatted messages and has been deprecated.

This document tries to act as a specification for the second version of the communication protocol with is based on JSON Formated messages.

The document is not complete and it will be filled in future with more information.

Message Delimiter
-----
All the protocol messages are delimited by a new line ```\r\n``` see something similar to  [Line_Delimited_JSON](https://en.wikipedia.org/wiki/Line_Delimited_JSON).

Commands
-----
## Library Sync
### Full Sync
The full sync procedure consists of two phases. The in the first phase tha client gets all the paths of the files in the libary. In the second phase the client syncs the metadata and covers for these files.

#### Initiate full sync
To initiate the full sync process the client has to sent a ```full``` sync command with an ```init``` action.
```
{
    "context": "librarysync",
    "type": "req",
    "data": {
        "type": "full"
        "action": "init"
    }
}
```

The plugin will reply with the number of tracks available.
```
{   
    "context":"librarysync",
    "type":"rep",
    "data":{
        "type":"full",
        "payload":16262
    }
}
```

Generally the payload contains the number of the tracks currently in the library.

#### Get metadata batch
Since now the clients knows the number of tracks it has to start requesting batches of metadata to process and store.

```
{
    "context": "librarysync",
    "type": "rep",
    "data": {
        "type": "meta",
        "offset": 0,
        "limit": 50
    }
}
```
**offset**: represents the start of the next batch.
**limit**: represents the number of data in the batch.

After each batch request the client will reply 
```
{
    "context":"librarysync",
    "type":"rep",
    "data":{
        "type":"meta",
        "data":[
            {
                "album":"Idja",
                "title":"Odda Mailbmi",
                "genre":"Folk Metal",
                "year":"1999",
                "track_no":"01",
                "hash":"6b541fd56872432839d3eb77b8212eb004c50129",
                "artist":"Shaman",
                "album_artist":"Shaman"
            },...
        ]
    }
}

```

## Playlist Sync

### Getting the available playlists
In order to get the available playlists the user has to send a message like the following:
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "get"
    }
}
```

In reply the plugin must send a message in the following format.
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "get",
        "playlists": [
            {
                "name": "Name of the playlist",
                "tracks": 31,
                "hash": "6b541fd56872432839d3eb77b8212eb004c50129"
            },
            ...
        ]
    }
}
```
**name**: is the name of the playlist,
**tracks**: is the number of tracks in the specified playlist,
**hash**: is a sha1 hash of the playlist path on the filesystem


### Getting Tracks
In order to get the tracks for a playlist the client has to send a message like the following to the plugin.

**hash**: is a sha1 hash of the path of the playlist in the computer's filesystem. The plugin should have a cache, mapping the sha1 hashes to the path.
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "gettracks",
        "hash": "6b541fd56872432839d3eb77b8212eb004c50129"
    }
}
```

After the request the client should receive a package containing the following
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "gettracks",
        "files": [
            {
                "artist": "Artist Name",
                "title": "Track title",
                "hash": "6b541fd56872432839d3eb77b8212eb004c50129"
        ]
    }
}
```

### Creating a new playlist
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "create",
        "name": "playlist name",
        "files: [
            "6b541fd56872432839d3eb77b8212eb004c50129",
            "6b541fd23572432839d3eb77b8212eb004c50129",
            ...
        ]
    }
}
```
### Adding files to playlist
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "add",
        "hash": "6b541fd56872432839d3eb77b8212eb004c50129",
        "files: [
            "6b541fd56872432839d3eb77b8212eb004c50129",
            "6b541fd23572432839d3eb77b8212eb004c50129",
            ...
        ]
    }
}
```
**hash**: It is the hash of the playlist we want to add tracks to.
**files**: It is an array containing the hashes of the files we want to add to the specified playlist

