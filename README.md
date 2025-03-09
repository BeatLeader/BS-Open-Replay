# BS Open Replay

Beat Saber open replay format.

# BSOR V1

## Details

BSOR stands for Beat Saber Open Replay. It's used by Beat Saber mods to encode gameplay.
It contains player movement, scoring, and other data. 

You can use various web-based viewers to play BSOR files:
[BeatLeader](https://replay.beatleader.com/)
[ArcViewer](https://allpoland.github.io/ArcViewer/)

To record and view replays you need to mod your game and install BeatLeader.
Additionally, you need to install [Replay Mod](https://github.com/Metalit/Replay) on a standalone Quest.

BSOR is also used by the first AI Beat Saber player - [CyberRamen](https://www.youtube.com/@CyberRamenBS)

## Implementations

- [C# implementation](https://github.com/BeatLeader/ReplayDecoder/tree/dev)
- [JS implementation](https://github.com/BeatLeader/BeatSaber-Web-Replays/blob/master/src/open-replay-decoder.js)
- [C++ implementation](https://github.com/BeatLeader/beatleader-qmod/blob/master/shared/Models/Replay.hpp)
- [Python implementation](https://github.com/Schippi/py-bsor/blob/main/src/bsor/Bsor.py)
- [Go implementation](https://github.com/motzel/go-bsor/blob/master/bsor/bsor.go)

## Structure

```
0x442d3d69                     - int, unique magic number.
1                              - byte, file version.

0                              - byte, info structure start.
{                              - Info structure.
  version                      - string, Mod version.
  gameVersion                  - string, Game version.
  timestamp;                   - string, play start unix timestamp.
  
  playerID;                    - string, player platform unique id.
  playerName;                  - string, player platform name.
  platform;                    - string, oculus or steam.

  trackingSytem;               - string, tracking system type. (OpenVR, Oculus, etc.)
  hmd;                         - string, headset type. (Oculus Quest, Valve Index, etc.)
  controller;                  - string, controllers type. (Oculus touch, etc)

  hash;                        - string, map hash.
  songName;                    - string, song name.
  mapper;                      - string, mapper name.
  difficulty;                  - string, difficulty name. (Easy, ExpertPlus, etc)

  score                        - int, total unmodified score.
  mode                         - string, game mode. (Standard, OneSaber, Lawless, etc.)
  environment                  - string, environment name. (The beginning, etc.)
  modifiers                    - comma separated string, game modifiers. (FS, GN, etc.)
  jumpDistance                 - float, note jump distance.
  leftHanded                   - bool.
  height                       - float, static height.

  startTime                    - float, song start time (practice mode), seconds.
  failTime                     - float, song fail time (only if failed), seconds.
  speed                        - float, song speed (practice mode).
}

1                              - byte, frames array start.
framesCount                    - int, frames count.
{                              - Frame structure.
  time                         - float, song time, seconds.
  fps                          - int, player's FPS.
  {                            - Head structure.
    {x, y, z}                  - 3 floats, position.
    {x, y, z, w}               - 4 floats, rotation.
  }
  {                            - Left hand structure.
    {x, y, z}                  - 3 floats, position.
    {x, y, z, w}               - 4 floats, rotation.
  }
  {                            - Right hand structure.
    {x, y, z}                  - 3 floats, position.
    {x, y, z, w}               - 4 floats, rotation.
  }
}

2                              - byte, note events array start.
noteCount                      - int, note events count.
{                              - Note event structure.
  noteID                       - int, scoringType*10000 + lineIndex*1000 + noteLineLayer*100 + colorType*10 + cutDirection.
      Where scoringType is game value + 2. Standard values: 
        Normal = 0, 
        Ignore = 1, 
        NoScore = 2, 
        Normal = 3, 
        SliderHead = 4, 
        SliderTail = 5, 
        BurstSliderHead = 6, 
        BurstSliderElement = 7
  eventTime                    - float, song time of event, seconds.
  spawnTime                    - float, spawn time of note, seconds.
  eventType                    - int, good = 0, bad = 1, miss = 2, bomb = 3.
  {                            - Cut info structure (only for Good and Bad!).
    speedOK                    - bool, note was hit at passable speed.
    directionOK                - bool, note was hit with correct direction.
    saberTypeOK                - bool, note was hit with correct saber.
    wasCutTooSoon              - bool, note was cut too soon.
    saberSpeed                 - float, speed of the saber.
    { x, y, z }                - 3 floats, saberDir, direction of the saber.
    saberType                  - int, type of the saber (0 = left, 1 = right).
    timeDeviation              - float, how far away the time hit was from the time expected.
    cutDirDeviation            - float, how far away the cut direction was from the one expected.
    { x, y, z }                - 3 floats, cutPoint, point at which the note was cut.
    { x, y, z }                - 3 floats, cutNormal, cut normal vector.
    cutDistanceToCenter        - float, distance from center of the note to the cut plane, to get the score 15 * (1 - Clamp01(cutDistanceToCenter / 0.3f)). With beforeCutRating and afterCutRating forms 115 score.
    cutAngle                   - float, angle of the cut.
    beforeCutRating            - float, angle rating before the cut. This rating is uncapped (can go over 1). 1 means 70 score.
    afterCutRating             - float, angle rating after the cut. This rating is uncapped (can go over 1). 1 means 30 score.
  }
}

3                              - byte, wall events array start.
wallCount                      - int, wall events count.
{
  wallID                       - int, lineIndex*100 + obstacleType*10 + width.
  energy                       - float, energy at the end of event.
  time                         - float, song time of event, seconds.
  spawnTime                    - float, spawn time of wall, seconds.
}

4                              - byte, automatic height array start.
heightCount                    - int, height change events count.
{
  height                       - float, height value.
  time                         - float, song time, seconds.
}

5                              - byte, pause array start.
pauseCount                     - int, pauses count.
{
  duration                     - long, duration in seconds.
  time                         - float, pause start time, seconds.
}
6                              - byte, start of controller offsets, *OPTIONAL*.
{                              - Left hand structure.
   {x, y, z}                   - 3 floats, position.
   {x, y, z, w}                - 4 floats, rotation.
}
{                              - Right hand structure.
   {x, y, z}                   - 3 floats, position.
   {x, y, z, w}                - 4 floats, rotation.
}
7                              - byte, start of user data, *OPTIONAL*.
userDataLength                 - int, length of userdata.
{
 content                       - byte, variable meaning.
}
```

## .bsor Encoding details
Binary file containing only values without the keys. Values are coded one after another.
Uses Little Endian!

- byte, 1 byte
- int, 4 bytes
- long, 8 bytes
- float, 4 bytes
- bool, 1 byte
- string, int (count) + count bytes

Rotations are stored as quaternions.

# Roadmap

- Create a streaming BSOR format
- Make BSOR v2 with improvements based on usage



