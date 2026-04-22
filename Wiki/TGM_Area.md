# Area

Area's are the main component in defining the relevant location data for Sosigs and Objectives throughout the gamemode


## TGM_Area
| Variable    | Setting |
| -------- | ------- |
| Capture Point  | A transform used to define this area's capture point used for gamemodes |
| Objective | Transforms location where Objectives are spawned for this area, e.g. Flag for CTF or Radios for Rush |
| Capture Time | How long it takes to capture this Objective / Capture Point, default 20 seconds  |
| Spawn Points | A Collection of Spawn point data, should always be a size of 2, one for each team, see Spawn Points below |
| Friendly Objects  | A list of gameobjects that will be enabled when this area is owned by the player's team |
| Enemy Objects  | A list of gameobjects that will be enabled when this area is owned by the player's enemy team |
| Neutral Objects  | A list of gameobjects that will be enabled when this area is not owned by any team |
| Red Objects  | A list of gameobjects that will be enabled when this area owned by the Red Team |
| Blue Objects  | A list of gameobjects that will be enabled when this area owned by the Blue Team |



### Spawn Points
| Variable    | Setting |
| -------- | ------- |
| Name  | For readiblity only, will auto name itself to the correct team. Does nothing |
| Player Spawns | A list of transforms randomly selected, where the Player will spawn |
| Sosig Spawn Points | A list of transforms randomly selected, where sosigs will spawn, if only 1 suppllied it needs to have a scale of at least `6, 0.1, 6` to support 32 sosigs |
| Sosig Attack Areas | List of Transforms that the Spawn Point team's defined sosigs will attack when attacking this area |
| Sosigs Defend Areas  | List of Transforms that the Spawn Point team's defined sosigs will use when defending this area |
