# TGM Scene

The core scene component that allows Teams Game Mode to setup menus and data in the scene


## TGM_Scene
| Variable    | Setting |
| -------- | ------- |
| Teams  | A List containing both teams, see Teams below  |
| Areas  | A List all areas in this map, see the `TGM_Area` page for more information on areas, Areas should be listed from Red (Attackers) to Blue (Defenders) in linear order top to bottom  |
| Avoidance Quailty  | Default LowQualityObstacleAvoidance, should be left on default unless having issues with navigation with other sosigs  |
| Player Reset Point  | Transform used for resetting the player after dying, should sit in the main menu area by default, should contain the `Player Reset Point` component |
| Main Menu  | Transform used for placement of the Main Menu |
| Team Setup Menu  | Transform used for placement of the Team Setup Menu |
| Profiles Menu | Transform used for placement of the Profiles Menu |
| Item Spawner | Transform used for placement of the Item Spawner |
| Spectator Static Cameras | List of Transforms for the spectator camera to view from |
| Rush Capture Prefab | Overwrite for the Rush Capture Prefab if custom one is desired (May be made main prefab instead of overwrite in future) |
| Audio Overwrite | All Audio overwrites if custom audio is desired for special maps, any not filled will use the default sounds |
| Audio Background | Background Audio that plays if filled, audio file should be looped. If blank nothing will play |


### Teams
| Variable    | Setting |
| -------- | ------- |
| Respawn Area  | The location where the player will spawn after joining the team or when they die and respawn |
| Start Spawn Area | Defined first Spawn area for this team, some gamemodes may change this. Default - Red should be first Area on the TGM_Scene `areas` list, Blue should be last area in `areas`  |
| Team Spawn Time | The time between wave respawns for this team, can be overwritten by the player or gamemode |