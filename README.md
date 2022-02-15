# BS Open Replay

BeatSaber open replay format.

# BSOR V1

Beat Saber Open Replay
.bsor
- [C# code](https://github.com/BeatLeader/beatleader-mod/blob/master/2_Core/Models/Replay.cs)
- [JS code](https://github.com/NSGolova/BeatSaber-Web-Replays/blob/nsgolova/openreplay/src/open-replay-decoder.js)

Binary file with the such structure:

```
0x442d3d69                    - int, unique magic number.
1                             - byte, file version.

0                             - byte, info structure start.
{                             - Info structure
  version                     - string, Mod version
  gameVersion                 - string, Game version
  timestamp;                  - string, play start unix timestamp.
  
  playerID;
  playerName;
  platform;

  trackingSytem;
  hmd;
  controller;

  hash;
  songName;
  mapper;
  difficulty;

  score                        - int, total modified score.
  mode                         - string, game mode. (Standard, OneSaber, Lawless, etc.)
  environment                  - string, environment name. (The beginning, etc.)
  modifiers                    - comma separated string, game modifiers. (FS, GN, etc.)
  jumpDistance                 - float, note jump distance.
  leftHanded                   - bool
  height                       - float, static height

  startTime                    - float, song start time (practice mode).
  failTime                     - float, song fail time (only if failed).
  speed                        - float, song speed (practice mode).
}

1                              - byte, frames array start.
framesCount                    - int, frames count.
{                              - Frame structure
  time                         - float, song time
  fps                          - int, player's FPS
  {                            - Head structure
    {x, y, z}                  - 3 floats, position.
    {x, y, z, w}               - 4 floats, rotation.
  }
  {                            - Left hand structure
    {x, y, z}                  - 3 floats, position.
    {x, y, z, w}               - 4 floats, rotation.
  }
  {                            - Right hand structure
    {x, y, z}                  - 3 floats, position.
    {x, y, z, w}               - 4 floats, rotation.
  }
}

2                              - byte, note events array start.
noteCount                      - int, note events count.
{                              - Note event structure.
  noteID                       - int, lineIndex*1000 + noteLineLayer*100 + colorType*10 + cutDirection
  eventTime                    - float, song time of event 
  spawnTime                    - float, spawn time of note
  eventType                    - int, good = 0,bad = 1,miss = 2,bomb = 3
  {                            - Cut info structure (only for Good and Bad!)
    bool speedOK;
    bool directionOK;
    bool saberTypeOK;
    bool wasCutTooSoon;
    float saberSpeed;
    Vector3 saberDir;
    int saberType;
    float timeDeviation;
    float cutDirDeviation;
    Vector3 cutPoint;
    Vector3 cutNormal;
    float cutDistanceToCenter;
    float cutAngle;
    float beforeCutRating;
    float afterCutRating;
  }
}

3                              - byte, wall events array start
wallCount                      - int, wall events count
{
  wallID                       - int, lineIndex*100 + obstacleType*10 + width
  energy                       - float, energy at the end of event
  time                         - float, song time of event 
  spawnTime                    - float, spawn time of wall
}

4                              - byte, automatic height array start
heightCount                    - int, height change events count
{
  height                       - float, height value
  time                         - float, song time
}

5                              - byte, pause array start
pauseCount                     - int, pauses count
{
  duration                     - long, duration in milliseconds
  time                         - float, pause start time
}
```

# OPA V1

Opennes Protection Algorhitm.

Server hosted algorhitm requiring database and authentication. 

# Legacy format (ScoreSaber based)

## Origin

Parsed ScoreSaber replays for good ranked plays. You can use ScoreSaber leaderboard API or HEAD request to the https://scoresaber.com/game/replays/songID-playerID.dat in order to check for replay.

## API

### ScoreSaber download

https://sspreviewdecode.azurewebsites.net/?playerID=playerID&songID=songID

Where:

- playerID is Steam or Oculus id of player.
- songID is ScoreSaber leaderboardID.

### Custom URL download

https://sspreviewdecode.azurewebsites.net/?link=link

Where:

- link is direct download link to replay.
Example: https://cdn.discordapp.com/attachments/921820046345523314/934953493624660058/76561198059961776-Cheshires_dance-ExpertPlus-Standard-A2B943FE75E48394352B4FD912CEE8306788D0B1.dat

### Custom file

POST request with replay file in body

https://sspreviewdecode.azurewebsites.net

## Format

JSON file with such structure:
```
{
    info: {
        version: ,                 // String. Version of the replay
        hash: ,                    // String. Song hash
        difficulty: ,              // int. Song difficulty (1 - E, 3 - H, 5 - E, 7 - E+)
        mode: ,                    // String. Game mode. (Standard, OneSaber, etc.)
        environment: ,             // String. Environment name (BigMirrorEnvironment, etc.)
        modifiers: ,               // [String]. Game modifiers. (GN, FS, etc.)
        noteJumpStartBeatOffset: , // float. Offset to calculate JD value.
        leftHanded: ,              // bool. Left handed play, all the notes is mirrored
        height: ,                  // float. Player static height
    }
    frames: [{   // One for the each frame of the replay
        h: {     // Head 
            r: { // Rotation quaternion
              x:, y:, z:, w:  
            }, 
            p: { // Position
              x:, y:, z:
            }  
        },
        l: {     // Left hand
            r: { // Rotation quaternion
              x:, y:, z:, w:  
            }, 
            p: { // Position
              x:, y:, z:
            }  
        },
        r: {     // Right hand
            r: { // Rotation quaternion
              x:, y:, z:, w:  
            }, 
            p: { // Position
              x:, y:, z:
            }  
        },
        a: ,     // float. Frame time
        i:       // int. Player FPS 
    }], 
    scores: ,        // [int]. Score for every note. -5 - wall, -4 - bomb, -3 - miss, -2 - badcut, 1 to 115 - normal score. Walls are the last.
    combos: ,        // [int]. Combo for every note.
    noteTime: ,      // [float]. Time each note was cut. In seconds.
    noteInfos: ,     // [string]. Note type and position. Concatenated four ints: lineIndex + lineLayer + cutDirection + type
    dynamicHeight: [{// Player dynamic height value. Can be empty if user uses static height. 
      h:             // float. Height
      a:             // float. Time
    }]
}
```

# Motivation

Replays are a great source of information about the game. They not only can be used for displaying the recording of the play but as a statistic or insight data source. Only the ScoreSaber mod is capable of recording, playing, and storing replays. Replay logic and file format is closed and used only by the ScoreSaber. It's maybe great from the security point of view but leads to stagnation. 

The main goal of this project is to design and develop an open-source replay system which is secure and useful at the same time.

# Roadmap

- Chose which data should be sent.
- Create a new replay format.
- Create an algorithm for marking the replay to prevent usage of another's replay.
- Develop the server.
- Develop recorder and player. 