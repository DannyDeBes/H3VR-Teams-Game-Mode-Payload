# Player Team

## Player Team
| Variable    | Setting |
| -------- | ------- |
| name  | Player Team name |
| description | A Brief description of the player team |
| sosigSet | A collection of Sosig Sets that will be randomly selected from when spawning a sosig |


### Player Class
| Variable    | Setting |
| -------- | ------- |
| name  | Class's Display Name |
| spriteName | `Example.png` name of the image that will be used as a thumbnail |
| playerHealth    |  How much health this class has, default `5000`   |
| minKills  | The minimum amount of kills required before this class becomes available |
| maxKills  | The maximum amount of kills before it becomes unavailable |

#### Sub Class
| Variable    | Setting |
| -------- | ------- |
| name  | Internal name for this sub class |
| Items | List of all items that are spawned with this class |

##### Item Set
| Variable    | Setting |
| -------- | ------- |
| name  | Internal name for this item set |
| objectCount | How many of ObjectIDs are spawned |
| uniformObjects | If `true`, only one `objectID` is selected and spawned `objectCount` amount of times, if `false` randomly select from the `objectID` list per spawned object |
| requiredSecondaryPieces  | if `objectID` has a required Secondary Pieces, spawn them |
| objectID  | A list of spawnable objectIDs |
| ammoCount  | The amount of cartridges/magazines/speed loaders/clips spawned |
| ammoContainerID  | Specific ObjectID for the ammo container used, will autofill with its default cartridges if no `cartridgeID` is supplied |
| cartridgeID  | The ObjectID of the cartridge, if  `cartridgeID` is blank, `ammoContainerID` will use its default cartridge, if `ammoContainerID` is blank, `cartridgeID` will spawn |