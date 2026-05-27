using FistVR;
using H3MP.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace TeamsGameMode;

[Serializable]
public class TGM_Payload: TGM_Gamemode
{
    private bool isovertime;
    private float overtimeleft;

    private Payload_Cart cart;

    [HideInInspector] public List<PlSosig> livesosigs = new List<PlSosig>();
    private int checkindex;
    private bool isplayeroncart;
    //private bool hasPlayerDied;
    //private int defendersAlive = 0;
    //private int attackersAlive = 0;
    //private int defendersToSpawn = 0;
    //private int attackersToSpawn = 0;
    //public float timeTillDefenderWave = 0f;
    //public float timeTillAttackerWave = 0f;

    [System.Serializable]
    public class PlSosig
    {
        public Sosig sos;
        public bool isoncart;
        public bool isguard;
        public bool isflanker;
        public int iff;
    }

    public TGM_Payload(string modeName = "", string modeDescription = "", Sprite modeThumbnail = null)
    {
        name = modeName;
        description = modeDescription;
        thumbnail = modeThumbnail;
    }

    public override void LoadDefaultProfile()
    {
        base.LoadDefaultProfile();
        //Do Gamemode Settings here
        TGM_Settings.SetSetting(TGMSettingEnum.TimeLimit, TGM_Scene.instance.payloadCartPrefab.GetComponent<Payload_Cart>().timeLimitOverride);

    }

    private bool IsSetup = false;

    public override void Setup()
    {
        base.Setup();
        //Defaults
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            if (TGM_Manager.instance.team[i].scoreGoal == -1)
                TGM_Manager.instance.team[i].scoreGoal = TGM_Scene.instance.areas.Length - 1;
        }
        TGM_MainMenu.instance.UpdateSettings();

        if (!IsSetup)
        {
            IsSetup = true;
            TGM_Settings.gamemodeSettings = new List<TGM_Settings.Setting>()
            {
                new TGM_Settings.Setting
                {
                    description = "Reverse Mode:",
                    settings = ["Disabled", "Enabled"],
                    type = TGM_Settings.Setting.SettingType.Strings,
                    value = 0,
                    intMin = 0,
                    intMax = 1,
                    intIncrement = 1,
                    localOnly = false,
                }
            };

            TGM_MainMenu.instance.SetupGamemodeSettings();
        }

        //Hide Objective UI
        for (int i = 0; i < TGM_TeamSetup.instance.teamObjectiveAdjust.Length; i++)
        {
            TGM_TeamSetup.instance.teamObjectiveAdjust[i].SetActive(false);
        }
    }
}