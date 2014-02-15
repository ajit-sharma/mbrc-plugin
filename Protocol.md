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
        "total": 16262,
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
> **total**: The total number of tracks in the library during the sync. This is used to know the number of tracks to request.
## Playlist Sync

### Getting the available playlists
In order to get the available playlists the user has to send a message like the following:
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "get"
        "limit": 50,
        "offset": 0
    }
}
```
> **offset**: The index of the first track contained in the requested data.
> **limit**: The number of tracks requested in the current batch.

In reply the plugin must send a message in the following format.
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "get",
        "total": 16,
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
> **name**: is the name of the playlist,
> **tracks**: is the number of tracks in the specified playlist,
> **hash**: is a sha1 hash of the playlist path on the filesystem
> **total**: The total number of playlists available.


### Getting Tracks
In order to get the tracks for a playlist the client has to send a message like the following to the plugin.

```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "gettracks",
        "limit": 50,
        "offset": 0,
        "hash": "6b541fd56872432839d3eb77b8212eb004c50129"
    }
}
```

> **offset**: The index of the first track contained in the requested data.
> **limit**: The number of tracks requested in the current batch.
> **hash**: The sha1 hash of the playlist.

```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "gettracks",
        "total": 1284,
        "files": [
            {
                "artist": "Artist Name",
                "title": "Track title",
                "hash": "6b541fd56872432839d3eb77b8212eb004c50129"
        ]
    }
}
```
**total**: Contains the total number of available tracks in the requested playlist.

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
> **hash**: It is the hash of the playlist we want to add tracks to.
> **files**: It is an array containing the hashes of the files we want to add to the specified playlist

### Playing a playlist

To add a playlist for immediated playback:
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "play",
        "hash": "6b541fd56872432839d3eb77b8212eb004c50129",
    }
}
```

> **hash**: Is the sha1 hash obtained by the path of the playlist on the filesystem.

### Removing a track from a playlist

```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "remove",
        "playlist": "6b541fd56872432839d3eb77b8212eb004c50129",
        "index": 1
    }
}
```

> **playlist**: The sha1 hash representing the playlist.
> **index**: The index of the file in the playlist.

### Rearranging tracks in a playlist
Though the MusicBee API supports moving multiple tracks at this point the protocol only supports moving a single track to a new position.

To move a track the client has to send the following message:

```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "move",
        "playlist": "6b541fd56872432839d3eb77b8212eb004c50129",
        "from": 1,
        "to": 2
    }
}
```
The message will be processed by the plugin and it will send a reply containing whether the track move was successful or not. 
```
{
    "context": "playlists",
    "type": "req",
    "data": {
        "type": "move",
        "playlist": "6b541fd56872432839d3eb77b8212eb004c50129",
        "success": true
        "from": 1,
        "to": 2
    }
}
```

> **playlist**: Contains the sha1 hash representing the playlist.
> **from**: Contains the former position of the track to be moved.
> **to**: Contains the new position where the track will be moved.

----------
