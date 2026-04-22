using System;
using FistVR;
using UnityEngine;
using H3MP.Networking;
using System.Collections.Generic;


namespace TeamsGameMode;

[Serializable]
public class TGM_Payload: TGM_Gamemode
{
    List<Rush_CapturePoint> capturePoints = new List<Rush_CapturePoint>();
    int captureRatio = 4;  //1 in X will get the objective
    int redSpawnRatio = 0;

    public TGM_Payload(string modeName = "", string modeDescription = "", Sprite modeThumbnail = null)
    {
        name = modeName;
        description = modeDescription;
        thumbnail = modeThumbnail;
    }
}