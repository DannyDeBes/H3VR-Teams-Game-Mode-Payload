using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TeamsGameMode
{
    [Serializable]
    public class TGM_PlayerClass
    {
        public string name;
        public Sprite thumbnail;
        public bool canSpawnLock = true;    //Spawnlock per class
        public int minKills = -1;   //Minimum kills before usable
        public int maxKills = -1;   //Maximum kills before unusable
        public List<string> subtractionIDs = new List<string>();    //Stop certain item/ammo types from spawning
        public SubClass[] subClasses;

        [Serializable]
        public class SubClass
        {
            public string name; //For Readiblity mostly
            public ItemSet[] items;
        }

        [Serializable]
        public class ItemSet()
        {
            public string name; //For readiblity
            [Header("Objects")]
            public int objectCount = 1;
            public bool uniformObjects = false; //If True will only spawn one type of objectID
            public string[] objectID;   //Randomly Select per ObjectCount

            [Header("Ammo")]
            [Tooltip("Amount of Magazines/Clips/Ammo that will spawn")]
            public int ammoCount = 0;
            [Tooltip("Magazine/Clip Min Capacity for this loot table")]
            public int minCapacity = -1;
            [Tooltip("Magazine/Clip Max Capacity for this loot table")]
            public int maxCapacity = -1;
            [Tooltip("The preloaded or type the ammo will spawn as, AmmoEnum")]
            public int ammoType = 0;
            [Tooltip("If not blank, specific ammo type will be spawned")]
            public string ammoID = "";
        }

        public class ItemTable()
        {
            public List<FVRObject> mainItems = new List<FVRObject>();
            public List<FVRObject> smallItems = new List<FVRObject>();
        }
    }
}
