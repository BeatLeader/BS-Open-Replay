# BS Open Replay

BeatSaber open replay format.

# Current format (bad and )

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