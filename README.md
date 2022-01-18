# BS Open Replay

BeatSaber open replay format.

# Current format

## Origin

Parsed ScoreSaber replays for good ranked plays. You can use ScoreSaber leaderboard API or HEAD request to the https://scoresaber.com/game/replays/songID-playerID.dat in order to check for replay.

## API

https://sspreviewdecode.azurewebsites.net/?playerID=playerID&songID=songID

Where:

- playerID is Steam or Oculus id of player.
- songID is ScoreSaber leaderboardID.

## Format

JSON file with such structure:
```
{
    info: {
        version: ,                 // String. Version of the replay
		hash: ,                    // String. Song hash
	    difficulty: ,              // int. Song difficulty (1 - E, 3 - H, 5 - E, 7 - E+)
		mode: ,                    // String
		environment: ,             // String
		modifiers: ,               // [String]
		noteJumpStartBeatOffset: , // float
		leftHanded: ,              // bool
		height: ,                  // float. Player static height
		rr: ,                      // float. Room rotation
		room: {                    // Room offsets
            x:,
            y:,
            z:
        },
		st:                        // float
    }
    frames: [{   // One for the each frame of the replay
        h: {     // Head 
            r: { // Rotation quaternion
              x, y, z, w  
            }, 
            p: { // Position
              x, y, z
            }  
        },
        l: {     // Left hand
            r: { // Rotation quaternion
              x, y, z, w  
            }, 
            p: { // Position
              x, y, z
            }  
        },
        r: {     // Right hand
            r: { // Rotation quaternion
              x, y, z, w  
            }, 
            p: { // Position
              x, y, z
            }  
        }
    }], 
    scores: ,      // [int]. Score for every note. -5 - wall, -4 - bomb, -3 - miss, -2 - badcut, 1 to 115 - normal score. Walls are the last.
    combos: ,      // [int]. Combo for every note.
    noteTime: ,    // [float]. Time each note was cut. In seconds.
    noteInfos: ,   // [string]. Note type and position. Concatenated four ints: lineIndex + lineLayer + cutDirection + type
    dynamicHeight: // [float]. Player dynamic height value. Can be empty if user uses static height. 
}
```