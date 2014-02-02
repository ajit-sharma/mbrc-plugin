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

