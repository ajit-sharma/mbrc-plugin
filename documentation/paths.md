## Paths
### The currently playing track info
```
GET /track
```

#### Description

The endpoint returns information about the now playing track


#### Responses
|HTTP Code|Description|Schema|
|----|----|----|
|200|An array of products|TrackInfo|


#### Tags

* Track

### The cover of the playing track
```
GET /track/cover
```

#### Description

The endpoint returns the cover of the currently playing track


#### Responses
|HTTP Code|Description|Schema|
|----|----|----|
|200|The cover image|No Content|


#### Produces

* image/jpeg

#### Tags

* Track

