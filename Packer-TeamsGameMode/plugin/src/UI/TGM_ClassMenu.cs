using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TeamsGameMode
{
    public class TGM_ClassMenu : MonoBehaviour
    {
        public static TGM_ClassMenu instance;


        [Header("Menu")]
        public Text spawnCountdown;
        public GameObject buttonPrefab;
        public Transform buttonContent;

        private List<TGM_Button> buttons = new List<TGM_Button>();


        [Header("Spawn Points")]
        public Transform[] mainSpawns;
        public Transform[] ammoSpawns;

        private float spawnRange = 0.1f;



        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if (spawnCountdown.gameObject.activeSelf)
                spawnCountdown.text = TGM_Manager.instance.nextSpawnWave.ToString();
        }


        public void Setup(TGM_PlayerClass[] classes)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                TGM_Button button = Instantiate(buttonPrefab, buttonContent).GetComponent<TGM_Button>();
                buttons.Add(button);
            }
        }

        public void ClearButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            buttons.Clear();
        }

        public void JoinRespawn()
        {
            TGM_Manager.instance.localPlayer.awaitingRespawn = true;
        }

        public void SpawnClass(int id)
        {
            if (id < 0)
            {
                TeamGameModePlugin.Logger.LogMessage(TeamGameModePlugin.Name + "Spawn Class is -1 or less and will not be spawned");
                return;
            }

            int team = TGM_Manager.instance.localPlayer.iff;
        }


        void OnDrawGizmos()
        {
            for (int i = 0; i < mainSpawns.Length; i++)
            {
                if (mainSpawns[i])
                    Gizmos.DrawLine((-mainSpawns[i].forward * spawnRange) + mainSpawns[i].position,
                        (mainSpawns[i].forward * spawnRange) + mainSpawns[i].position);
            }

            for (int i = 0; i < ammoSpawns.Length; i++)
            {
                if (ammoSpawns[i] != null)
                    Gizmos.DrawLine((-ammoSpawns[i].forward * spawnRange) + ammoSpawns[i].position, 
                        (ammoSpawns[i].forward * spawnRange) + ammoSpawns[i].position);
            }
        }
    }
}
